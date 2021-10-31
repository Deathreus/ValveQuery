using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for Server cvar event. 
	/// </summary>
	[Serializable]
	public class CvarEventArgs : LogEventArgs
	{
		/// <summary>
		/// Gets Cvar name.
		/// </summary>
		public string Cvar { get; internal set; }

		/// <summary>
		/// Gets Cvar Value.
		/// </summary>
		public string Value { get; internal set; }
	}
}
