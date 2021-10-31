using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for map loaded event.
	/// </summary>
	[Serializable]
	public class MapLoadEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Map name.
		/// </summary>
		public string MapName { get; internal set; }
	}
}
