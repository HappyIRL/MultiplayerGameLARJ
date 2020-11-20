using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNetworkData
{
    public byte ID;
    public byte TaskState;
	public int ObjectInstanceID;

	public static byte[] SerializeMethod(object customObject)
	{
		TaskNetworkData data = (TaskNetworkData)customObject;
		byte[] instanceID = BitConverter.GetBytes(data.ObjectInstanceID);
		byte[] result = new byte[6];

		result[0] = data.ID;
		result[1] = data.TaskState;
		instanceID.CopyTo(result, 2);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		TaskNetworkData data = new TaskNetworkData();

		data.ID = input[0];
		data.TaskState = input[1];
		data.ObjectInstanceID = BitConverter.ToInt32(input, 2);

		return data;
	}
}
