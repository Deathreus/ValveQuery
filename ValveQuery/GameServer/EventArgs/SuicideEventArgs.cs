using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for suicide event.
	/// </summary>
	[Serializable]
	public class SuicideEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the weapon name.
		/// </summary>
		public string Weapon { get; internal set; }
	}
}
