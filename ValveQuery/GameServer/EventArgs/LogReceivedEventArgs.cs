using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for log received event.
	/// </summary>
	[Serializable]
	public class LogReceivedEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets received log message.
		/// </summary>
		public string Message { get; internal set; }
	}
}
