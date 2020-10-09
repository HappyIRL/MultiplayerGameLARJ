using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerNetworkData
{
	public byte ObjectInteractableID;

	public static byte[] SerializeMethod(object customObject)
	{
		CustomerNetworkData data = (CustomerNetworkData)customObject;
		byte[] result = new byte[1];

		result[0] = data.ObjectInteractableID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		CustomerNetworkData data = new CustomerNetworkData();

		data.ObjectInteractableID = input[0];

		return data;
	}
}
