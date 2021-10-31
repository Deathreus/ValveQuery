using System;
using System.Runtime.Serialization;

namespace ValveQuery
{
	/// <summary>
	/// The exception that is thrown when an invalid message header is received.
	/// </summary>
	[Serializable]
	public class InvalidHeaderException : Exception
	{
		public InvalidHeaderException()
		{
		}

		public InvalidHeaderException(string message)
			: base(message)
		{
		}

		public InvalidHeaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidHeaderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
