using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class NotMasterClientInstantiateData
{
	public byte ID;
	public Vector3? Position = null;
	public Quaternion? Rotation = null;
	public int UniqueInstanceID = 0;

	public static byte[] SerializeMethod(object customObject)
	{
		NotMasterClientInstantiateData data = (NotMasterClientInstantiateData)customObject;
		byte[] b = BitConverter.GetBytes(data.UniqueInstanceID);
		byte[] result;

		if (data.Position == null)
		{
			result = new byte[1 + b.Length];
			result[0] = data.ID;
			b.CopyTo(result, 1);

		}
		else
		{
			result = new byte[29 + b.Length];
			result[0] = data.ID;
			LARJMath.Vector3ToByte((Vector3)data.Position).CopyTo(result, 1);
			LARJMath.QuaternionToByte((Quaternion)data.Rotation).CopyTo(result, 13);
			b.CopyTo(result, 29);
		}

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		NotMasterClientInstantiateData data = new NotMasterClientInstantiateData();

		if(input.Length == 1)
		{
			data.ID = input[0];
			data.UniqueInstanceID = BitConverter.ToInt32(input, 1);
		}
		else
		{
			data.ID = input[0];
			data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 1, 12));
			data.Rotation = LARJMath.ByteToQuaternion(LARJMath.SubArray(input, 13, 16));
			data.UniqueInstanceID = BitConverter.ToInt32(input, 29);
		}

		return data;
	}
}
