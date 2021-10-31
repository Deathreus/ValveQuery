using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Contains information on SourceTV.
	/// </summary>
	[Serializable]
	public class SourceTVInfo : DataObject
	{
		/// <summary>
		/// Spectator port number for SourceTV.
		/// </summary>
		public ushort Port { get; internal set; }

		/// <summary>
		/// Name of the spectator server for SourceTV.
		/// </summary>
		public string Name { get; internal set; }
	}
}
