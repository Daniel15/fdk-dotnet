using System;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents the context that a function is executing in
	/// </summary>
	public class Context : IContext
	{
		private readonly HttpRequest _request;

		public Context(IConfig config, IHttpContextAccessor httpContextAccessor)
		{
			_request = httpContextAccessor.HttpContext.Request;
			Config = config;
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
		public IHeaderDictionary Headers => _request.Headers;

		/// <summary>
		/// Gets the method for this event
		/// </summary>
		public string Method => _request.Headers["Fn-Http-Method"];

		/// <summary>
		/// Gets the HTTP request URL for this event
		/// </summary>
		public string RequestUrl => _request.Headers["Fn-Http-Request-Url"];
	}
}
