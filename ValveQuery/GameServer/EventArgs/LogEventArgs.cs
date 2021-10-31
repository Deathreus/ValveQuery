using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Serves as base class for all log  EventArgs.
	/// </summary>
	[Serializable]
	public class LogEventArgs : EventArgs
	{
		/// <summary>
		/// Gets Timestamp.
		/// </summary>
		public DateTime Timestamp { get; internal set; }
	}
}
