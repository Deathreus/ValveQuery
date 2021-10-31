using System;
using System.Text;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Represents a filter that filters by player.
	/// </summary>
	public class PlayerFilter : LogFilter
	{
		/// <summary>
		/// Name of the player.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// User id of the player.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// SteamId of the player.
		/// </summary>
		public string SteamId { get; set; }

		/// <summary>
		/// Team in which the player is in.
		/// </summary>
		public string Team { get; set; }

		/// <summary>
		/// Creates a regex filter pattern based on name,userid,steamid and team. 
		/// </summary>
		/// <returns>Regex filter pattern.</returns>
		public override string ToString()
		{
			if (String.IsNullOrEmpty(FilterString))
			{
				var stringBuilder = new StringBuilder("^.*");
				if (!String.IsNullOrEmpty(Name))
				{
					stringBuilder.Append(Name);
				}
				stringBuilder.Append("<");
				if (String.IsNullOrEmpty(UserId))
				{
					stringBuilder.Append("\\d+");
				}
				else
				{
					stringBuilder.Append(UserId);
				}
				stringBuilder.Append("><");
				if (String.IsNullOrEmpty(SteamId))
				{
					stringBuilder.Append(".+");
				}
				else
				{
					stringBuilder.Append(SteamId);
				}
				stringBuilder.Append("><");
				if (String.IsNullOrEmpty(Team))
				{
					stringBuilder.Append(".*");
				}
				else
				{
					stringBuilder.Append(Team);
				}
				stringBuilder.Append(">.*$");
				FilterString = stringBuilder.ToString();
			}
			return FilterString;
		}
	}
}
