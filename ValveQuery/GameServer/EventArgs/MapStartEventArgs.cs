using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for map started event.
	/// </summary>
	[Serializable]
	public class MapStartEventArgs : MapLoadEventArgs
	{
		/// <summary>
		/// Get map CRC value.
		/// </summary>
		public string MapCRC { get; internal set; }
	}
}
