using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents the context that a function is executing in
	/// </summary>
	public class Context : IContext
	{
		/// <summary>
		/// Prefix used before HTTP headers that are passed to the function
		/// </summary>
		private const string HEADER_PREFIX = "Fn-Http-H-";

		private readonly HttpRequest _request;

		public Context(IConfig config, IHttpContextAccessor httpContextAccessor)
		{
			_request = httpContextAccessor.HttpContext.Request;
			Config = config;
			Headers = GetFnHeaders(_request.Headers);
		}

		/// <summary>
		/// Gets the unique ID assigned to this request.
		/// </summary>
		public string CallId => _request.Headers["Fn-Call-Id"];

		/// <summary>
		/// Gets the system configuration.
		/// </summary>
		public IConfig Config { get; }

		/// <summary>
		/// Get the date/time after which the request will be aborted.
		/// </summary>
		public DateTime? Deadline
		{
			get
			{
				var rawHeader = _request.Headers["Fn-Deadline"];
				if (rawHeader.Count == 0)
				{
					return null;
				}
				return DateTime.Parse(rawHeader);
			}
		}

		/// <summary>
		/// Get all HTTP headers for the request
		/// </summary>
		public IHeaderDictionary Headers { get; }

		/// <summary>
		/// Gets the method for this event
		/// </summary>
		public string Method => _request.Headers["Fn-Http-Method"];

		/// <summary>
		/// Gets the HTTP request URL for this event
		/// </summary>
		public string RequestUrl => _request.Headers["Fn-Http-Request-Url"];

		/// <summary>
		/// Gets the cancellation token to handle aborting the request if it takes too long.
		/// </summary>
		public CancellationToken TimedOut { get; set; }

		/// <summary>
		/// Gets the headers that should be passed to the function.
		/// </summary>
		/// <param name="allHeaders">All headers passed to the request</param>
		/// <returns>Headers to pass to the function</returns>
		private static IHeaderDictionary GetFnHeaders(IHeaderDictionary allHeaders)
		{
			return new HeaderDictionary(
				allHeaders
					.Where(pair => pair.Key.StartsWith(HEADER_PREFIX))
					.ToDictionary(pair => pair.Key.Substring(HEADER_PREFIX.Length), pair => pair.Value)
			);
		}
	}
}
