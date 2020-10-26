using System;
public class CustomerNetworkData
{
	public int UniqueInstanceID;
	public InteractionType Type;

	public static byte[] SerializeMethod(object customObject)
	{
		CustomerNetworkData data = (CustomerNetworkData)customObject;

		byte[] result = new byte[5];
		result[0] = (byte)data.Type;
		BitConverter.GetBytes(data.UniqueInstanceID).CopyTo(result,1);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		CustomerNetworkData data = new CustomerNetworkData();

		data.Type = (InteractionType)input[0];
		data.UniqueInstanceID = BitConverter.ToInt32(input,1);

		return data;
	}
}
