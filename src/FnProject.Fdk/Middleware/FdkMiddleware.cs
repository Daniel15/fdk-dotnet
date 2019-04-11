using System;
using System.Threading;
using System.Threading.Tasks;
using FnProject.Fdk.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FnProject.Fdk.Middleware
{
	/// <summary>
	/// ASP.NET Core middleware to handle function requests
	/// </summary>
	public class FdkMiddleware
	{
		private readonly IFunction _function;
		private readonly ILogger<FdkMiddleware> _logger;
		private readonly IServiceProvider _services;

		public FdkMiddleware(
			RequestDelegate next, 
			IFunction function, 
			ILogger<FdkMiddleware> logger, 
			IServiceProvider services
		)
		{
			_function = function;
			_logger = logger;
			_services = services;
		}

		/// <summary>
		/// Middleware entry point. Called by Kestrel.
		/// </summary>
		public async Task InvokeAsync(HttpContext httpContext, IContext fnContext, IInput input)
		{
			var rawResult = await RunFunctionAsync(fnContext, input);
			var result = ResultFactory.Create(rawResult);
			await result.WriteResult(httpContext.Response);
		}

		/// <summary>
		/// Executes the function. If it does not complete within the specified time frame,
		/// returns a timeout error.
		/// </summary>
		private async Task<object> RunFunctionAsync(IContext fnContext, IInput input)
		{
			var tokenSource = new CancellationTokenSource();
			fnContext.TimedOut = tokenSource.Token;
			var timeUntilTimeout = GetTimeUntilTimeout(fnContext);

			if (timeUntilTimeout == null)
			{
				// No timeout, just run the function directly
				return await _function.InvokeAsync(fnContext, input, _services);
			}
			
			var functionTask = _function.InvokeAsync(fnContext, input, _services);
			var resultOrCancellation = await Task.WhenAny(
				functionTask,
				Task.Delay(timeUntilTimeout.Value, tokenSource.Token)
			);

			// Cancel the `Task.Delay` if the function completed, or the function itself if it
			// didn't complete in the allotted time.
			tokenSource.Cancel();

			if (resultOrCancellation == functionTask)
			{
				// Function completed before the timeout
				return functionTask.Result;
			}

			// Function didn't complete before the timeout
			_logger.LogWarning("Function timed out after {timeUntilTimeout}", timeUntilTimeout);
			return new RawResult(string.Empty)
			{
				HttpStatus = StatusCodes.Status504GatewayTimeout,
			};
		}

		/// <summary>
		/// Gets a <see cref="TimeSpan"/> representing the period of time until the deadline.
		/// </summary>
		private static TimeSpan? GetTimeUntilTimeout(IContext fnContext)
		{
			if (fnContext.Deadline == null)
			{
				return null;
			}

			var timeUntilTimeout = fnContext.Deadline.Value - DateTime.Now;
			// Disregard deadline if it's in the past
			if (timeUntilTimeout.TotalSeconds < 0)
			{
				return null;
			}
			return timeUntilTimeout;
		}
	}
}
