using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for rcon event.
	/// </summary>
	[Serializable]
	public class RconEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Challenge Id of remote client.
		/// </summary>
		public string Challenge { get; internal set; }

		/// <summary>
		/// Gets Password.
		/// </summary>
		public string Password { get; internal set; }

		/// <summary>
		/// Gets command sent by remote client.
		/// </summary>
		public string Command { get; internal set; }

		/// <summary>
		/// Gets IP-Address of client.
		/// </summary>
		public string Ip { get; internal set; }

		/// <summary>
		/// Gets Port number of client.
		/// </summary>
		public ushort Port { get; internal set; }

		/// <summary>
		/// Returns true if password sent is valid.
		/// </summary>
		public bool IsValid { get; internal set; }
	}
}
