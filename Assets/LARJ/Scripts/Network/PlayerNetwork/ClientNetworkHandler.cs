using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Tasks;
using UnityEngine;

public enum LARJNetworkEvents
{
	PCUpdate = 128,
	InteractableUpdate = 129,
	TaskUpdate = 130,
	CustomerSpawn = 131
}

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject[] _players = new GameObject[4];
	private PlayerInteraction _playerInteraction;
	[SerializeField] private List<GameObject> _interactables = new List<GameObject>();

	private GameObject _playerFromID;

	private LARJNetworkID _myID = 0;
	private LARJNetworkID _simulatedID = (LARJNetworkID)4;
	private InteractableObjectID _simulatedIDInteractables = (InteractableObjectID)100;
	private GameObject _simulatedPlayerGO = null;
	private GameObject _simulatedPlayerObjectHolder;
	private GameObject _simulatedInteractableGO;
	private Interactable _simulatedInteractable;
	private GameObject _myPlayer;
	private CustomerSpawner _customerSpawner;
	private Dictionary<int, GameObject> _customerIDs = new Dictionary<int, GameObject>();
	private int _uniqueCustomerID = 0;

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
		_customerSpawner = FindObjectOfType<CustomerSpawner>();
		_customerSpawner.OnCustomerSpawn += OnCustomerSpawn;
		_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
		_playerInteraction.OnNetworkTaskEvent += OnNetworkTask;
		_playerInteraction.LARJInteractableUse += UpdateLocalInteractables;
	}

	private void OnCustomerSpawn(GameObject go)
	{
		_customerIDs.Add(_uniqueCustomerID + 73, go);
		go.GetComponent<Customer>().SetID(73 + _uniqueCustomerID);
		RaiseNetworkedCustomer(73 + _uniqueCustomerID);
		_uniqueCustomerID+=1;
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

	private void OnNetworkTask(InteractableObjectID id, LARJTaskState state)
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
		if (_playerFromID != null)
		{
			TaskNetworkData taskNetworkData = new TaskNetworkData()
			{
				ID = (byte)_myID,
				TaskState = (byte)state,
				InteractableID = (byte)id
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.TaskUpdate, taskNetworkData, raiseEventOptions, sendOptions);
		}
	}

	private void RaiseNetworkedCustomer(int id)
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

		CustomerNetworkData interactableNetworkData = new CustomerNetworkData()
		{
			ID = (byte)id,
		};

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.CustomerSpawn, interactableNetworkData, raiseEventOptions, sendOptions);
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
				InteractableID = (byte)id,
				InteractableUseID = (byte)type
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

		if ((int)id >= 73)
		{
			if (_customerIDs.ContainsKey((int)id))
			{
				return _customerIDs[(int)id];
			}
			else
			{
				Debug.LogError("No Customer with Key: " + (int) id);
			}
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
		if(_simulatedID != (LARJNetworkID)data.ID)
		{
			_simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
			_simulatedID = (LARJNetworkID)data.ID;
		}

		if (_simulatedPlayerGO != null)
		{
			_simulatedPlayerGO.transform.position = data.Position;
			_simulatedPlayerGO.GetComponent<SimulatedPlayer>()._baseCharacter.transform.eulerAngles = data.Rotation;
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

	private void ReceiveTaskUpdate(TaskNetworkData data)
	{
		_myPlayer = GetPlayerFromID(_myID);
		_playerInteraction = _myPlayer.GetComponent<PlayerInteraction>();
		Interactable interactable = GetInteractableGoFromID((InteractableObjectID)data.InteractableID).GetComponent<Interactable>();
		Task task = interactable.GetComponent<Task>();
		TaskManagerUI taskManagerUI = TaskManager.TaskManagerSingelton.TaskManagerUI;
		Score score = TaskManager.TaskManagerSingelton.Score;

		switch ((LARJTaskState)data.TaskState)
		{
			case LARJTaskState.TaskComplete:
				if (_playerInteraction.AllowedInteractibles.Contains(interactable))
					_playerInteraction.AllowedInteractibles.Remove(interactable);
				task.IsTaskActive = false;
				task.StopTask();
				taskManagerUI.RemoveUITask(task.TaskUI);
				score.UpdateScore(task.GetRewardMoney, true);
				break;

			case LARJTaskState.TaskFailed:
				if (_playerInteraction.AllowedInteractibles.Contains(interactable))
					_playerInteraction.AllowedInteractibles.Remove(interactable);
				task.IsTaskActive = false;
				task.StopTask();
				taskManagerUI.RemoveUITask(task.TaskUI);
				score.UpdateScore(task.GetLostMoneyOnFail, false);
				break;

			case LARJTaskState.TaskStart:
				_playerInteraction.AllowedInteractibles.Add(GetInteractableGoFromID((InteractableObjectID)data.InteractableID).GetComponent<Interactable>());
				task.TaskUI = taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
				break;
		}
	}

	private void ReceiveCustomerSpawn(CustomerNetworkData data)
	{
		var go = _customerSpawner.SpawnNetworkedCustomer();
		go.GetComponent<Customer>().SetID(data.ID);
		if (!_customerIDs.ContainsKey(data.ID))
		{
			_customerIDs.Add(data.ID, go);
		}
	}
	private void ReceiveInteractableUpdate(InteractableNetworkData data)
	{
		if (_simulatedIDInteractables != (InteractableObjectID)data.InteractableID)
		{
			_simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
			_simulatedPlayerObjectHolder = _simulatedPlayerGO.GetComponent<SimulatedPlayer>()._objectHolder;
			_simulatedInteractableGO = GetInteractableGoFromID((InteractableObjectID)data.InteractableID);
			_simulatedInteractable = _simulatedInteractableGO.GetComponent<Interactable>();
			_simulatedIDInteractables = (InteractableObjectID)data.InteractableID;
		}

		InteractableUseType type = (InteractableUseType)data.InteractableUseID;

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
			case LARJNetworkEvents.TaskUpdate:
				ReceiveTaskUpdate((TaskNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.CustomerSpawn:
				//ReceiveCustomerSpawn((CustomerNetworkData)photonEvent.CustomData);
				break;

		}
	}
}

