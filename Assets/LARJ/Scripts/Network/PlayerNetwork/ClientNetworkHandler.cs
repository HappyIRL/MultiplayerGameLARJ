using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum LARJNetworkEvents
{
	PCUpdate = 128,
	InteractabnleUpdate = 129
}

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject[] _players = new GameObject[4];
	[SerializeField] private PlayerInteraction _playerInteraction;

	private LARJNetworkID myID = 0;

	public enum LARJNetworkID
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3
	}

	private void Start()
	{
		PhotonNetwork.AddCallbackTarget(this);
		_playerInteraction.LARJInteractableUse += UpdateLocalInteractables;
	}

	private void Update()
	{
		myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		UpdateLocalPlayerController();
	}

	private void UpdateLocalInteractables(InteractableObjectID id, InteractableUseType type)
	{

	}

	public void SetPlayers(GameObject[] go)
	{
		_players = go;
	}

	private void UpdateLocalPlayerController()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};

		GameObject player = GetPlayerFromID(myID);

		if(player != null)
		{
			ClientNetworkData clientNetworkData = new ClientNetworkData()
			{
				ID = (byte)myID,
				Position = player.transform.position,
				Rotation = player.transform.eulerAngles
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.PCUpdate, clientNetworkData, raiseEventOptions, sendOptions);
		}
		else
		{
			Debug.Log("Player is null ID: " + myID);
			return;
		}
	}

	private GameObject GetPlayerFromID(LARJNetworkID id)
	{
		switch (id)
		{
			case LARJNetworkID.Player1:
				return _players[0];
			case LARJNetworkID.Player2:
				return _players[1];
			case LARJNetworkID.Player3:
				return _players[2];
			case LARJNetworkID.Player4:
				return _players[3];
		}
		return null;
	}
	private void ReceiveUpdatePC(ClientNetworkData data)
	{
		GameObject player = GetPlayerFromID((LARJNetworkID)data.ID);

		Debug.Log(player);

		if(player != null)
		{
			player.transform.position = data.Position;
			player.transform.eulerAngles = data.Rotation;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		LARJNetworkEvents eventCode = (LARJNetworkEvents)photonEvent.Code;

		Debug.Log(eventCode);

		switch(eventCode)
		{
			case LARJNetworkEvents.PCUpdate:
				ReceiveUpdatePC((ClientNetworkData)photonEvent.CustomData);
				break;
		}
	}
}

