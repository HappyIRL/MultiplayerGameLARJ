using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNetworkData
{
	public byte ID;
	public byte InteractableID;
	public byte InteractableUseID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableNetworkData data = (InteractableNetworkData)customObject;
		byte[] result = new byte[3];

		result[0] = data.ID;
		result[1] = data.InteractableID;
		result[2] = data.InteractableUseID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableNetworkData data = new InteractableNetworkData();

		data.ID = input[0];
		data.InteractableID = input[1];
		data.InteractableUseID = input[2];

		return data;
	}
}
