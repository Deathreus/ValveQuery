using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for role selection event.
	/// </summary>
	[Serializable]
	public class RoleSelectionEventArgs : PlayerEventArgs
	{
		/// <summary>
		/// Gets the role name.
		/// </summary>
		public string Role { get; internal set; }
	}
}
