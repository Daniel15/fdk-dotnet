using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents a Fn function that is invoked by instantiating a class and calling
	/// its <c>InvokeAsync</c> method.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FunctionClassWrapper<T> : IFunction where T : class
	{
		private readonly Func<T, IServiceProvider, Task<object>> _function;
		private readonly T _instance;

		public FunctionClassWrapper(IServiceProvider services)
		{
			_instance = ActivatorUtilities.CreateInstance<T>(services);
			_function = FunctionExpressionTreeBuilder.CreateLambda<T>();
		}

		/// <summary>
		/// Invokes the specified function
		/// </summary>
		/// <param name="ctx">Execution context for the function</param>
		/// <param name="input">Input to the function</param>
		/// <param name="services">Service provider for dependency injection</param>
		/// <returns>Data to return from the request</returns>
		public Task<object> InvokeAsync(IContext ctx, IInput input, IServiceProvider services)
		{
			return _function(_instance, services);
		}
	}
}
