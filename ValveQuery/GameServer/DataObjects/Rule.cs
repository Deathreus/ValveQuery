using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Contains information of a server rule.
	/// </summary>
	[Serializable]
	public class Rule : DataObject
	{
		/// <summary>
		/// Name of the rule. 
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// Value of the rule. 
		/// </summary>
		public string Value { get; internal set; }
	}
}
