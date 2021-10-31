using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Contains information of a player.
	/// </summary>
	[Serializable]
	public class LogPlayerInfo
	{
		/// <summary>
		/// Name of player.
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// UId of player(Steam ID).
		/// </summary>
		public string Uid { get; internal set; }

		/// <summary>
		/// Won Id.
		/// </summary>
		public string WonId { get; internal set; }

		/// <summary>
		/// Player's Team Name.
		/// </summary>
		public string Team { get; internal set; }
	}
}
