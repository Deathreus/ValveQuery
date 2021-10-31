using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using Ionic.BZip2;
using Ionic.Crc;

namespace ValveQuery.GameServer
{
	internal class UdpQuery : ServerSocket
	{
		private const int SinglePacket = -1;
		private const int MultiPacket = -2;

		internal UdpQuery(ConnectionInfo conInfo)
			: base(conInfo, ProtocolType.Udp)
		{
		}

		internal byte[] GetResponse(byte[] msg, bool isMultiPacket = false)
		{
			int header;

			SendData(msg);
			byte[] recvData = ReceiveData();
			if (isMultiPacket)
			{
				var data = new List<byte>();
				data.AddRange(recvData);
				while (true)
				{
					try
					{
						recvData = ReceiveData();
						data.AddRange(recvData);
					}
					catch (SocketException ex)
					{
						if (ex.SocketErrorCode == SocketError.TimedOut)
						{
							recvData = data.ToArray();
							break;
						}
						else
						{
							Dispose();
							throw;
						}
					}
				}
			}

			try
			{
				header = BitConverter.ToInt32(recvData, 0);
				return header switch
				{
					SinglePacket => ParseSinglePkt(recvData),
					MultiPacket => ParseMultiPkt(recvData),
					_ => throw new InvalidHeaderException("Protocol header is not valid")
				};
			}
			catch (Exception e)
			{
				e.Data.Add("ReceivedData", recvData == null ? new byte[1] : recvData);
				Dispose();
				throw;
			}
		}

		private byte[] ParseSinglePkt(byte[] data)
			=> data.Skip(4).ToArray();

		private byte[] ParseMultiPkt(byte[] data)
		{
			bool isCompressed = false;
			int checksum = 0;
			byte[] recvData = null;
			List<byte> recvList = null;
			Parser parser = null;

			byte pktCount = data[8];

			var pktList = new List<KeyValuePair<byte, byte[]>>(pktCount) {
				new KeyValuePair<byte, byte[]>(data[9], data)
			};

			for (int i = 1; i < pktCount; i++)
			{
				recvData = ReceiveData();
				pktList.Add(new KeyValuePair<byte, byte[]>(recvData[9], recvData));
			}

			pktList.Sort((x, y) => x.Key.CompareTo(y.Key));
			recvList = new List<byte>();
			parser = new Parser(pktList[0].Value);
			parser.SkipBytes(4);//header
			if (parser.ReadInt() < 0)//ID
			{
				isCompressed = true;
			}

			parser.ReadByte();//total
			int pktId = parser.ReadByte();// packet id
			parser.ReadUShort();//size
			if (isCompressed)
			{
				parser.SkipBytes(2);//decompressed size of data
				checksum = parser.ReadInt();//Checksum
			}
			recvList.AddRange(parser.GetUnParsedBytes());

			for (int i = 1; i < pktList.Count; i++)
			{
				parser = new Parser(pktList[i].Value);
				parser.SkipBytes(12);//multipacket header 
				recvList.AddRange(parser.GetUnParsedBytes());
			}

			recvData = recvList.ToArray();
			if (isCompressed)
			{
				recvData = Decompress(recvData);
				if (!IsValid(recvData, checksum))
				{
					throw new InvalidPacketException("packet's checksum value does not match with the calculated checksum");
				}
			}

			return recvData.Skip(4).ToArray();
		}

		private byte[] Decompress(byte[] data)
		{
			using (var input = new MemoryStream(data))
			using (var output = new MemoryStream())
			using (var unZip = new BZip2InputStream(input))
			{
				int ch = unZip.ReadByte();

				while (ch != -1)
				{
					output.WriteByte((byte)ch);
					ch = unZip.ReadByte();
				}

				output.Flush();

				return output.ToArray();
			}
		}

		private bool IsValid(byte[] data, int Checksum)
		{
			using (var memoryStream = new MemoryStream(data))
			{
				return Checksum == new CRC32().GetCrc32(memoryStream);
			}
		}
	}
}
