using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNetworkData
{
	public byte ID;
	public byte InteractableUseID;
	public byte ItemInHandID;
	public int ObjectInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableNetworkData data = (InteractableNetworkData)customObject;
		byte[] b = BitConverter.GetBytes(data.ObjectInstanceID);
		byte[] result = new byte[4 + b.Length];

		result[0] = data.ID;
		result[1] = data.InteractableUseID;
		result[2] = data.ItemInHandID;
		b.CopyTo(result, 3);
		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableNetworkData data = new InteractableNetworkData();

		data.ID = input[0];
		data.InteractableUseID = input[1];
		data.ItemInHandID = input[2];
		data.ObjectInstanceID = BitConverter.ToInt32(input, 3);

		return data;
	}
}
