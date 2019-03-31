using System.Runtime.InteropServices;
using FnProject.Fdk.Exceptions;

namespace FnProject.Fdk
{
	/// <summary>
	/// Handles validation of the configuration
	/// </summary>
	public static class ConfigValidator
	{
		/// <summary>
		/// Validates that the specified config is valid.
		/// </summary>
		public static void Validate(IConfig config)
		{
			if (config.Format != "http-stream")
			{
				throw new InvalidConfigException("http-stream is the only supported format");
			}

			if (config.ListenerSocketType == SocketType.Unknown)
			{
				throw new InvalidConfigException("Invalid listener: " + config.Listener);
			}

			if (
				RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
				config.ListenerSocketType == SocketType.Unix
			)
			{
				throw new InvalidConfigException("UNIX sockets are not supported on Windows");
			}
		}
	}
}
