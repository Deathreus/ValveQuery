using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ValveQuery.GameServer
{
	internal class ServerSocket : QueryBase
	{
		internal static readonly int UdpBufferSize = 1400;
		internal static readonly int TcpBufferSize = 4110;
		internal IPEndPoint Address;
		protected internal int BufferSize;
		private readonly object LockObj = new object();

		internal Socket Socket { get; set; }

		internal ServerSocket(ConnectionInfo conInfo, ProtocolType type)
		{
			switch (type)
			{
				case ProtocolType.Tcp:
					Socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
					BufferSize = TcpBufferSize;
					break;
				case ProtocolType.Udp:
					Socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);
					BufferSize = UdpBufferSize;
					break;
				default:
					throw new ArgumentException("An invalid SocketType was specified.");
			}

			Socket.SendTimeout = conInfo.SendTimeout;
			Socket.ReceiveTimeout = conInfo.ReceiveTimeout;
			Address = conInfo.EndPoint;

			if (!Socket.BeginConnect(Address, null, null).AsyncWaitHandle.WaitOne(conInfo.ReceiveTimeout, true))
			{
				throw new SocketException(10060);
			}

			IsDisposed = false;
		}

		internal int SendData(byte[] data)
		{
			ThrowIfDisposed();

			lock (LockObj)
			{
				return Socket.Send(data);
			}
		}

		internal byte[] ReceiveData()
		{
			ThrowIfDisposed();

			byte[] array = new byte[BufferSize];
			int count = 0;
			lock (LockObj)
			{
				count = Socket.Receive(array);
			}

			return array.Take(count).ToArray();
		}

		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				lock (LockObj)
				{
					if (Socket != null)
					{
						Socket.Close();
					}
				}
			}

			base.Dispose(disposing);
			IsDisposed = true;
		}
	}
}
