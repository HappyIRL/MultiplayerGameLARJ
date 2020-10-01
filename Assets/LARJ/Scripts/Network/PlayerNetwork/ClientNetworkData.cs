using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkData
{
	public byte ID;
	public Vector3 Position;
	public Vector3 Rotation;

	public static byte[] SerializeMethod(object customObject)
	{
		ClientNetworkData data = (ClientNetworkData)customObject;
		byte[] result = new byte[25];
		
		result[0] = data.ID;
		data.Vector3ToByte(data.Position).CopyTo(result, 1);
		data.Vector3ToByte(data.Rotation).CopyTo(result, 13);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		ClientNetworkData data = new ClientNetworkData();
		
		data.ID = input[0];
		data.Position = data.ByteToVector3(data.SubArray(input, 1, 12));
		data.Rotation = data.ByteToVector3(data.SubArray(input, 13, 12));

		return data;
	}

	private byte[] SubArray(byte[] array, int startIndex, int cutLength)
	{
		List<byte> newArray = new List<byte>();
		
		if(startIndex + cutLength > array.Length)
		{
			Debug.LogError($"Index would go of bounds, where: {this}. StartIndex: {startIndex} + CutLength: {cutLength} > Array: {array.Length}");
			return null;
		}

		for(int i = startIndex; i < startIndex + cutLength; i++)
		{
			newArray.Add(array[i]);
		}

		return newArray.ToArray();
	}

	private byte[] Vector3ToByte(Vector3 v)
	{
		byte[] b = new byte[12];

		BitConverter.GetBytes(v.x).CopyTo(b, 0);
		BitConverter.GetBytes(v.y).CopyTo(b, 4);
		BitConverter.GetBytes(v.z).CopyTo(b, 8);

		return b;
	}

	private Vector3 ByteToVector3(byte[] b)
	{
		float x = BitConverter.ToSingle(b, 0);
		float y = BitConverter.ToSingle(b, 4);
		float z = BitConverter.ToSingle(b, 8);

		return new Vector3(x, y, z);
	}
}
