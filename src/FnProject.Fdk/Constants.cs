namespace FnProject.Fdk
{
	/// <summary>
	/// Various constants
	/// </summary>
	internal static class Constants
	{
		/// <summary>
		/// Prefix for UNIX socket paths (eg. "unix:/run/foo.sock")
		/// </summary>
		public const string UNIX_SOCKET_PREFIX = "unix:";

		/// <summary>
		/// Prefix when listening via TCP (eg. "http://127.0.0.1:8080")
		/// </summary>
		public const string TCP_PREFIX = "http://";
	}
}
