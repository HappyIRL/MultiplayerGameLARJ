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

	private LARJNetworkID _myID = 0;
	private LARJNetworkID _simulatedIDMovement = (LARJNetworkID)4;
	private InteractableObjectID _simulatedIDInteractables = (InteractableObjectID)100;
	private GameObject _simulatedPlayerGO = null;
	private GameObject _simulatedPlayerObjectHolder;
	private GameObject _simulatedInteractableGO;
	private Interactable _simulatedInteractable;

	public enum LARJNetworkID
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
		none = 4
	}

	private void Start()
	{
		PhotonNetwork.AddCallbackTarget(this);
		_myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		SubscribeToEvents();
	}

	private void Update()
	{
		UpdateLocalPlayerController();
	}

	private void SubscribeToEvents()
	{
		_playerFromID = GetPlayerFromID((LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1);
		_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
		_playerInteraction.LARJInteractableUse += UpdateLocalInteractables;
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
				ID = (byte)_myID,
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

		if(_playerFromID != null)
		{
			ClientNetworkData clientNetworkData = new ClientNetworkData()
			{
				ID = (byte)_myID,
				Position = _playerFromID.transform.position,
				Rotation = _playerFromID.transform.Find("BaseCharacter").eulerAngles
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.PCUpdate, clientNetworkData, raiseEventOptions, sendOptions);
		}
		else
		{
			Debug.Log("Player is null ID: " + _myID);
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
		if(_simulatedIDMovement != (LARJNetworkID)data.ID)
		{
			_simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
			_simulatedIDMovement = (LARJNetworkID)data.ID;
		}

		if(_simulatedPlayerGO != null)
		{
			_simulatedPlayerGO.transform.position = data.Position;
			_simulatedPlayerGO.transform.Find("BaseCharacter").eulerAngles = data.Rotation;
		}
	}

	private void ReceiveSimulatedPlayerPickUp()
	{
		_simulatedInteractable.DisableColliders();

		_simulatedInteractableGO.transform.rotation = _simulatedPlayerObjectHolder.transform.rotation;
		_simulatedInteractableGO.transform.position = _simulatedPlayerObjectHolder.transform.position;
		_simulatedInteractableGO.transform.parent = _simulatedPlayerObjectHolder.transform;
	}

	private void ReceiveSimulatedPlayerDrop()
	{
		_simulatedInteractableGO.transform.parent = null;
		_simulatedInteractable.EnableColliders();
	}

	private void ReceiveSimulatedPlayerPress()
	{
		_simulatedInteractable.PressEvent();
	}

	private void ReceiveSimulatedPlayerStartHold()
	{
		_simulatedInteractable.HoldingStartedEvent();
	}

	private void ReceiveSimulatedPlayerFailedHold()
	{
		_simulatedInteractable.HoldingFailedEvent();
	}

	private void ReceiveSimulatedPlayerFinishHold()
	{
		_simulatedInteractable.HoldingFinishedEvent();
	}

	private void ReceiveSimulatedPlayerMousePress()
	{
		_simulatedInteractable.MousePressEvent();
	}

	private void ReceiveSimulatedPlayerMouseRelease()
	{
		_simulatedInteractable.MouseReleaseEvent();
	}

	private void ReceiveInteractableUpdate(InteractableNetworkData data)
	{
		if (_simulatedIDInteractables != (InteractableObjectID)data.interactableID)
		{
			_simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
			_simulatedPlayerObjectHolder = _simulatedPlayerGO.transform.Find("BaseCharacter").Find("ObjectHolder").gameObject;
			_simulatedInteractableGO = GetInteractableGoFromID((InteractableObjectID)data.interactableID);
			_simulatedInteractable = _simulatedInteractableGO.GetComponent<Interactable>();
			_simulatedIDInteractables = (InteractableObjectID)data.interactableID;
		}

		InteractableUseType type = (InteractableUseType)data.interactableUseID;

		switch(type)
		{
			case InteractableUseType.Drop:
				ReceiveSimulatedPlayerDrop();
				break;
			case InteractableUseType.PickUp:
				ReceiveSimulatedPlayerPickUp();
				break;
			case InteractableUseType.Press:
				ReceiveSimulatedPlayerPress();
				break;
			case InteractableUseType.HoldStart:
				ReceiveSimulatedPlayerStartHold();
				break;
			case InteractableUseType.HoldFailed:
				ReceiveSimulatedPlayerFailedHold();
				break;
			case InteractableUseType.HoldFinish:
				ReceiveSimulatedPlayerFinishHold();
				break;
			case InteractableUseType.MousePress:
				ReceiveSimulatedPlayerMousePress();
				break;
			case InteractableUseType.MouseRelease:
				ReceiveSimulatedPlayerMouseRelease();
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

