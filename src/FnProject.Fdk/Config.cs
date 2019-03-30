using System;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents the system-wide configuration obtained through environment variables.
	/// </summary>
	public class Config : IConfig
	{
		/// <summary>
		/// Gets the name of this function's app.
		/// </summary>
		public string AppName => Environment.GetEnvironmentVariable("FN_APP_NAME");

		/// <summary>
		/// Gets the application ID associated with this function.
		/// </summary>
		public string AppId => Environment.GetEnvironmentVariable("FN_APP_ID");

		/// <summary>
		/// Gets the format of the data stream (deprecated, always http-stream)
		/// </summary>
		public string Format => Environment.GetEnvironmentVariable("FN_FORMAT");

		/// <summary>
		/// Gets the function ID associated with this function.
		/// </summary>
		public string Id => Environment.GetEnvironmentVariable("FN_ID");

		/// <summary>
		/// Gets the path of the UNIX socket to accept requests.
		/// </summary>
		public string Listener => Environment.GetEnvironmentVariable("FN_LISTENER");

		/// <summary>
		/// Gets the amount of RAM (in MB) allocated to this function
		/// </summary>
		public int Memory => int.Parse(Environment.GetEnvironmentVariable("FN_MEMORY"));

		/// <summary>
		/// Gets the name of this function.
		/// </summary>
		public string Name => Environment.GetEnvironmentVariable("FN_NAME");

		/// <summary>
		/// Gets the amount of writable disk space available under /tmp for the call, in MB
		/// </summary>
		public int TempSize => int.Parse(Environment.GetEnvironmentVariable("FN_TMPSIZE"));
	}
}
