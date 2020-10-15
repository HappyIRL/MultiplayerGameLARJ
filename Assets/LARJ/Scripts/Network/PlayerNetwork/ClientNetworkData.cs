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
		LARJMath.Vector3ToByte(data.Position).CopyTo(result, 1);
		LARJMath.Vector3ToByte(data.Rotation).CopyTo(result, 13);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		ClientNetworkData data = new ClientNetworkData();
		
		data.ID = input[0];
		data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 1, 12));
		data.Rotation = LARJMath.ByteToVector3(LARJMath.SubArray(input, 13, 12));

		return data;
	}
}
