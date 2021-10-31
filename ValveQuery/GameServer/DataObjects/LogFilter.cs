using System.Text.RegularExpressions;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents log filter.
	/// </summary>
	public abstract class LogFilter
	{
		/// <summary>
		/// Regex instance.
		/// </summary>
		protected internal Regex RegexInstance { get; set; }

		/// <summary>
		/// used to store the regex pattern.
		/// </summary>
		protected internal string FilterString { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the filter is enabled.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets <see cref="T:ValveQuery.GameServer.LogFilterAction" />
		/// </summary>
		public LogFilterAction Action { get; set; }

		internal LogFilter()
		{
			Enabled = true;
			Action = LogFilterAction.Allow;
		}
	}
}
