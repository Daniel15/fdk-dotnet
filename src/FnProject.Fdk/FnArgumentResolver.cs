using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using FaasUtils;

namespace FnProject.Fdk
{
	/// <summary>
	/// Adds Fn-specific argument resolution to <see cref="ArgumentResolver"/>
	/// </summary>
	internal class FnArgumentResolver : ArgumentResolver
	{
		public override Expression Resolve(ParameterInfo param, ParameterExpression servicesArg)
		{
			var paramType = param.ParameterType;

			// CancellationToken
			// --> Get it from the context
			if (paramType == typeof(CancellationToken))
			{
				return CreateCancellationTokenCall(servicesArg);
			}

			return base.Resolve(param, servicesArg);
		}

		/// <summary>
		/// Creates a call to get the CancellationToken for the request
		/// </summary>
		private Expression CreateCancellationTokenCall(ParameterExpression servicesArg)
		{
			var contextArg = CreateResolveServiceCall(servicesArg, typeof(IContext));
			return Expression.Property(contextArg, nameof(IContext.TimedOut));
		}
	}
}
