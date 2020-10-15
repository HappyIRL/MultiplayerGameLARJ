using System;
using System.Collections.Generic;
using UnityEngine;

public static class LARJMath
{
	public static byte[] SubArray(byte[] array, int startIndex, int cutLength)
	{
		List<byte> newArray = new List<byte>();

		for (int i = startIndex; i < startIndex + cutLength; i++)
		{
			newArray.Add(array[i]);
		}

		return newArray.ToArray();
	}

	public static byte[] Vector3ToByte(Vector3 v)
	{
		byte[] b = new byte[12];

		BitConverter.GetBytes(v.x).CopyTo(b, 0);
		BitConverter.GetBytes(v.y).CopyTo(b, 4);
		BitConverter.GetBytes(v.z).CopyTo(b, 8);

		return b;
	}

	public static Vector3 ByteToVector3(byte[] b)
	{
		float x = BitConverter.ToSingle(b, 0);
		float y = BitConverter.ToSingle(b, 4);
		float z = BitConverter.ToSingle(b, 8);

		return new Vector3(x, y, z);
	}

	public static byte[] QuaternionToByte(Quaternion q)
	{
		byte[] b = new byte[16];

		BitConverter.GetBytes(q.x).CopyTo(b, 0);
		BitConverter.GetBytes(q.y).CopyTo(b, 4);
		BitConverter.GetBytes(q.z).CopyTo(b, 8);
		BitConverter.GetBytes(q.w).CopyTo(b, 12);

		return b;
	}

	public static Quaternion ByteToQuaternion(byte[] b)
	{
		float x = BitConverter.ToSingle(b, 0);
		float y = BitConverter.ToSingle(b, 4);
		float z = BitConverter.ToSingle(b, 8);
		float w = BitConverter.ToSingle(b, 12);

		return new Quaternion(x, y, z, w);
	}
}
