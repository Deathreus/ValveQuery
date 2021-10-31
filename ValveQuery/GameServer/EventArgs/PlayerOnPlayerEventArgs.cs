using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for PlayerOnPLayerTriggered event.
	/// </summary>
	[Serializable]
	public class PlayerOnPlayerEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets info about the player who triggered an action.
		/// </summary>
		public LogPlayerInfo Source { get; internal set; }

		/// <summary>
		/// Gets info about the player on whom the ation was triggered.
		/// </summary>
		public LogPlayerInfo Target { get; internal set; }

		/// <summary>
		/// Gets the name of the  action performed.
		/// </summary>
		public string Action { get; internal set; }
	}
}
