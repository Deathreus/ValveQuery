using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for server say event.
	/// </summary>
	[Serializable]
	public class ServerSayEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets the message said by server.
		/// </summary>
		public string Message { get; internal set; }
	}
}
