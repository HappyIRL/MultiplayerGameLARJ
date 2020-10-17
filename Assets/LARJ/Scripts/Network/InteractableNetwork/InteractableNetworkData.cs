using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNetworkData
{
	public byte ID;
	public byte InteractableID;
	public byte InteractableUseID;
	public byte ItemInHandID;
	public int ObjectInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		InteractableNetworkData data = (InteractableNetworkData)customObject;
		byte[] b = BitConverter.GetBytes(data.ObjectInstanceID);
		byte[] result = new byte[5 + b.Length];

		result[0] = data.ID;
		result[1] = data.InteractableID;
		result[2] = data.InteractableUseID;
		result[3] = data.ItemInHandID;
		b.CopyTo(result, 4);
		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		InteractableNetworkData data = new InteractableNetworkData();

		data.ID = input[0];
		data.InteractableID = input[1];
		data.InteractableUseID = input[2];
		data.ItemInHandID = input[3];
		data.ObjectInstanceID = BitConverter.ToInt32(input, 4);

		return data;
	}
}
