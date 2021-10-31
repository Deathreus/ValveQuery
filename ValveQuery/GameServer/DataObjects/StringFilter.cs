namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents a filter that filters by provided string.
	/// </summary>
	public class StringFilter : LogFilter
	{
		/// <summary>
		/// Filter string.
		/// </summary>
		public string String { get; set; }

		/// <summary>
		/// Creates a regex filter pattern based on the filter string.
		/// </summary>
		/// <returns>Regex filter pattern.</returns>
		public override string ToString() => ".*" + String + ".*";
	}
}
