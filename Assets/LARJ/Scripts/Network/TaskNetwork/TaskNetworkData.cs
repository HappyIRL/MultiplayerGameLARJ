using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNetworkData
{
    public byte ID;
    public byte TaskState;
	public int ObjectInstanceID;
	public bool StopInteractable;

	public static byte[] SerializeMethod(object customObject)
	{
		TaskNetworkData data = (TaskNetworkData)customObject;
		byte[] instanceID = BitConverter.GetBytes(data.ObjectInstanceID);
		byte[] result = new byte[7];

		result[0] = data.ID;
		result[1] = data.TaskState;
		instanceID.CopyTo(result, 2);
		BitConverter.GetBytes(data.StopInteractable).CopyTo(result, 6);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		TaskNetworkData data = new TaskNetworkData();

		data.ID = input[0];
		data.TaskState = input[1];
		data.ObjectInstanceID = BitConverter.ToInt32(input, 2);
		data.StopInteractable = BitConverter.ToBoolean(input, 6);

		return data;
	}
}
