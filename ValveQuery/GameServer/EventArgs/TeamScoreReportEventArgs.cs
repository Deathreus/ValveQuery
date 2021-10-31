using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for TeamScoreReport event.
	/// </summary>
	[Serializable]
	public class TeamScoreReportEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets the name of team.
		/// </summary>
		public string Team { get; internal set; }

		/// <summary>
		/// Gets the score of team.
		/// </summary>
		public string Score { get; internal set; }

		/// <summary>
		/// Gets the player count.
		/// </summary>
		public string PlayerCount { get; internal set; }

		/// <summary>
		/// Gets the additional data present in the message.
		/// </summary>
		public string ExtraInfo { get; internal set; }
	}
}
