using System.Linq.Expressions;
using System.Reflection;

namespace FaasUtils
{
	/// <summary>
	/// Resolves arguments for functions passed in to <see cref="FunctionExpressionTreeBuilder"/>.
	/// </summary>
	public interface IArgumentResolver
	{
		/// <summary>
		/// Builds an expression to compute the value that will be passed for this argument.
		/// </summary>
		/// <param name="param">Parameter to build expression for</param>
		/// <returns>Expression</returns>
		Expression Resolve(ParameterInfo param, ParameterExpression servicesArg);
	}
}
