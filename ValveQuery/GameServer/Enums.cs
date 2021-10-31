namespace ValveQuery.GameServer
{
    /// <summary>
	/// Specifies the type of action filter should perform.
	/// </summary>
	public enum LogFilterAction
	{
		/// <summary>
		/// Allow.
		/// </summary>
		Allow,
		/// <summary>
		/// Block.
		/// </summary>
		Block
	}

    /// <summary>
	/// Game Server's type.
	/// </summary>
	public enum GameServerType
	{
		/// <summary>
		/// Server returned an invalid value.
		/// </summary>
		Invalid,
		/// <summary>
		/// Dedicated.
		/// </summary>
		Dedicated,
		/// <summary>
		/// Non Dedicated.
		/// </summary>
		NonDedicated,
		/// <summary>
		/// Listen.
		/// </summary>
		Listen,
		/// <summary>
		/// Source TV.
		/// </summary>
		SourceTV,
		/// <summary>
		/// HLTV Server
		/// </summary>
		HLTVServer
	}

    /// <summary>
	/// Server's operating system.
	/// </summary>
	public enum GameEnvironment
	{
		/// <summary>
		/// Server returned an invalid value.
		/// </summary>
		Invalid,
		/// <summary>
		/// Linux.
		/// </summary>
		Linux,
		/// <summary>
		/// Windows.
		/// </summary>
		Windows,
		/// <summary>
		/// Mac.
		/// </summary>
		Mac
	}
}