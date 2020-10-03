using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public enum LARJNetworkEvents
{
	PCUpdate = 128,
	InteractableUpdate = 129
}

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject[] _players = new GameObject[4];
	[SerializeField] private PlayerInteraction _playerInteraction;
	[SerializeField] private List<GameObject> _interactables = new List<GameObject>();

	private GameObject _playerFromID;

	private LARJNetworkID myID = 0;
	private LARJNetworkID oldID = (LARJNetworkID)1;

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
	}

	private void Update()
	{
		myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		
		if(myID != oldID)
		{
			_playerFromID = GetPlayerFromID((LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1);
			_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
			_playerInteraction.LARJInteractableUse += UpdateLocalInteractables;
		}

		UpdateLocalPlayerController();
		oldID = myID;
	}


	public void SetPlayers(GameObject[] go)
	{
		_players = go;
	}

	public void SetInteractables(List<GameObject> go)
	{
		_interactables = go;
	}

	public void SetPlayerInteractionInstance(PlayerInteraction pi)
	{
		_playerInteraction = pi;
	}

	public void AddInteractable(GameObject go)
	{
		_interactables.Add(go);
	}
	private void UpdateLocalInteractables(InteractableObjectID id, InteractableUseType type)
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

		GameObject interactable_go = GetInteractableGoFromID(id);

		if(interactable_go != null)
		{
			InteractableNetworkData interactableNetworkData = new InteractableNetworkData()
			{
				playerID = (byte)myID,
				interactableID = (byte)id,
				interactableUseID = (byte)type
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.InteractableUpdate, interactableNetworkData, raiseEventOptions, sendOptions);
		}
		else
		{
			Debug.LogError("Interactable is null");
		}
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

	private GameObject GetInteractableGoFromID(InteractableObjectID id)
	{
		switch(id)
		{
			case InteractableObjectID.Broom:
				return _interactables[0];
			case InteractableObjectID.Telephone1:
				return _interactables[5];
			case InteractableObjectID.Telephone2:
				return _interactables[6];
			case InteractableObjectID.FireExtinguisher:
				return _interactables[1];
			case InteractableObjectID.Paper:
				return null;
			case InteractableObjectID.PC:
				return _interactables[2];
			case InteractableObjectID.Shotgun:
				return _interactables[4];
			case InteractableObjectID.WaterCooler:
				return _interactables[7];
			case InteractableObjectID.Printer:
				return _interactables[3];
		}

		return null;
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


		if(player != null)
		{
			player.transform.position = data.Position;
			player.transform.eulerAngles = data.Rotation;
		}
	}

	private void ReceiveInteractableUpdate(InteractableNetworkData data)
	{
		GameObject player = GetPlayerFromID((LARJNetworkID)data.playerID);
		InteractableUseType type = (InteractableUseType)data.interactableUseID;
		GameObject go = GetInteractableGoFromID((InteractableObjectID)data.interactableID);

		switch(type)
		{
			case InteractableUseType.Drop:
				Debug.Log("Dropped smthing");
				break;
			case InteractableUseType.PickUp:
				Debug.Log("PickedUp smthing");
				break;
			case InteractableUseType.PressInteraction:
				Debug.Log("Pressed smthing");
				break;
			case InteractableUseType.HoldInteraction:
				Debug.Log("Holds smthing");
				break;
		}


	}

	public void OnEvent(EventData photonEvent)
	{
		LARJNetworkEvents eventCode = (LARJNetworkEvents)photonEvent.Code;

		switch(eventCode)
		{
			case LARJNetworkEvents.PCUpdate:
				ReceiveUpdatePC((ClientNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.InteractableUpdate:
				ReceiveInteractableUpdate((InteractableNetworkData)photonEvent.CustomData);
				break;

		}
	}
}

