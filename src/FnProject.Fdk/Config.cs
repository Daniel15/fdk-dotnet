using System;
using System.Net;

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
		/// Gets the type of socket being listened on.
		/// </summary>
		public SocketType ListenerSocketType
		{
			get
			{
				if (Listener.StartsWith(Constants.UNIX_SOCKET_PREFIX))
				{
					return SocketType.Unix;
				}
				return Listener.StartsWith(Constants.TCP_PREFIX) ? SocketType.Tcp : SocketType.Unknown;
			}
		}

		/// <summary>
		/// Gets the path of the UNIX socket to accept request, or <c>null</c> if not using a socket.
		/// </summary>
		public string ListenerUnixSocketPath =>
			ListenerSocketType == SocketType.Unix
				? Listener.Substring(Constants.UNIX_SOCKET_PREFIX.Length)
				: null;

		/// <summary>
		/// Gets the TCP endpoint being listened on, or <c>null</c> if not using TCP.
		/// </summary>
		public IPEndPoint ListenerTcpEndpoint
		{
			get
			{
				if (ListenerSocketType != SocketType.Tcp)
				{
					return null;
				}
				var tcpSocket = Listener.Substring(Constants.TCP_PREFIX.Length);
				var parts = tcpSocket.Split(':');
				return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
			}
		}

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
