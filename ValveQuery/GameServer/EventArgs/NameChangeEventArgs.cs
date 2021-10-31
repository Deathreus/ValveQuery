using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for player name change event.
	/// </summary>
	[Serializable]
	public class NameChangeEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets player's new name.
		/// </summary>
		public string NewName { get; internal set; }
	}
}
