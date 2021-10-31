using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for team selection event.
	/// </summary>
	[Serializable]
	public class TeamSelectionEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the team name.
		/// </summary>
		public string Team { get; internal set; }
	}
}
