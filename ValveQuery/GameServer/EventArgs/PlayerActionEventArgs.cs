using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Player action event.
	/// </summary>
	[Serializable]
	public class PlayerActionEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the name of the action performed.
		/// </summary>
		public string Action { get; internal set; }

		/// <summary>
		/// Gets additional data present in the message.
		/// </summary>
		public string ExtraInfo { get; internal set; }
	}
}
