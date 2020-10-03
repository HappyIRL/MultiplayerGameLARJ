using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkCharacterSetup: MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private GameObject _simulatedPlayer;
	[SerializeField] private List<GameObject> _interactables = new List<GameObject>();

	private GameObject[] _players = new GameObject[4];


	private ClientNetworkHandler _clientNetworkHandler;

	private void Start()
	{

		PhotonPeer.RegisterType(typeof(ClientNetworkData), (byte)LARJNetworkEvents.PCUpdate, ClientNetworkData.SerializeMethod, ClientNetworkData.DeserializeMethod);
		PhotonPeer.RegisterType(typeof(InteractableNetworkData), (byte)LARJNetworkEvents.InteractableUpdate, InteractableNetworkData.SerializeMethod, InteractableNetworkData.DeserializeMethod);

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

			_clientNetworkHandler.SetPlayers(_players);
			_clientNetworkHandler.SetInteractables(_interactables);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
	}
}
