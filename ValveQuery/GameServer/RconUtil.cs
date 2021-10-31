using System;
using System.Collections.Generic;

namespace ValveQuery.GameServer
{
	internal class RconSrcPacket
	{
		internal int Size { get; set; }
		internal int Id { get; set; }
		internal int Type { get; set; }
		internal string Body { get; set; }
	}
	internal static class RconUtil
	{
		internal static byte[] GetBytes(RconSrcPacket packet)
		{
			byte[] command = Util.StringToBytes(packet.Body);
			packet.Size = 10 + command.Length;

			var list = new List<byte>(packet.Size + 4);
			list.AddRange(BitConverter.GetBytes(packet.Size));
			list.AddRange(BitConverter.GetBytes(packet.Id));
			list.AddRange(BitConverter.GetBytes(packet.Type));

			list.AddRange(command);

			list.Add(0x00);
			list.Add(0x00);

			return list.ToArray();
		}

		internal static RconSrcPacket ProcessPacket(byte[] data)
		{
			try
			{
				var parser = new Parser(data);
				var rconSrcPacket = new RconSrcPacket()
				{
					Size = parser.ReadInt(),
					Id = parser.ReadInt(),
					Type = parser.ReadInt()
				};

				byte[] unParsedBytes = parser.GetUnParsedBytes();
				if (unParsedBytes.Length != 2)
				{
					rconSrcPacket.Body = Util.BytesToString(unParsedBytes, 0, unParsedBytes.Length - 3);
					return rconSrcPacket;
				}

				rconSrcPacket.Body = String.Empty;
				return rconSrcPacket;
			}
			catch (Exception ex)
			{
				ex.Data.Add("ReceivedData", data ?? (new byte[1]));
				throw;
			}
		}
	}
}
