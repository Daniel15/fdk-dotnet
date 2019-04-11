using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		private const string METHOD_NAME = "InvokeAsync";
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
		public static Func<T, IContext, IInput, IServiceProvider, Task<object>> CreateLambda<T>()
		{
			var fnType = typeof(T);
			var method = fnType.GetMethod(METHOD_NAME, BindingFlags.Instance | BindingFlags.Public);
			if (method == null)
			{
				throw new InvalidFunctionException($"{fnType.Name} has no {METHOD_NAME} method");
			}

			var instanceArg = Expression.Parameter(fnType, "instance");
			var servicesArg = Expression.Parameter(typeof(IServiceProvider), "services");
			var contextArg = Expression.Parameter(typeof(IContext), "ctx");
			var inputArg = Expression.Parameter(typeof(IInput), "input");

			var getServiceMethod = typeof(ServiceProviderServiceExtensions).GetMethod(
				"GetRequiredService",
				new[] { typeof(IServiceProvider), typeof(Type) }
			);

			var callArgs = method.GetParameters().Select(param =>
			{
				var paramType = param.ParameterType;

				// Standard params
				if (paramType == typeof(IServiceProvider))
				{
					return servicesArg;
				}

				if (paramType == typeof(IContext))
				{
					return contextArg;
				}

				if (paramType == typeof(IInput))
				{
					return inputArg;
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

			var lambda = Expression.Lambda<Func<T, IContext, IInput, IServiceProvider, Task<object>>>(
				body,
				instanceArg,
				contextArg,
				inputArg,
				servicesArg
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
		private static Expression CreateAsJsonCall(ParameterExpression inputArg, Type paramType)
		{
			return Expression.Call(inputArg, nameof(IInput.AsJson), new[] {paramType});
		}

		/// <summary>
		/// Creates a call to <see cref="IInput.AsString"/>
		/// </summary>
		private static Expression CreateAsStringCall(ParameterExpression inputArg)
		{
			return Expression.Call(inputArg, typeof(IInput).GetMethod(nameof(IInput.AsString)));
		}
	}
}
