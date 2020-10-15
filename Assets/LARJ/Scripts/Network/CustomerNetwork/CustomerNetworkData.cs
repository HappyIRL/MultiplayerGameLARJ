using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerNetworkData
{
	public int UniqueInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		CustomerNetworkData data = (CustomerNetworkData)customObject;

		byte[] result = BitConverter.GetBytes(data.UniqueInstanceID);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		CustomerNetworkData data = new CustomerNetworkData();

		data.UniqueInstanceID = BitConverter.ToInt32(input,0);

		return data;
	}
}
