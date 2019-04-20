using System;
using System.Diagnostics;
using System.IO;
using FaasUtils;
using FaasUtils.Extensions;
using FnProject.Fdk.Middleware;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mono.Unix.Native;

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
			services.AddFaasUtils();
			services.Replace(ServiceDescriptor.Transient<IArgumentResolver, FnArgumentResolver>());

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddScoped<IContext, Context>();
		}

		/// <summary>
		/// Configures the web server
		/// </summary>
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			IApplicationLifetime appLifetime,
			IConfig config
		)
		{
			app.UseJsonExceptionHandler(isDev: env.IsDevelopment());
			app.UseMiddleware<FdkMiddleware>();

			if (config.ListenerSocketType == SocketType.Unix)
			{
				appLifetime.ApplicationStarted.Register(() => SetPermissionsOnUnixSocket(config));
				appLifetime.ApplicationStopped.Register(() => File.Delete(config.ListenerUnixSocketPath));
			}
		}

		/// <summary>
		/// Starts the web server
		/// </summary>
		public static IWebHostBuilder CreateWebHostBuilder(IConfig config, Action<IServiceCollection> configureServices)
		{
			ConfigValidator.Validate(config);
			return WebHost.CreateDefaultBuilder()
				.UseStartup<HttpServer>()
				.UseLibuv()
				.ConfigureKestrel(options =>
				{
					// We deviate from the official FDK spec slightly, and allow both TCP sockets *and*
					// UNIX sockets. TCP sockets are useful for debugging on Windows.
					switch (config.ListenerSocketType)
					{
						case SocketType.Tcp:
							options.Listen(config.ListenerTcpEndpoint);
							break;
						case SocketType.Unix:
							options.ListenUnixSocket(config.ListenerUnixSocketPath + ".tmp");
							break;
					}
				})
				.ConfigureServices(services =>
				{
					services.AddSingleton(config);
					configureServices(services);
				});
		}

		/// <summary>
		/// Sets the proper permissions on the UNIX socket used by the server.
		/// Kestrel doesn't support setting the permissions on the socket, so we
		/// need to create it in a temporary location, then symlink it across. 
		/// This is the same as what the Node.js and Go FDKs do.
		/// </summary>
		/// <param name="config">Configuration</param>
		private void SetPermissionsOnUnixSocket(IConfig config)
		{
			var unixSocket = config.ListenerUnixSocketPath;
			var tempUnixSocket = unixSocket + ".tmp";
			Syscall.chmod(
				tempUnixSocket,
				FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | 
				FilePermissions.S_IRGRP | FilePermissions.S_IWGRP |
				FilePermissions.S_IROTH | FilePermissions.S_IWOTH
			);

			var tempUnixSocketName = Path.GetFileName(tempUnixSocket);
			Syscall.symlink(tempUnixSocketName, unixSocket);

			Console.WriteLine("UNIX socket: {0}", unixSocket);
		}
	}
}
