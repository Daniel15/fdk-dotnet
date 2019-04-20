using System;
using System.Threading.Tasks;

namespace FaasUtils
{
	/// <summary>
	/// Handles building expression trees for functions.
	/// </summary>
	public interface IFunctionExpressionTreeBuilder
	{
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
		Func<T, IServiceProvider, Task<object>> CreateLambda<T>();
	}
}