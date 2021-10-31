using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for player connect event.
	/// </summary>
	[Serializable]
	public class ConnectEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets IP-Address of client.
		/// </summary>
		public string Ip { get; internal set; }

		/// <summary>
		/// Gets Port number of client.
		/// </summary>
		public ushort Port { get; internal set; }
	}
}
