using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides data for player injured event.
	/// </summary>
	[Serializable]
	public class InjureEventArgs : KillEventArgs
	{
		/// <summary>
		/// Gets damage.
		/// </summary>
		public string Damage { get; internal set; }
	}
}
