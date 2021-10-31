using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for WorldAction event.
	/// </summary>
	[Serializable]
	public class WorldActionEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets the name of the action performed.
		/// </summary>
		public string Action { get; internal set; }
	}
}
