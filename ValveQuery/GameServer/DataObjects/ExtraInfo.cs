using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Contains extra information about server.
	/// </summary>
	[Serializable]
	public class ExtraInfo : DataObject
	{
		/// <summary>
		/// The server's game port number.
		/// </summary>
		public ushort Port { get; internal set; }

		/// <summary>
		/// Server's SteamID. 
		/// </summary>
		public ulong SteamId { get; internal set; }

		/// <summary>
		/// Contains information on Source TV.(if it is Source TV).
		/// </summary>
		public SourceTVInfo SpecInfo { get; internal set; }

		/// <summary>
		/// Tags that describe the game according to the server. 
		/// </summary>
		public string Keywords { get; internal set; }

		/// <summary>
		/// The server's 64-bit GameID.
		/// </summary>
		public ulong GameId { get; internal set; }
	}
}
