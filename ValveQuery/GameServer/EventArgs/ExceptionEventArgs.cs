using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Exception event.
	/// </summary>
	[Serializable]
	public class ExceptionEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets received log message.
		/// </summary>
		public string Message { get; internal set; }
	}
}
