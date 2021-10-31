using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for playerkicked event.
	/// </summary>
	[Serializable]
	public class KickEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the name of the admin who kicked the player.
		/// </summary>
		public string Kicker { get; internal set; }

		/// <summary>
		/// Gets the message sent as a reason for the kick.
		/// </summary>
		public string Message { get; internal set; }
	}
}
