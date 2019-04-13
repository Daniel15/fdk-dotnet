using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FnProject.Fdk.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace FnProject.Fdk
{
	/// <summary>
	/// Handles building expression trees for functions.
	/// </summary>
	public static class FunctionExpressionTreeBuilder
	{
		private const string ASYNC_METHOD_NAME = "InvokeAsync";
		private const string SYNC_METHOD_NAME = "Invoke";
		private const string INPUT_PARAM = "input";

		/// <summary>
		/// Creates a delegate to call the specified function class.
		/// </summary>
		/// <remarks>
		/// Given this class:
		///
		///   class MyFunction
		///   {
		///       public Task InvokeAsync(IFoo, string input) { ... }
		///   }
		///
		/// Returns a delegate like:
		///
		///   Task Invoke(MyFunction instance, IContext ctx, IInput input, IServiceProvider services)
		///   {
		///       return instance.InvokeAsync(provider.GetRequiredService(typeof(IFoo)), input.AsString());
		///   }
		/// </remarks>
		/// <typeparam name="T">Type of class containing the InvokeAsync method</typeparam>
		/// <returns>Lambda function</returns>
		public static Func<T, IServiceProvider, Task<object>> CreateLambda<T>()
		{
			var fnType = typeof(T);
			var method =
				fnType.GetMethod(ASYNC_METHOD_NAME, BindingFlags.Instance | BindingFlags.Public)
				?? fnType.GetMethod(SYNC_METHOD_NAME, BindingFlags.Instance | BindingFlags.Public)
				?? throw new InvalidFunctionException(
					$"{fnType.Name} has no {ASYNC_METHOD_NAME} or {SYNC_METHOD_NAME} method."
				);

			var instanceArg = Expression.Parameter(fnType, "instance");
			var servicesArg = Expression.Parameter(typeof(IServiceProvider), "services");

			var getServiceMethod = typeof(ServiceProviderServiceExtensions).GetMethod(
				"GetRequiredService",
				new[] { typeof(IServiceProvider), typeof(Type) }
			);

			var inputArg = CreateResolveServiceCall(servicesArg, typeof(IInput), getServiceMethod);

			var callArgs = method.GetParameters().Select(param =>
			{
				var paramType = param.ParameterType;

				if (paramType == typeof(IServiceProvider))
				{
					return servicesArg;
				}

				// CancellationToken
				// --> Get it from the context
				if (paramType == typeof(CancellationToken))
				{
					return CreateCancellationTokenCall(servicesArg, getServiceMethod);
				}

				// String => assume it's the raw input
				// --> input.AsString()
				if (paramType == typeof(string))
				{
					return CreateAsStringCall(inputArg);
				}

				// Interface => Assume it needs to be resolved through the DI container
				// --> (IFoo)ServiceProviderServiceExtensions.GetRequiredService(services, typeof(IFoo))
				if (paramType.IsInterface)
				{
					return CreateResolveServiceCall(servicesArg, paramType, getServiceMethod);
				}

				// Class with name of "input" => Assume it's coming from JSON
				if (paramType.IsClass && param.Name == INPUT_PARAM)
				{
					return CreateAsJsonCall(inputArg, paramType);
				}

				// Unrecognised - Bail out
				throw new InvalidOperationException($"Unrecognized parameter type {paramType} for {param.Name}");
			});

			var body = Expression.Call(instanceArg, method, callArgs);

			// If it's not async, wrap it in Task.FromResult(...)
			if (method.ReturnType.BaseType != typeof(Task))
			{
				body = CreateTaskFromResultCall(body);
			}
			// If method doesn't return Task<object> (eg. it returns Task<string>), explicitly cast result to object
			// This is required because Task<T> isn't covariant :(
			else if (
				method.ReturnType.BaseType == typeof(Task) && 
				method.ReturnType.IsGenericType &&
				method.ReturnType.GenericTypeArguments[0] != typeof(object))
			{
				body = CreateUpcastTaskCall(body, method.ReturnType.GenericTypeArguments[0]);
			}

			var lambda = Expression.Lambda<Func<T, IServiceProvider, Task<object>>>(
				body,
				"FunctionExpressionTreeInvoke",
				new [] {
					instanceArg,
					servicesArg
				}
			);
			return lambda.Compile();
		}

		/// <summary>
		/// Creates a call to <see cref="ServiceProviderServiceExtensions.GetRequiredService"/>
		/// </summary>
		private static Expression CreateResolveServiceCall(
			ParameterExpression servicesArg, 
			Type paramType,
			MethodInfo getServiceMethod
		)
		{
			var getServiceArgs = new Expression[]
			{
				servicesArg,
				Expression.Constant(paramType, typeof(Type))
			};
			var getServiceCall = Expression.Call(null, getServiceMethod, getServiceArgs);
			return Expression.Convert(getServiceCall, paramType);
		}

		/// <summary>
		/// Creates a call to <see cref="IInput.AsJson"/>
		/// </summary>
		private static Expression CreateAsJsonCall(Expression inputArg, Type paramType)
		{
			return Expression.Call(inputArg, nameof(IInput.AsJson), new[] {paramType});
		}

		/// <summary>
		/// Creates a call to <see cref="IInput.AsString"/>
		/// </summary>
		private static Expression CreateAsStringCall(Expression inputArg)
		{
			return Expression.Call(inputArg, typeof(IInput).GetMethod(nameof(IInput.AsString)));
		}

		/// <summary>
		/// If method doesn't return Task{object} (eg. it returns Task{string}), explicitly cast
		/// result to object. This is required because <see cref="Task{TResult}"/> isn't covariant :(
		/// </summary>
		/// <param name="task">Expression that returns a Task{TResult}</param>
		/// <returns>Expression that returns a Task{object}</returns>
		private static MethodCallExpression CreateUpcastTaskCall(
			MethodCallExpression task,
			Type taskType
		)
		{
			var upcastMethod = typeof(FunctionExpressionTreeBuilder)
				.GetMethod(nameof(UpcastTask))
				.MakeGenericMethod(taskType);
			return Expression.Call(null, upcastMethod, task);
		}

		/// <summary>
		/// Cast Task{T} to Task{object}
		/// </summary>
		/// <typeparam name="T">Type of the task</typeparam>
		/// <param name="task">The task to upcast</param>
		/// <returns>Task with its result upcasted to <see cref="Object"/>.</returns>
		public static Task<object> UpcastTask<T>(Task<T> task)
		{
			return task.ContinueWith(x => (object) x.Result);
		}

		/// <summary>
		/// Creates a <see cref="Task.FromResult{TResult}"/> call to wrap a result in a Task.
		/// </summary>
		private static MethodCallExpression CreateTaskFromResultCall(MethodCallExpression result)
		{
			var taskFromResultMethod = typeof(Task)
				.GetMethod(nameof(Task.FromResult))
				.MakeGenericMethod(typeof(object));
			return Expression.Call(null, taskFromResultMethod, result);
		}

		/// <summary>
		/// Creates a call to get the CancellationToken for the request
		/// </summary>
		private static Expression CreateCancellationTokenCall(ParameterExpression servicesArg, MethodInfo getServiceMethod)
		{
			var contextArg = CreateResolveServiceCall(servicesArg, typeof(IContext), getServiceMethod);
			return Expression.Property(contextArg, nameof(IContext.TimedOut));
		}
	}
}
