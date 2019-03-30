using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FnProject.Fdk
{
	/// <summary>
	/// Configures the Kestrel web server for FDK usage
	/// </summary>
	internal class HttpServer
	{
		/// <summary>
		/// Configures services in the dependency injection container
		/// </summary>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IConfig, Config>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddScoped<IContext, Context>();
		}

		/// <summary>
		/// Configures the web server
		/// </summary>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseMiddleware<FdkMiddleware>();
		}

		/// <summary>
		/// Starts the web server
		/// </summary>
		public static void Start(IFunction function)
		{
			WebHost.CreateDefaultBuilder()
				.UseStartup<HttpServer>()
				.ConfigureServices(services =>
				{
					services.AddSingleton(function);
				})
				.Build()
				.Run();
		}
	}
}
