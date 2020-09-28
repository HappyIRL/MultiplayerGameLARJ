using UnityEngine;
using WebSocketSharp;

public class ClientNetworkData
{
	public byte ID;
	public Vector3 Position;
	public Vector3 Rotation;

	public static byte[] SerializeMethod(object customObject)
	{
		ClientNetworkData data = (ClientNetworkData)customObject;
		byte[] result = new byte[25];
		
		result[0] = data.ID;
		data.Position.ToByteArray(ByteOrder.Big).CopyTo(result, 1);
		data.Rotation.ToByteArray(ByteOrder.Big).CopyTo(result, 13);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		ClientNetworkData data = new ClientNetworkData();
		
		data.ID = input[0];
		data.Position = input.SubArray(1, 12).To<Vector3>(ByteOrder.Big);
		data.Rotation = input.SubArray(13, 12).To<Vector3>(ByteOrder.Big);

		return data;
	}
}
