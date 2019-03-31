using System.Threading.Tasks;
using FnProject.Fdk.Result;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk.Middleware
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

		public async Task InvokeAsync(HttpContext httpContext, IContext fnContext, IInput input)
		{
			var rawResult = await _function.InvokeAsync(fnContext, input);
			var result = ResultFactory.Create(rawResult);
			await result.WriteResult(httpContext.Response);
		}
	}
}
