using System;
public class CustomerNetworkData
{
	public int UniqueInstanceID;
	public InteractionType Type;
	public bool WantsMoney;

	public static byte[] SerializeMethod(object customObject)
	{
		CustomerNetworkData data = (CustomerNetworkData)customObject;

		byte[] result = new byte[6];
		result[0] = (byte)data.Type;
		BitConverter.GetBytes(data.UniqueInstanceID).CopyTo(result,1);
		BitConverter.GetBytes(data.WantsMoney).CopyTo(result, 5);

		return result;
	}

	public static object DeserializeMethod(byte[] input)
	{
		CustomerNetworkData data = new CustomerNetworkData();

		data.Type = (InteractionType)input[0];
		data.UniqueInstanceID = BitConverter.ToInt32(input,1);
		data.WantsMoney = BitConverter.ToBoolean(input, 5);

		return data;
	}
}
