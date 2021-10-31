using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for PlayerScoreReport event.
	/// </summary>
	[Serializable]
	public class PlayerScoreReportEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets player score.
		/// </summary>
		public string Score { get; internal set; }

		/// <summary>
		/// Gets the additional data present in the message.
		/// </summary>
		public string ExtraInfo { get; internal set; }
	}
}
