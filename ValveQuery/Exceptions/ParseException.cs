using System;
using System.Runtime.Serialization;

namespace ValveQuery
{
	/// <summary>
	/// The exception that is thrown when there is an error while parsing received packets.
	/// </summary>
	[Serializable]
	public class ParseException : Exception
	{
		public ParseException()
			: base()
		{
		}

		public ParseException(string message)
			: base(message)
		{
		}

		public ParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
