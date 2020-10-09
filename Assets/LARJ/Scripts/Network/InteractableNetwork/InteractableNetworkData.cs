using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNetworkData
{
	public byte ID;
	public byte InteractableID;
	public byte InteractableUseID;
	public byte ObjectInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableNetworkData data = (InteractableNetworkData)customObject;
		byte[] result = new byte[4];

		result[0] = data.ID;
		result[1] = data.InteractableID;
		result[2] = data.InteractableUseID;
		result[3] = data.ObjectInstanceID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableNetworkData data = new InteractableNetworkData();

		data.ID = input[0];
		data.InteractableID = input[1];
		data.InteractableUseID = input[2];
		data.ObjectInstanceID = input[3];

		return data;
	}
}
