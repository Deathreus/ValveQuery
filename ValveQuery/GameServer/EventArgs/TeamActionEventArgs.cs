using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for team action event.
	/// </summary>
	[Serializable]
	public class TeamActionEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets the name of the team who triggered an action.
		/// </summary>
		public string Team { get; internal set; }

		/// <summary>
		/// Gets the name of the action performed.
		/// </summary>
		public string Action { get; internal set; }
	}
}
