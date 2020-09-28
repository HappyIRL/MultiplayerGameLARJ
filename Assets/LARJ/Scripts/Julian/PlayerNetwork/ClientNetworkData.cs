using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkData : MonoBehaviour
{
	private byte _id;
	private Transform _transform;

	public ClientNetworkData(byte id, Transform transform)
	{
		this._id = id;
		this._transform = transform;
	}

	public int ID
	{
		get { return _id; }
	}

	public Transform Transform
	{
		get { return _transform; }
	}

}
