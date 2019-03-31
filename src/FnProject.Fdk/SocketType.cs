namespace FnProject.Fdk
{
	/// <summary>
	/// The type of socket being used to listen
	/// </summary>
	public enum SocketType
	{
		/// <summary>
		/// UNIX socket
		/// </summary>
		Unix,

		/// <summary>
		/// TCP socket
		/// </summary>
		Tcp,

		/// <summary>
		/// Invalid configuration
		/// </summary>
		Unknown,
	}
}
