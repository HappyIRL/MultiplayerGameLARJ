using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkCharacterSetup: MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _playerPrefab;
	[SerializeField] private GameObject _simulatedPlayer;

	private GameObject[] _players = new GameObject[4];


	private ClientNetworkHandler _clientNetworkHandler;

	private void Start()
	{

		PhotonPeer.RegisterType(typeof(ClientNetworkData), (byte)LARJNetworkEvents.PCUpdate, ClientNetworkData.SerializeMethod, ClientNetworkData.DeserializeMethod);

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
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
	}
}
