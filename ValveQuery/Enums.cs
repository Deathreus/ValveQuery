using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveQuery
{
	/// <summary>
	/// Region codes
	/// </summary>
	public enum Region : byte
	{
		/// <summary>
		/// US East coast 
		/// </summary>
		US_East = 0,
		/// <summary>
		/// US West coast 
		/// </summary>
		US_West = 1,
		/// <summary>
		/// South America
		/// </summary>
		South_America = 2,
		/// <summary>
		/// Europe
		/// </summary>
		Europe = 3,
		/// <summary>
		/// Asia
		/// </summary>
		Asia = 4,
		/// <summary>
		/// Australia
		/// </summary>
		Australia = 5,
		/// <summary>
		/// Middle East 
		/// </summary>
		Middle_East = 6,
		/// <summary>
		/// Africa
		/// </summary>
		Africa = 7,
		/// <summary>
		/// Rest of the world 
		/// </summary>
		World = byte.MaxValue
	}

	internal enum SocketType
	{
		UDP,
		TCP
	}

	internal enum PacketId
	{
		Empty = 10,
		ExecCmd
	}

	internal enum PacketType
	{
		Auth = 3,
		AuthResponse = 2,
		Exec = 2,
		ExecResponse = 0
	}

	internal enum ResponseMsgHeader : byte
	{
		A2S_INFO = 0x49,
		A2S_PLAYER = 0x44,
		A2S_RULES = 0x45,
		A2S_SERVERQUERY_GETCHALLENGE = 0x41
	}
}
