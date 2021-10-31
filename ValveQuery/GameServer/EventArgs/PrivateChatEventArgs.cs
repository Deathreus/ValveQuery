using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for PrivateChat event.
	/// </summary>
	[Serializable]
	public class PrivateChatEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Sender Player's info.
		/// </summary>
		public LogPlayerInfo Sender { get; internal set; }

		/// <summary>
		/// Gets Receiver Player's info.
		/// </summary>
		public LogPlayerInfo Receiver { get; internal set; }

		/// <summary>
		/// Get the message sent by sender.
		/// </summary>
		public string Message { get; internal set; }
	}
}
