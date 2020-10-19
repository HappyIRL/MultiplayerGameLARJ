using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkCharacterSetup: MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private GameObject _simulatedPlayer;
	[SerializeField] private List<Interactable> _interactables = new List<Interactable>();
	[SerializeField] private GameObject _healtbarCanvasPrefab = null;

	private GameObject[] _players = new GameObject[4];

	private ClientNetworkHandler _clientNetworkHandler;

	public ClientNetworkHandler ClientNetworkHandler { get => _clientNetworkHandler; }

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
		PhotonPeer.RegisterType(typeof(InteractableTransformNetworkData), (byte)LARJNetworkEvents.SyncInteractablesFromMaster, InteractableTransformNetworkData.SerializeMethod, InteractableTransformNetworkData.DeserializeMethod);

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
					_players[i] = Instantiate(_playerPrefab);
				}
				else
				{
					_players[i] = Instantiate(_simulatedPlayer);
					var simPlayer = _players[i].GetComponent<SimulatedPlayer>();
					simPlayer.SetHairColor(Color.HSVToRGB((float)i / 4, 1, 1));
				}
			}
			ClientNetworkHandler.SetPlayers(_players);
			ClientNetworkHandler.SetInteractables(_interactables);
			ClientNetworkHandler.HealthbarCanvasPrefab = _healtbarCanvasPrefab;
		}
	}
}
