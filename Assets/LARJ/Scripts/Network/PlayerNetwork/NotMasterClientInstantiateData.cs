using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class NotMasterClientInstantiateData
{
	public byte ID;
	public Vector3 LocalScale;
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
			result = new byte[17];
			result[0] = data.ID;
			LARJMath.Vector3ToByte(data.LocalScale).CopyTo(result, 1);
			b.CopyTo(result, 13);

		}
		else
		{
			result = new byte[45];
			result[0] = data.ID;
			LARJMath.Vector3ToByte(data.Position.Value).CopyTo(result, 1);
			LARJMath.QuaternionToByte(data.Rotation.Value).CopyTo(result, 13);
			LARJMath.Vector3ToByte(data.LocalScale).CopyTo(result, 29);
			b.CopyTo(result, 41);
		}

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		NotMasterClientInstantiateData data = new NotMasterClientInstantiateData();

		if(input.Length == 17)
		{
			data.ID = input[0];
			data.LocalScale = LARJMath.ByteToVector3(LARJMath.SubArray(input, 1, 12));
			data.UniqueInstanceID = BitConverter.ToInt32(input, 13);
		}
		else
		{
			data.ID = input[0];
			data.Position = LARJMath.ByteToVector3(LARJMath.SubArray(input, 1, 12));
			data.Rotation = LARJMath.ByteToQuaternion(LARJMath.SubArray(input, 13, 16));
			data.LocalScale = LARJMath.ByteToVector3(LARJMath.SubArray(input, 29, 12));
			data.UniqueInstanceID = BitConverter.ToInt32(input, 41);
		}

		return data;
	}
}
