using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FaasUtils
{
	/// <summary>
	/// Resolves arguments for functions passed in to <see cref="FunctionExpressionTreeBuilder"/>.
	/// </summary>
	public class ArgumentResolver : IArgumentResolver
	{
		private const string INPUT_PARAM = "input";

		private readonly MethodInfo _getServiceMethod;

		public ArgumentResolver()
		{
			_getServiceMethod = typeof(ServiceProviderServiceExtensions).GetMethod(
				"GetRequiredService",
				new[] {typeof(IServiceProvider), typeof(Type)}
			);
		}

		/// <summary>
		/// Builds an expression to compute the value that will be passed for this argument.
		/// </summary>
		/// <param name="param">Parameter to build expression for</param>
		/// <param name="servicesArg">Argument where the IServiceProvider is passed</param>
		/// <returns>Expression</returns>
		public virtual Expression Resolve(ParameterInfo param, ParameterExpression servicesArg)
		{
			var paramType = param.ParameterType;

			if (paramType == typeof(IServiceProvider))
			{
				return servicesArg;
			}

			var inputArg = CreateResolveServiceCall(servicesArg, typeof(IInput));

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
				return CreateResolveServiceCall(servicesArg, paramType);
			}

			// Class with name of "input" => Assume it's coming from JSON
			if (paramType.IsClass && param.Name == INPUT_PARAM)
			{
				return CreateAsJsonCall(inputArg, paramType);
			}

			return null;
		}

		/// <summary>
		/// Creates a call to <see cref="ServiceProviderServiceExtensions.GetRequiredService"/>
		/// </summary>
		protected Expression CreateResolveServiceCall(
			ParameterExpression servicesArg,
			Type paramType
		)
		{
			var getServiceArgs = new Expression[]
			{
				servicesArg,
				Expression.Constant(paramType, typeof(Type))
			};
			var getServiceCall = Expression.Call(null, _getServiceMethod, getServiceArgs);
			return Expression.Convert(getServiceCall, paramType);
		}

		/// <summary>
		/// Creates a call to <see cref="IInput.AsJson"/>
		/// </summary>
		private Expression CreateAsJsonCall(Expression inputArg, Type paramType)
		{
			return Expression.Call(inputArg, nameof(IInput.AsJson), new[] { paramType });
		}

		/// <summary>
		/// Creates a call to <see cref="IInput.AsString"/>
		/// </summary>
		private Expression CreateAsStringCall(Expression inputArg)
		{
			return Expression.Call(inputArg, typeof(IInput).GetMethod(nameof(IInput.AsString)));
		}
	}
}
