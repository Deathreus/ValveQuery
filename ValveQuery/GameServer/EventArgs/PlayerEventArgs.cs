using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Playervalidate,playerenteredgame and player disconnected event.
	/// </summary>
	[Serializable]
	public class PlayerEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Player information.
		/// </summary>
		public LogPlayerInfo Player { get; internal set; }
	}
}
