using System;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Contains information about the Mod.
	/// </summary>
	/// <remarks>Present only in Obsolete server responses.</remarks>
	[Serializable]
	public class ModInfo : DataObject
	{
		/// <summary>
		/// URL to mod website. 
		/// </summary>
		public string Link { get; internal set; }

		/// <summary>
		/// URL to download the mod. 
		/// </summary>
		public string DownloadLink { get; internal set; }

		/// <summary>
		/// Version of mod installed on server. 
		/// </summary>
		public long Version { get; internal set; }

		/// <summary>
		/// Space (in bytes) the mod takes up. 
		/// </summary>
		public long Size { get; internal set; }

		/// <summary>
		/// Indicates the type of mod.
		/// </summary>
		public bool IsOnlyMultiPlayer { get; internal set; }

		/// <summary>
		/// Indicates whether mod uses its own DLL.
		/// </summary>
		public bool IsHalfLifeDll { get; internal set; }
	}
}
