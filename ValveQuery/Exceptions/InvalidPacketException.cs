using System;
using System.Runtime.Serialization;

namespace ValveQuery
{
	/// <summary>
	/// The exception that is thrown when an invalid packet is received.
	/// </summary>
	[Serializable]
	public class InvalidPacketException : Exception
	{
		public InvalidPacketException()
			: base()
		{
		}

		public InvalidPacketException(string message)
			: base(message)
		{
		}

		public InvalidPacketException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidPacketException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
