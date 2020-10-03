using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNetworkData : MonoBehaviour
{
	public byte playerID;
	public byte interactableID;
	public byte interactableUseID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableNetworkData data = (InteractableNetworkData)customObject;
		byte[] result = new byte[3];

		result[0] = data.playerID;
		result[1] = data.interactableID;
		result[2] = data.interactableUseID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableNetworkData data = new InteractableNetworkData();

		data.playerID = input[0];
		data.interactableID = input[1];
		data.interactableUseID = input[2];

		return data;
	}
}
