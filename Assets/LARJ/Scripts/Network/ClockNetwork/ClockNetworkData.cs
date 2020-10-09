using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockNetworkData
{
	public int Time;

	public static byte[] SerializeMethod(object customObject)
	{
		ClockNetworkData data = (ClockNetworkData)customObject;
		byte[] result = new byte[1];

		result = BitConverter.GetBytes(data.Time);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		ClockNetworkData data = new ClockNetworkData();

		data.Time = BitConverter.ToInt32(input, 0);

		return data;
	}
}
