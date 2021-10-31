using System;
using System.Linq;
using System.Text;

namespace ValveQuery
{
	internal class Parser
	{
		private readonly byte[] Data;

		private int CurrentPosition = -1;

		private readonly int LastPosition;

		internal bool HasUnParsedBytes {
			get => CurrentPosition <= LastPosition;
		}

		internal Parser(byte[] data)
		{
			Data = data;
			CurrentPosition = -1;
			LastPosition = Data.Length - 1;
		}

		internal byte ReadByte()
		{
			CurrentPosition++;
			if (CurrentPosition > LastPosition)
			{
				throw new ParseException("Index was outside the bounds of the byte array.");
			}

			return Data[CurrentPosition];
		}

		internal ushort ReadUShort()
		{
			CurrentPosition++;
			if (CurrentPosition + 3 > LastPosition)
			{
				throw new ParseException("Unable to parse bytes to ushort.");
			}

			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(Data, CurrentPosition, 2);
			}

			ushort result = BitConverter.ToUInt16(Data, CurrentPosition);
			CurrentPosition++;
			return result;
		}

		internal int ReadInt()
		{
			CurrentPosition++;
			if (CurrentPosition + 3 > LastPosition)
			{
				throw new ParseException("Unable to parse bytes to int.");
			}

			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(Data, CurrentPosition, 4);
			}

			int result = BitConverter.ToInt32(Data, CurrentPosition);
			CurrentPosition += 3;
			return result;
		}

		internal ulong ReadULong()
		{
			CurrentPosition++;
			if (CurrentPosition + 7 > LastPosition)
			{
				throw new ParseException("Unable to parse bytes to ulong.");
			}

			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(Data, CurrentPosition, 8);
			}

			ulong result = BitConverter.ToUInt64(Data, CurrentPosition);
			CurrentPosition += 7;
			return result;
		}

		internal float ReadFloat()
		{
			CurrentPosition++;
			if (CurrentPosition + 3 > LastPosition)
			{
				throw new ParseException("Unable to parse bytes to float.");
			}

			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(Data, CurrentPosition, 4);
			}

			float result = BitConverter.ToSingle(Data, CurrentPosition);
			CurrentPosition += 3;
			return result;
		}

		internal string ReadString()
		{
			CurrentPosition++;
			int currentPosition = CurrentPosition;
			while (Data[CurrentPosition] != 0)
			{
				CurrentPosition++;
				if (CurrentPosition > LastPosition)
				{
					throw new ParseException("Unable to parse bytes to string.");
				}
			}

			string result = Encoding.UTF8.GetString(Data, currentPosition, CurrentPosition - currentPosition);
			return result;
		}

		internal void SkipBytes(byte count)
		{
			CurrentPosition += count;
			if (CurrentPosition > LastPosition)
			{
				throw new ParseException("skip count was outside the bounds of the byte array.");
			}
		}

		internal byte[] GetUnParsedBytes()
		{
			return Data.Skip(CurrentPosition + 1).ToArray();
		}
	}
}
