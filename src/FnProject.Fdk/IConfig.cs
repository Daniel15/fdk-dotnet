using System.Net;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents the system-wide configuration.
	/// </summary>
	public interface IConfig
	{
		/// <summary>
		/// Gets the name of this function's app.
		/// </summary>
		string AppName { get; }

		/// <summary>
		/// Gets the application ID associated with this function.
		/// </summary>
		string AppId { get; }

		/// <summary>
		/// Gets the format of the data stream (deprecated, always http-stream)
		/// </summary>
		string Format { get; }

		/// <summary>
		/// Gets the function ID associated with this function.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the path of the UNIX socket to accept requests.
		/// </summary>
		string Listener { get; }

		/// <summary>
		/// Gets the type of socket being listened on.
		/// </summary>
		SocketType ListenerSocketType { get; }

		/// <summary>
		/// Gets the path of the UNIX socket to accept request, or <c>null</c> if not using a socket.
		/// </summary>
		string ListenerUnixSocketPath { get; }

		/// <summary>
		/// Gets the TCP endpoint being listened on, or <c>null</c> if not using TCP.
		/// </summary>
		IPEndPoint ListenerTcpEndpoint { get; }

		/// <summary>
		/// Gets the amount of RAM (in MB) allocated to this function
		/// </summary>
		int Memory { get; }

		/// <summary>
		/// Gets the name of this function.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the amount of writable disk space available under /tmp for the call, in MB
		/// </summary>
		int TempSize { get; }
	}
}
