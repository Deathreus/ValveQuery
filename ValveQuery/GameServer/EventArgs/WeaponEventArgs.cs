using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for WeaponSelect and WeaponAcquired event.
	/// </summary>
	[Serializable]
	public class WeaponEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets name of weapon.
		/// </summary>
		public string Weapon { get; internal set; }
	}
}
