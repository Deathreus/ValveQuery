using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ValveQuery.GameServer
{
	internal class TcpQuery : ServerSocket
	{
		private readonly byte[] EmptyPkt = new byte[] { 0x0a, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

		internal TcpQuery(ConnectionInfo conInfo)
			: base(conInfo, ProtocolType.Tcp)
		{
		}

		internal byte[] GetResponse(byte[] msg)
		{
			SendData(msg);
			ReceiveData();//Response value packet

			return ReceiveData();//Auth response packet
		}

		internal List<byte[]> GetMultiPacketResponse(byte[] msg)
		{
			var recvBytes = new List<byte[]>();
			bool isRemaining = true;
			byte[] recvData;
			SendData(msg);
			SendData(EmptyPkt);//Empty packet
			recvData = ReceiveData();//reply
			recvBytes.Add(recvData);

			do
			{
				recvData = ReceiveData();//may or may not be an empty packet
				if (BitConverter.ToInt32(recvData, 4) == (int)PacketId.Empty)
				{
					isRemaining = false;
				}
				else
				{
					recvBytes.Add(recvData);
				}
			} while (isRemaining);

			return recvBytes;
		}
	}
}
