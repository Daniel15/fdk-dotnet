using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
			Handle(services =>
			{
				services.AddSingleton(new FunctionWrapper(function));
			});
		}

		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <param name="function">Function to handle requests</param>
		public static void Handle(Func<IContext, IInput, Task<object>> function)
		{
			Handle(services =>
			{
				services.AddSingleton(new FunctionWrapper(function));
			});
		}

		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <typeparam name="T">Type of function class to serve</typeparam>
		public static void Handle<T>() where T : class
		{
			Handle(services =>
			{
				if (typeof(IFunction).IsAssignableFrom(typeof(T)))
				{
					// T directly implements IFunction.
					services.AddSingleton(typeof(IFunction), typeof(T));
				}
				else
				{
					// T doesn't implement IFunction, so assume it's a generic class that
					// needs to be wrapped.
					services.AddSingleton<IFunction, FunctionClassWrapper<T>>();
				}
			});
		}

		/// <summary>
		/// Configure the request handler. This starts the web server and waits for requests.
		/// </summary>
		/// <param name="configureServices">Delegate to configure services in dependency injection container</param>
		private static void Handle(Action<IServiceCollection> configureServices)
		{
			HttpServer.CreateWebHostBuilder(new Config(), configureServices)
				.Build()
				.Run();
		}
	}
}
