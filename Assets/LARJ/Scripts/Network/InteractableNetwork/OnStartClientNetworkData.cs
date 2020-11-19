using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartClientNetworkData
{
	public Vector3 Position;
	public Vector3 Rotation;
	public Vector3 LocalScale;
	public byte ObjectIndex;
	//public int Seed;

	public static byte[] SerializeMethod(object customObject)
	{
		OnStartClientNetworkData data = (OnStartClientNetworkData)customObject;
		byte[] result = new byte[37];

		LARJMath.Vector3ToByte(data.Position).CopyTo(result, 0);
		LARJMath.Vector3ToByte(data.Rotation).CopyTo(result, 12);
		LARJMath.Vector3ToByte(data.LocalScale).CopyTo(result, 24);
		//BitConverter.GetBytes(data.Seed).CopyTo(result, 36);
		result[36] = data.ObjectIndex;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		OnStartClientNetworkData data = new OnStartClientNetworkData();

		data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 0, 12));
		data.Rotation = LARJMath.ByteToVector3(LARJMath.SubArray(input, 12, 12));
		data.LocalScale = LARJMath.ByteToVector3(LARJMath.SubArray(input, 24, 12));
		//data.Seed = BitConverter.ToInt32(input, 36);
		data.ObjectIndex = input[36];

		return data;
	}
}
