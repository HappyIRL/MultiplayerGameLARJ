using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTransformNetworkData
{
	public Vector3 Position;
	public Vector3 Rotation;
	public Vector3 LocalScale;
	public byte ObjectID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableTransformNetworkData data = (InteractableTransformNetworkData)customObject;
		byte[] result = new byte[37];

		LARJMath.Vector3ToByte(data.Position).CopyTo(result, 0);
		LARJMath.Vector3ToByte(data.Rotation).CopyTo(result, 12);
		LARJMath.Vector3ToByte(data.LocalScale).CopyTo(result, 24);
		result[36] = data.ObjectID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableTransformNetworkData data = new InteractableTransformNetworkData();

		data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 0, 12));
		data.Rotation = LARJMath.ByteToVector3(LARJMath.SubArray(input, 12, 12));
		data.LocalScale = LARJMath.ByteToVector3(LARJMath.SubArray(input, 24, 12));
		data.ObjectID = input[36];

		return data;
	}
}
