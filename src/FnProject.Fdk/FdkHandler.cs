using System;
using System.Threading.Tasks;

namespace FnProject.Fdk
{
	/// <summary>
	/// Handler for FDK requests
	/// </summary>
	public static class FdkHandler
	{
		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <param name="function">Function to handle requests</param>
		public static void Handle(Func<IContext, IInput, object> function)
		{
			Handle(new FunctionWrapper(function));
		}

		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <param name="function">Function to handle requests</param>
		public static void Handle(Func<IContext, IInput, Task<object>> function)
		{
			Handle(new FunctionWrapper(function));
		}

		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <param name="function">Function to handle requests</param>
		public static void Handle(IFunction function)
		{
			HttpServer.Start(function, new Config());
		}
	}
}
