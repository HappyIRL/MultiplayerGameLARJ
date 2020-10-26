using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
			if (!PhotonNetwork.IsMasterClient)
				ClientNetworkHandler.OnNotMasterClientInstantiate(prefabGO);
			else
			{
				return ClientNetworkHandler.OnMasterClientInstantiate(prefabGO);
			}
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
			if(!PhotonNetwork.IsMasterClient)
				ClientNetworkHandler.OnNotMasterClientInstantiate(prefabGO, position, rotation);
			else
			{
				return ClientNetworkHandler.OnMasterClientInstantiate(prefabGO, position, rotation);
			}
				
		}
		else
		{
			GameObject go = Object.Instantiate(prefabGO, position, rotation);
			return go;
		}
		return null;
	}

	public GameObject ForceLocalInstantiate(GameObject prefabGO)
	{
		GameObject go = Object.Instantiate(prefabGO);
		SpawnGarbageHealthbar(go);
		return go;
	}

	public GameObject ForceLocalInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		GameObject go = Object.Instantiate(prefabGO, position, rotation);
		SpawnGarbageHealthbar(go);
		return go;
	}

	public void SpawnGarbageHealthbar(GameObject duplicatedObject)
    {
		GameObject healthbarCanvas = Object.Instantiate(HealthbarCanvasHolder.Instance.HealthbarCanvasPrefab);
		healthbarCanvas.transform.SetParent(duplicatedObject.transform);
		healthbarCanvas.transform.position = duplicatedObject.transform.position + Vector3.up;

		duplicatedObject.AddComponent<Garbage>();
	}
}
