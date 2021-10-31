using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for servername event.
	/// </summary>
	[Serializable]
	public class ServerNameEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets name of server.
		/// </summary>
		public string Name { get; internal set; }
	}
}
