using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Encapsulates a method that has a parameter of type string which is the log message received from server.
	/// Invoked when a log message is received from server.
	/// </summary>
	/// <param name="log">Received log message.</param>
	public delegate void LogCallback(string log);
	/// <summary>
	/// Provides methods to listen to logs and to set up events on desired type of log message.
	/// </summary>
	public class Logs : QueryBase
	{
		private Socket UdpSocket;

		private IPEndPoint ServerEndPoint;

		private LogCallback Callback;

		private readonly int BufferSize = 1400;
		private readonly byte[] recvData;
		private readonly int Port;
		private readonly int HeaderSize = 7;

		private readonly List<LogEvents> EventsInstanceList = new List<LogEvents>();

		/// <summary>
		/// Gets a value that indicates whether its listening.
		/// </summary>
		public bool IsListening { get; private set; }

		internal Logs(int port, IPEndPoint serverEndPoint)
		{
			Port = port;
			ServerEndPoint = serverEndPoint;
			recvData = new byte[BufferSize];
		}

		/// <summary>
		/// Start listening to logs.
		/// </summary>
		public void Start()
		{
			ThrowIfDisposed();
			if (IsListening)
			{
				throw new Exception("QueryMaster already listening to logs.");
			}

			IsListening = true;

			UdpSocket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);
			UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			UdpSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			UdpSocket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, Recv, null);
		}

		/// <summary>
		/// Stop listening to logs.
		/// </summary>
		public void Stop()
		{
			ThrowIfDisposed();
			if (UdpSocket != null)
			{
				UdpSocket.Close();
			}
			IsListening = false;
		}

		/// <summary>
		/// Listen to logs sent by the server.
		/// </summary>
		/// <param name="callback">Called when a log message is received.</param>
		public void Listen(LogCallback callback)
		{
			ThrowIfDisposed();
			Callback = callback;
		}

		/// <summary>
		/// Returns an instance of <see cref="T:ValveQuery.GameServer.LogEvents" /> that provides event and filtering mechanism.
		/// </summary>
		/// <returns>Instance of <see cref="T:ValveQuery.GameServer.LogEvents" /> </returns>
		public LogEvents GetEventsInstance()
		{
			ThrowIfDisposed();
			var logEvents = new LogEvents(ServerEndPoint);
			EventsInstanceList.Add(logEvents);
			return logEvents;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}
			if (disposing)
			{
				if (UdpSocket != null)
				{
					UdpSocket.Close();
				}
				foreach (LogEvents eventsInstance in EventsInstanceList)
				{
					eventsInstance.Dispose();
				}
			}
			base.Dispose(disposing);
			IsDisposed = true;
		}

		private void Recv(IAsyncResult res)
		{
			int num;
			try
			{
				num = UdpSocket.EndReceive(res);
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			if (num > HeaderSize)
			{
				string log = Encoding.UTF8.GetString(recvData, HeaderSize, num - HeaderSize);
				if (Callback != null)
				{
					Callback(log);
				}
				foreach (LogEvents eventsInstance in EventsInstanceList)
				{
					eventsInstance.ProcessLog(log);
				}
			}
			UdpSocket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, Recv, null);
		}
	}
}
