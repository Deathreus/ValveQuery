using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ValveQuery
{
	/// <summary>
	/// References a method to be called when an attempt to perform some action is made.
	/// </summary>
	/// <param name="attempt"></param>
	public delegate void AttemptCallback(int attempt);
	/// <summary>
	/// References a method to be called when an exception occurs.
	/// </summary>
	/// <param name="ex">Thrown exception.</param>
	public delegate void ErrorCallback(Exception ex);

	internal static class Util
	{
		internal static string BytesToString(byte[] bytes) => Encoding.UTF8.GetString(bytes);
		internal static string BytesToString(byte[] bytes, int index, int count) => Encoding.UTF8.GetString(bytes, index, count);
		internal static byte[] StringToBytes(string str) => Encoding.UTF8.GetBytes(str);

		internal static byte[] MergeByteArrays(byte[] array1, byte[] array2)
		{
			byte[] newArray = new byte[array1.Length + array2.Length];
			Buffer.BlockCopy(array1, 0, newArray, 0, array1.Length);
			Buffer.BlockCopy(array2, 0, newArray, array1.Length, array2.Length);
			return newArray;
		}
		internal static byte[] MergeByteArrays(byte[] array1, byte[] array2, byte[] array3)
		{
			byte[] newArray = new byte[array1.Length + array2.Length + array3.Length];
			Buffer.BlockCopy(array1, 0, newArray, 0, array1.Length);
			Buffer.BlockCopy(array2, 0, newArray, array1.Length, array2.Length);
			Buffer.BlockCopy(array3, 0, newArray, array1.Length + array2.Length, array3.Length);
			return newArray;
		}

		internal static IPEndPoint ToIPEndPoint(string endPointStr)
		{
			IPEndPoint iPEndPoint = null;
			string[] endpoints = endPointStr.Split(':');
			if (endpoints.Length == 2 && IPAddress.TryParse(endpoints[0], out IPAddress address) && Int32.TryParse(endpoints[1], out int port))
			{
				iPEndPoint = new IPEndPoint(address, port);
			}

			return iPEndPoint;
		}
	}
}
