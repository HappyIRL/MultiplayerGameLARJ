using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNetworkData
{
    public byte ID;
    public bool Active;
    public byte InteractableID;

	public static byte[] SerializeMethod(object customObject)
	{
		TaskNetworkData data = (TaskNetworkData)customObject;
		byte[] result = new byte[3];

		result[0] = data.ID;
		if (data.Active) result[1] = 1;
		else result[1] = 0;
		result[2] = data.InteractableID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		TaskNetworkData data = new TaskNetworkData();

		data.ID = input[0];
		if (input[1] == 0) data.Active = false;
		else data.Active = true;
		data.InteractableID = input[2];

		return data;
	}
}
