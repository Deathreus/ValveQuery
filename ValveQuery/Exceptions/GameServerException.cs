using System;
using System.Runtime.Serialization;

namespace ValveQuery
{
	/// <summary>
	/// Base for all GameServer Exception.
	/// </summary>
	[Serializable]
	public class GameServerException : Exception
	{
		public GameServerException()
			: base()
		{
		}

		public GameServerException(string message)
			: base(message)
		{
		}

		public GameServerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected GameServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
