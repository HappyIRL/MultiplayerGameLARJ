﻿using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkCharacterSetup : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private GameObject _simulatedPlayer;
	[SerializeField] private List<Interactable> _interactables = new List<Interactable>();
	[SerializeField] private List<GameObject> _syncOnStartGOs = new List<GameObject>();
	[SerializeField] private GameObject _healtbarCanvasPrefab = null;

	private GameObject[] _players = new GameObject[4];

	private ClientNetworkHandler _clientNetworkHandler;

	public ClientNetworkHandler ClientNetworkHandler { get => _clientNetworkHandler; }

	public void Awake()
	{
		if (PhotonNetwork.IsConnected)
		{
			_clientNetworkHandler = gameObject.AddComponent<ClientNetworkHandler>();
			Destroy(FindObjectOfType<PlayerInputManager>());
			foreach (PlayerInput x in FindObjectsOfType<PlayerInput>())
			{
				Destroy(x.gameObject);
			}

			for (int i = 0; i < 4; i++)
			{
				if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == i)
				{
					_players[i] = Instantiate(_playerPrefab, new Vector3(PhotonNetwork.LocalPlayer.ActorNumber - 3, 1.155f, 1), Quaternion.identity);
				}
				else
				{
					_players[i] = Instantiate(_simulatedPlayer, new Vector3(i - 2, 1.155f, 1), Quaternion.identity);
				}
				var simPlayer = _players[i].GetComponent<PlayerCharacterAppearance>();
				simPlayer.SetHairColor(Color.HSVToRGB((float)i / 4, 1, 1));
			}
			ClientNetworkHandler.SetPlayers(_players);
			ClientNetworkHandler.SetInteractables(_interactables, _syncOnStartGOs);
			ClientNetworkHandler.HealthbarCanvasPrefab = _healtbarCanvasPrefab;
		}
	}

	private void Start()
	{
		PhotonPeer.RegisterType(typeof(ClientNetworkData), (byte)LARJNetworkEvents.PCUpdate, ClientNetworkData.SerializeMethod, ClientNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(ClientNetworkData), (byte)LARJNetworkEvents.NotifyMasterOnSceneLoad, ClientNetworkData.SerializeMethod, ClientNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(InteractableNetworkData), (byte)LARJNetworkEvents.InteractableUpdate, InteractableNetworkData.SerializeMethod, InteractableNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(TaskNetworkData), (byte)LARJNetworkEvents.TaskUpdate, TaskNetworkData.SerializeMethod, TaskNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(CustomerNetworkData), (byte)LARJNetworkEvents.CustomerSpawn, CustomerNetworkData.SerializeMethod, CustomerNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(ClockNetworkData), (byte)LARJNetworkEvents.ClockUpdate, ClockNetworkData.SerializeMethod, ClockNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(NotMasterClientInstantiateData), (byte)LARJNetworkEvents.InstantiateOnMaster, NotMasterClientInstantiateData.SerializeMethod, NotMasterClientInstantiateData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(NotMasterClientInstantiateData), (byte)LARJNetworkEvents.InstantiateOnOther, NotMasterClientInstantiateData.SerializeMethod, NotMasterClientInstantiateData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(OnStartClientNetworkData), (byte)LARJNetworkEvents.SyncOnStartFromMaster, OnStartClientNetworkData.SerializeMethod, OnStartClientNetworkData.DeserializeMethod);
	}
}
