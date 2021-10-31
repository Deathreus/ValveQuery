using System.Net;

namespace ValveQuery
{
	internal class ConnectionInfo
	{
		internal IPEndPoint EndPoint { get; set; }
		internal int SendTimeout { get; set; }
		internal int ReceiveTimeout { get; set; }
		internal int Retries { get; set; }
		internal bool ThrowExceptions { get; set; }
	}
}
