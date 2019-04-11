using System;
using System.Threading.Tasks;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents a Fn function that can be invoked through a delegate.
	/// </summary>
	public class FunctionWrapper : IFunction
	{
		private readonly Func<IContext, IInput, Task<object>> _function;

		public FunctionWrapper(Func<IContext, IInput, Task<object>> function)
		{
			_function = function;
		}

		public FunctionWrapper(Func<IContext, IInput, object> function)
		{
			_function = (ctx, input) => Task.FromResult(function(ctx, input));
		}

		/// <summary>
		/// Invokes the function
		/// </summary>
		/// <param name="ctx">Execution context for the function</param>
		/// <param name="input">Input to the function</param>
		/// <param name="services">Service provider for dependency injection</param>
		/// <returns>Data to return from the request</returns>
		public Task<object> InvokeAsync(IContext ctx, IInput input, IServiceProvider services)
		{
			return _function(ctx, input);
		}
	}
}
