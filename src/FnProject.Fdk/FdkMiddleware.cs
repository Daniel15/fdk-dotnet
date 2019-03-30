using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk
{
	/// <summary>
	/// ASP.NET Core middleware to handle function requests
	/// </summary>
	public class FdkMiddleware
	{
		private readonly IFunction _function;

		public FdkMiddleware(RequestDelegate next, IFunction function)
		{
			_function = function;
		}

		public async Task InvokeAsync(HttpContext httpContext, IContext fnContext)
		{
			var input = new Input();
			// TODO: Format this properly
			// TODO: Handle errors (maybe in separate middleware to handle errors elsewhere?)
			var result = await _function.InvokeAsync(fnContext, input);
			await httpContext.Response.WriteAsync((string)result);
		}
	}
}
