using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerNetworkData
{
	public byte ID;

	public static byte[] SerializeMethod(object customObject)
	{
		CustomerNetworkData data = (CustomerNetworkData)customObject;
		byte[] result = new byte[1];

		result[0] = data.ID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		CustomerNetworkData data = new CustomerNetworkData();

		data.ID = input[0];

		return data;
	}
}
