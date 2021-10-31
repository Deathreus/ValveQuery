namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents a filter that filters by provided regex filter pattern.
	/// </summary>
	public class RegexFilter : LogFilter
	{
		/// <summary>
		/// Regex pattern.
		/// </summary>
		public string RegexPattern { get; set; }

		/// <summary>
		/// Regex filter pattern.
		/// </summary>
		/// <returns>Regex filter pattern.</returns>
		public override string ToString() => RegexPattern;
	}
}
