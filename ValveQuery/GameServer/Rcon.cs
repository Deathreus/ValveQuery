using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace ValveQuery.GameServer
{
	/// <summary>
	/// Provides methods to access server using rcon password.
	/// </summary>
	public class Rcon : QueryBase
	{
		internal TcpQuery socket;
		private readonly ConnectionInfo ConInfo;

		internal Rcon(ConnectionInfo conInfo)
		{
			ConInfo = conInfo;
		}

		internal static Rcon Authorize(ConnectionInfo conInfo, string msg)
		{
			return new QueryBase().Invoke(() => InternalAuthorize(conInfo, msg), conInfo.Retries + 1, null, conInfo.ThrowExceptions);
		}

		private static Rcon InternalAuthorize(ConnectionInfo conInfo, string msg)
		{
			var rconSource = new Rcon(conInfo)
			{
				socket = new TcpQuery(conInfo)
			};

			var packet = new RconSrcPacket
			{
				Body = msg,
				Id = (int)PacketId.ExecCmd,
				Type = (int)PacketType.Auth
			};
			byte[] response = rconSource.socket.GetResponse(RconUtil.GetBytes(packet));

			try
			{
				if (BitConverter.ToInt32(response, 4) == -1)
					throw new InvalidPacketException("Not authorized.");
				
				return rconSource;
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", (response == null) ? new byte[1] : response);
				throw;
			}
		}

		internal string InternalSendCommand(string command, bool isMultipacketResponse)
		{
			var rconPacket = new RconSrcPacket
			{
				Body = command,
				Id = (int)PacketId.ExecCmd,
				Type = (int)PacketType.Exec
			};
			List<byte[]> multiPacketResponse = socket.GetMultiPacketResponse(RconUtil.GetBytes(rconPacket));
			var stringBuilder = new StringBuilder();

			try
			{
				for (int i = 0; i < multiPacketResponse.Count; i++)
				{
					if (BitConverter.ToInt32(multiPacketResponse[i], 4) != (int)PacketId.Empty)
					{
						if (multiPacketResponse[i].Length - BitConverter.ToInt32(multiPacketResponse[i], 0) == 4)
						{
							stringBuilder.Append(RconUtil.ProcessPacket(multiPacketResponse[i]).Body);
						}
						else
						{
							stringBuilder.Append(RconUtil.ProcessPacket(multiPacketResponse[i]).Body + Util.BytesToString(multiPacketResponse[++i].Take(multiPacketResponse[i].Length - 2).ToArray()));
						}
					}
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", multiPacketResponse.SelectMany((byte[] x) => x).ToArray());
				throw;
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Enable logging on server.
		/// </summary>
		public virtual void Enablelogging()
		{
			ThrowIfDisposed();
			SendCommand("log on");
		}

		/// <summary>
		/// Disable logging on server.
		/// </summary>
		public virtual void Disablelogging()
		{
			ThrowIfDisposed();
			SendCommand("log off");
		}

		/// <summary>
		/// Send a Command to server.
		/// </summary>
		/// <param name="cmd">Server command.</param>
		/// <param name="isMultiPacketResponse">Whether the reply could be/is larger than 1400 bytes.</param>
		/// <returns>Reply from server in string format.</returns>
		public virtual string SendCommand(string cmd, bool isMultiPacketResponse = false)
		{
			ThrowIfDisposed();
			return Invoke(() => InternalSendCommand(cmd, isMultiPacketResponse), 1, null, ConInfo.ThrowExceptions);
		}

		/// <summary>
		/// Add a client socket to server's logaddress list.
		/// </summary>
		/// <param name="ip">IP-Address of client.</param>
		/// <param name="port">Port number of client.</param>
		public virtual void AddlogAddress(string ip, ushort port)
		{
			ThrowIfDisposed();
			SendCommand("logaddress_add " + ip + ":" + port);
		}

		/// <summary>
		/// Delete a client socket from server's logaddress list.
		/// </summary>
		/// <param name="ip">IP-Address of client.</param>
		/// <param name="port">Port number of client.</param>
		public virtual void RemovelogAddress(string ip, ushort port)
		{
			ThrowIfDisposed();
			SendCommand("logaddress_del " + ip + ":" + port);
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing && socket != null)
			{
				socket.Dispose();
			}

			base.Dispose(disposing);
			IsDisposed = true;
		}
	}
}
