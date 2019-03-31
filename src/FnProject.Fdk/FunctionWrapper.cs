using System.Threading.Tasks;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents a Fn function that can be invoked through a delegate.
	/// </summary>
	public class FunctionWrapper : IFunction
	{
		private readonly FdkFunction _function;

		public FunctionWrapper(FdkFunction function)
		{
			_function = function;
		}

		/// <summary>
		/// Invokes the function
		/// </summary>
		/// <param name="ctx">Execution context for the function</param>
		/// <param name="input">Input to the function</param>
		/// <returns>Data to return from the request</returns>
		public Task<object> InvokeAsync(IContext ctx, IInput input)
		{
			return _function(ctx, input);
		}

		/// <summary>
		/// Handles for Fn functions
		/// </summary>
		/// <param name="ctx">Execution context for the function</param>
		/// <param name="input">Input to the function</param>
		/// <returns>Data to return from the request</returns>
		public delegate Task<object> FdkFunction(IContext ctx, IInput input);
	}
}
