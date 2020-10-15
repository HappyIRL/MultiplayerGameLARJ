using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NotMasterClientInstantiateData
{
	public byte ID;
	public Vector3? Position = null;
	public Quaternion? Rotation = null;

	public static byte[] SerializeMethod(object customObject)
	{
		NotMasterClientInstantiateData data = (NotMasterClientInstantiateData)customObject;
		byte[] result;

		if (data.Position == null)
		{
			result = new byte[1];
			result[0] = data.ID;

		}
		else
		{
			result = new byte[29];
			result[0] = data.ID;
			LARJMath.Vector3ToByte((Vector3)data.Position).CopyTo(result, 1);
			LARJMath.QuaternionToByte((Quaternion)data.Rotation).CopyTo(result, 13);
		}

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		NotMasterClientInstantiateData data = new NotMasterClientInstantiateData();

		if(input.Length == 1)
		{
			data.ID = input[0];
		}
		else
		{
			data.ID = input[0];
			data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 1, 12));
			data.Rotation = LARJMath.ByteToQuaternion(LARJMath.SubArray(input, 13, 16));
		}

		return data;
	}
}
