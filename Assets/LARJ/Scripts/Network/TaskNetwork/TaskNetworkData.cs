using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNetworkData
{
    public byte ID;
    public byte TaskState;
    public byte InteractableID;
	public byte ObjectInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		TaskNetworkData data = (TaskNetworkData)customObject;
		byte[] result = new byte[4];

		result[0] = data.ID;
		result[1] = data.TaskState;
		result[2] = data.InteractableID;
		result[3] = data.ObjectInstanceID;

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		TaskNetworkData data = new TaskNetworkData();

		data.ID = input[0];
		data.TaskState = input[1];
		data.InteractableID = input[2];
		data.ObjectInstanceID = input[3];

		return data;
	}
}
