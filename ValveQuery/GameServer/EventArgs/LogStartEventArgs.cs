using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for log start event.
	/// </summary>
	[Serializable]
	public class LogStartEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Filename.
		/// </summary>
		public string FileName { get; internal set; }

		/// <summary>
		/// Gets Game name.
		/// </summary>
		public string Game { get; internal set; }

		/// <summary>
		/// Gets Protocol version.
		/// </summary>
		public string Protocol { get; internal set; }

		/// <summary>
		/// Gets Release version.
		/// </summary>
		public string Release { get; internal set; }

		/// <summary>
		/// Gets Build version.
		/// </summary>
		public string Build { get; internal set; }
	}
}
