using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateManager
{
	private static InstantiateManager _instance;

	private ClientNetworkHandler _clientNetworkHandler;

	private ClientNetworkHandler ClientNetworkHandler
	{
		get
		{
			if (_clientNetworkHandler == null)
				_clientNetworkHandler = Object.FindObjectOfType<NetworkCharacterSetup>().ClientNetworkHandler;
			return _clientNetworkHandler;
		}
	}

	public static InstantiateManager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new InstantiateManager();
			}
			return _instance;
		}
	}

	public GameObject Instantiate(GameObject prefabGO)
	{
		if(PhotonNetwork.IsConnected)
		{
			ClientNetworkHandler.OnNotMasterClientInstantiate(prefabGO);
		}
		else
		{
			GameObject go = Object.Instantiate(prefabGO);
			return go;
		}
		return null;
	}

	public GameObject Instantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		if (PhotonNetwork.IsConnected)
		{
			ClientNetworkHandler.OnNotMasterClientInstantiate(prefabGO, position, rotation);
		}
		else
		{
			GameObject go = Object.Instantiate(prefabGO, position, rotation);
			return go;
		}
		return null;
	}

	public GameObject Instantiate(GameObject prefabGO, Transform parent)
	{
		if (PhotonNetwork.IsConnected)
		{
			ClientNetworkHandler.OnNotMasterClientInstantiate(prefabGO, parent);
		}
		else
		{
			GameObject go = Object.Instantiate(prefabGO, parent);
			return go;
		}
		return null;
	}

}
