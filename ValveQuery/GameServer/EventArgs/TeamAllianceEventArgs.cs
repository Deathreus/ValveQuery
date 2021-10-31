using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for TeamAlliance event.
	/// </summary>
	[Serializable]
	public class TeamAllianceEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets the name of 1st team.
		/// </summary>
		public string Team1 { get; internal set; }

		/// <summary>
		/// Gets the name of 2nd team.
		/// </summary>
		public string Team2 { get; internal set; }
	}
}
