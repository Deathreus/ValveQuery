using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for player killed event.
	/// </summary>
	[Serializable]
	public class KillEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets Victim player's info.
		/// </summary>
		public LogPlayerInfo Victim { get; internal set; }

		/// <summary>
		/// Gets the name of the weapon used.
		/// </summary>
		public string Weapon { get; internal set; }
	}
}
