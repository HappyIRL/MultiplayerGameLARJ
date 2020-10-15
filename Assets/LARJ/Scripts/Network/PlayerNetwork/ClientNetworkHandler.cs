using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Tasks;
using UnityEngine;

public enum LARJNetworkEvents
{
	PCUpdate = 128,
	InteractableUpdate = 129,
	TaskUpdate = 130,
	CustomerSpawn = 131,
	ClockUpdate = 132
}

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject[] _players = new GameObject[4];
	private PlayerInteraction _playerInteraction;
	[SerializeField] private List<GameObject> _interactables = new List<GameObject>();

	private GameObject _playerFromID;

	private LARJNetworkID _myID = 0;
	private LARJNetworkID _simulatedID = (LARJNetworkID)4;
	private GameObject _simulatedPlayerGO = null;
	private GameObject _myPlayer;
	private CustomerSpawner _customerSpawner;
	private Dictionary<int, GameObject> _instanceIDs = new Dictionary<int, GameObject>();

	// _uniqueCustomerID == 0 returns the object in scene.
	private int _uniqueInstanceID = 1;


	private DayTimeManager _dayTimeManager;

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
		_dayTimeManager = FindObjectOfType<DayTimeManager>();
		PhotonNetwork.AddCallbackTarget(this);
		_myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		SubscribeToEvents();
	}

	private void Update()
	{
		UpdateLocalPlayerController();
		if(PhotonNetwork.IsMasterClient)
			UpdateLocalTime();
	}

	private void SubscribeToEvents()
	{
		_playerFromID = GetPlayerFromID((LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1);
		_customerSpawner = FindObjectOfType<CustomerSpawner>();
		_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
		_customerSpawner.OnCustomerSpawn += OnCustomerSpawn;
		_playerInteraction.OnNetworkTaskEvent += OnNetworkTask;
		_playerInteraction.LARJInteractableUse += RaiseNetworkedInteractable;
	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO)
	{

	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{

	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO, Transform parent)
	{

	}

	private void OnCustomerSpawn(GameObject go)
	{
		AddInstanceToObjectList(go);
		RaiseNetworkedCustomer(_uniqueInstanceID);
	}

	private void AddInstanceToObjectList(GameObject go)
	{
		_instanceIDs.Add(_uniqueInstanceID, go);
		go.GetComponent<Interactable>().ObjectInstanceID = _uniqueInstanceID;
		_uniqueInstanceID += 1;
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

	private void OnNetworkTask(InteractableObjectID id, LARJTaskState state, int objectInstanceID)
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
				InteractableID = (byte)id,
				ObjectInstanceID = objectInstanceID
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.TaskUpdate, taskNetworkData, raiseEventOptions, sendOptions);
		}
	}

	private void RaiseNetworkedCustomer(int _uniqueInstanceID)
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
			UniqueInstanceID = _uniqueInstanceID,
		};

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.CustomerSpawn, interactableNetworkData, raiseEventOptions, sendOptions);
	}

	private void RaiseNetworkedInteractable(InteractableObjectID id, InteractableUseType type, int objectInstanceID, InteractableObjectID itemInHandID)
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

		GameObject interactable_go = GetInteractableGoFromID(id, objectInstanceID);

		if(interactable_go != null)
		{
			InteractableNetworkData interactableNetworkData = new InteractableNetworkData()
			{
				ID = (byte)_myID,
				InteractableID = (byte)id,
				InteractableUseID = (byte)type,
				ItemInHandID = (byte)itemInHandID,
				ObjectInstanceID = objectInstanceID

			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.InteractableUpdate, interactableNetworkData, raiseEventOptions, sendOptions);
		}
	}

	private void UpdateLocalTime()
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

		int time = _dayTimeManager.CurrentMinutes;
		time += _dayTimeManager.CurrentHour * 60;

		if (_playerFromID != null)
		{
			ClockNetworkData clockNetworkData = new ClockNetworkData()
			{
				Time = time
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.ClockUpdate, clockNetworkData, raiseEventOptions, sendOptions);
		}
		else
		{
			Debug.Log("Player is null ID: " + _myID);
			return;
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

	private GameObject GetInteractableGoFromID(InteractableObjectID id, int objectInstanceID)
	{
		if (_instanceIDs.ContainsKey(objectInstanceID))
			return _instanceIDs[objectInstanceID];

		switch (id)
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
				return _interactables[8];

			case InteractableObjectID.PC:
				return _interactables[2];

			case InteractableObjectID.Shotgun:
				return _interactables[4];

			case InteractableObjectID.WaterCooler:
				return _interactables[7];

			case InteractableObjectID.Printer:
				return _interactables[3];

			case InteractableObjectID.Customer:
				break;
			case InteractableObjectID.None:
				break;
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

	private void ReceiveSimulatedPlayerPickUp(GameObject simulatedInteractableGO, Interactable simulatedInteractable, GameObject simulatedPlayerObjectHolder)
	{
		simulatedInteractable.DisableColliders();

		simulatedInteractableGO.transform.rotation = simulatedPlayerObjectHolder.transform.rotation;
		simulatedInteractableGO.transform.position = simulatedPlayerObjectHolder.transform.position;
		simulatedInteractableGO.transform.parent = simulatedPlayerObjectHolder.transform;
	}

	private void ReceiveSimulatedPlayerDrop(GameObject simulatedInteractableGO, Interactable simulatedInteractable)
	{
		simulatedInteractableGO.transform.parent = null;
		simulatedInteractable.EnableColliders();
	}

	private void ReceiveSimulatedPlayerPress(Interactable simulatedInteractable)
	{
		simulatedInteractable.PressEvent();
	}

	private void ReceiveSimulatedPlayerStartHold(Interactable simulatedInteractable)
	{
		simulatedInteractable.HoldingStartedEvent();
	}

	private void ReceiveSimulatedPlayerFailedHold(Interactable simulatedInteractable)
	{
		simulatedInteractable.HoldingFailedEvent();
	}

	private void ReceiveSimulatedPlayerFinishHold(Interactable simulatedInteractable, GameObject simulatedCloneGO)
	{
		if (simulatedCloneGO != null)
			simulatedInteractable.HoldingFinishedEvent(simulatedCloneGO);
		else
			simulatedInteractable.HoldingFinishedEvent();
	}

	private void ReceiveSimulatedPlayerMousePress(Interactable simulatedInteractable)
	{
		simulatedInteractable.MousePressEvent();
	}

	private void ReceiveSimulatedPlayerMouseRelease(Interactable simulatedInteractable)
	{
		simulatedInteractable.MouseReleaseEvent();
	}

	private void ReceiveTaskUpdate(TaskNetworkData data)
	{
		_myPlayer = GetPlayerFromID(_myID);
		_playerInteraction = _myPlayer.GetComponent<PlayerInteraction>();
		Interactable interactable = GetInteractableGoFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID).GetComponent<Interactable>();
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
				_playerInteraction.AllowedInteractibles.Add(GetInteractableGoFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID).GetComponent<Interactable>());
				task.TaskUI = taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
				break;
		}
	}

	private void ReceiveCustomerSpawn(CustomerNetworkData data)
	{
		var go = _customerSpawner.SpawnNetworkedCustomer();
		go.GetComponent<Interactable>().ObjectInstanceID = _uniqueInstanceID;
		if (!_instanceIDs.ContainsKey(data.UniqueInstanceID))
		{
			_instanceIDs.Add(data.UniqueInstanceID, go);
		}
	}
	private void ReceiveInteractableUpdate(InteractableNetworkData data)
	{
		GameObject simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
		GameObject simulatedPlayerObjectHolder = simulatedPlayerGO.GetComponent<SimulatedPlayer>()._objectHolder;
		GameObject simulatedInteractableGO = GetInteractableGoFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID);
		Interactable simulatedInteractable = simulatedInteractableGO.GetComponent<Interactable>();
		GameObject simulatedCloneGO = GetInteractableGoFromID((InteractableObjectID)data.ItemInHandID, 0);


		InteractableUseType type = (InteractableUseType)data.InteractableUseID;

		switch(type)
		{
			case InteractableUseType.Drop:
				ReceiveSimulatedPlayerDrop(simulatedInteractableGO, simulatedInteractable);
				break;
			case InteractableUseType.PickUp:
				ReceiveSimulatedPlayerPickUp(simulatedInteractableGO, simulatedInteractable, simulatedPlayerObjectHolder);
				break;
			case InteractableUseType.Press:
				ReceiveSimulatedPlayerPress(simulatedInteractable);
				break;
			case InteractableUseType.HoldStart:
				ReceiveSimulatedPlayerStartHold(simulatedInteractable);
				break;
			case InteractableUseType.HoldFailed:
				ReceiveSimulatedPlayerFailedHold(simulatedInteractable);
				break;
			case InteractableUseType.HoldFinish:
				ReceiveSimulatedPlayerFinishHold(simulatedInteractable, simulatedCloneGO);
				break;
			case InteractableUseType.MousePress:
				ReceiveSimulatedPlayerMousePress(simulatedInteractable);
				break;
			case InteractableUseType.MouseRelease:
				ReceiveSimulatedPlayerMouseRelease(simulatedInteractable);
				break;
		}
	}

	private void ReceiveClockUpdate(ClockNetworkData data)
	{
		_dayTimeManager.SetTimeForAllClocks(data.Time / 60, data.Time % 60);
		_dayTimeManager.SetSunLight();
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
				ReceiveCustomerSpawn((CustomerNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.ClockUpdate:
				ReceiveClockUpdate((ClockNetworkData)photonEvent.CustomData);
				break;

		}
	}
}

