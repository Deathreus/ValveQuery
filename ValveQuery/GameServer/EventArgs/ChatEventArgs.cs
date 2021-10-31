using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Say and TeamSay events.
	/// </summary>
	[Serializable]
	public class ChatEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the message said by player.
		/// </summary>
		public string Message { get; internal set; }
	}
}
