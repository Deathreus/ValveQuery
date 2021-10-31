using System;
using System.Net;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides methods to create Server instance.
	/// </summary>
	public static class ServerQuery
	{
		/// <summary>
		/// Returns an object that represents the server
		/// </summary>
		/// <param name="ip">IP-Address of server.</param>
		/// <param name="port">Port number of server.</param>
		/// <param name="isObsolete">Obsolete Gold Source servers reply only to half life protocol.if set to true then it would use half life protocol.If set to null,then protocol is identified at runtime[Default : false].</param>
		/// <param name="sendTimeout">Sets Socket's SendTimeout Property.</param>
		/// <param name="receiveTimeout">Sets Socket's ReceiveTimeout.</param>
		/// <param name="retries">Number of times to retry if first attempt fails.</param>
		/// <param name="throwExceptions">Whether to throw any exceptions.</param>
		/// <returns>Instance of server class that represents the connected server.</returns>
		public static Server GetServerInstance(string ip, ushort port, bool? isObsolete = false, int sendTimeout = 3000, int receiveTimeout = 3000, int retries = 3, bool throwExceptions = false)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			return GetServerInstance(endPoint, isObsolete, sendTimeout, receiveTimeout, retries, throwExceptions);
		}

		/// <summary>
		/// Returns an object that represents the server.
		/// </summary>
		/// <param name="endPoint">Socket address of server.</param>
		/// <param name="isObsolete">Obsolete Gold Source servers reply only to half life protocol.if set to true then it would use half life protocol.If set to null,then protocol is identified at runtime.</param>
		/// <param name="sendTimeout">Sets Socket's SendTimeout Property.</param>
		/// <param name="receiveTimeout">Sets Socket's ReceiveTimeout.</param>
		/// <param name="retries">Number of times to retry if first attempt fails.</param>
		/// <param name="throwExceptions">Whether to throw any exceptions.</param>
		/// <returns>Instance of server class that represents the connected server</returns>
		public static Server GetServerInstance(IPEndPoint endPoint, bool? isObsolete = false, int sendTimeout = 3000, int receiveTimeout = 3000, int retries = 3, bool throwExceptions = false)
		{
			var conInfo = new ConnectionInfo
			{
				SendTimeout = sendTimeout,
				ReceiveTimeout = receiveTimeout,
				EndPoint = endPoint,
				Retries = retries,
				ThrowExceptions = throwExceptions
			};
			return new Server(conInfo);
		}
	}
}
