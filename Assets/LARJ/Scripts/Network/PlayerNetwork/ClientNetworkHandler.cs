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
	CustomerSpawn = 131,
	ClockUpdate = 132,
	InstantiateOnMaster = 133,
	InstantiateOnOther = 134

}

public enum LARJNetworkID
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
		none = 4
	}

public enum LARJParentID
{
	None = 199
}

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject[] _players = new GameObject[4];
	[SerializeField] private List<GameObject> _interactables = new List<GameObject>();

	private PlayerInteraction _playerInteraction;
	private GameObject _playerFromID;
	private LARJNetworkID _myID = 0;
	private LARJNetworkID _simulatedID = (LARJNetworkID)4;
	private GameObject _simulatedPlayerGO = null;
	private GameObject _myPlayer;
	private CustomerSpawner _customerSpawner;
	private Dictionary<int, GameObject> _instanceIDs = new Dictionary<int, GameObject>();
	//_uniqueCustomerID == 0 returns the object in scene.
	private int _uniqueInstanceID = 0;
	private DayTimeManager _dayTimeManager;

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

	#region Private - Methods
	private void SubscribeToEvents()
	{
		_playerFromID = GetPlayerFromID((LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1);
		_customerSpawner = FindObjectOfType<CustomerSpawner>();
		_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
		_customerSpawner.OnCustomerSpawn += OnCustomerSpawn;
		_playerInteraction.OnNetworkTaskEvent += RaiseNetworkedTask;
		_playerInteraction.LARJInteractableUse += RaiseNetworkedInteractable;
	}

	private void OnCustomerSpawn(GameObject go)
	{
		AddInstanceToObjectList(go);
		RaiseNetworkedCustomer(_uniqueInstanceID);
	}

	private int AddInstanceToObjectList(GameObject go)
	{
		_uniqueInstanceID += 1;

		_instanceIDs.Add(_uniqueInstanceID, go);
		go.GetComponent<Interactable>().UniqueInstanceID = _uniqueInstanceID;
		return _uniqueInstanceID;
	}

	private void AddInstanceToObjectList(GameObject go, int id)
	{
		_instanceIDs.Add(id, go);
		go.GetComponent<Interactable>().UniqueInstanceID = id;
	}

	private GameObject GetInteractableGOFromID(InteractableObjectID id, int objectInstanceID)
	{
		if (_instanceIDs.ContainsKey(objectInstanceID))
		{
			Debug.Log("Returned Object: " + _instanceIDs[objectInstanceID] + " with the ID of: " + objectInstanceID);
			return _instanceIDs[objectInstanceID];
		}

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
				Debug.Log("No Customer prefab");
				break;
			case InteractableObjectID.None:
				Debug.Log("Object ID is none");
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

	private InteractableObjectID GetInteractableIDOfGameObject(GameObject prefabGO)
	{
		Debug.Log(prefabGO.ToString());
		Interactable interactable = prefabGO.GetComponent<Interactable>();
		return interactable.InteractableID;
	}

	private Transform GetParentTransformFromID(LARJParentID id)
	{
		switch(id)
		{
			default:
				return null;
		}
	}

	#endregion Private - Methods

	#region Private - RaiseNetworkEvents

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

		if (_playerFromID != null)
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

	private void RaiseNetworkedTask(InteractableObjectID id, LARJTaskState state, int objectInstanceID)
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

		GameObject interactable_go = GetInteractableGOFromID(id, objectInstanceID);

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

	private void RaiseOnInstantiate(InteractableObjectID id, Vector3? position, Quaternion? rotation, LARJNetworkEvents instantiationType, int unigueInstanceID)
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

		NotMasterClientInstantiateData notMasterClientInstantiateData = new NotMasterClientInstantiateData
		{
			ID = (byte)id,
			Position = position,
			Rotation = rotation,
			UniqueInstanceID = unigueInstanceID
		};

		PhotonNetwork.RaiseEvent((byte)instantiationType, notMasterClientInstantiateData, raiseEventOptions, sendOptions);

	}

	#endregion Private - RaiseNetworkEvents

	#region Private - ReceiveEvents

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
		Debug.Log("Received ID: " + data.ObjectInstanceID);
		Interactable interactable = GetInteractableGOFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID).GetComponent<Interactable>();
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
				_playerInteraction.AllowedInteractibles.Add(GetInteractableGOFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID).GetComponent<Interactable>());
				task.TaskUI = taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
				break;
		}
	}

	private void ReceiveCustomerSpawn(CustomerNetworkData data)
	{
		var go = _customerSpawner.SpawnNetworkedCustomer();
		go.GetComponent<Interactable>().UniqueInstanceID = _uniqueInstanceID;
		if (!_instanceIDs.ContainsKey(data.UniqueInstanceID))
		{
			_instanceIDs.Add(data.UniqueInstanceID, go);
		}
	}

	private void ReceiveInteractableUpdate(InteractableNetworkData data)
	{
		GameObject simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
		GameObject simulatedPlayerObjectHolder = simulatedPlayerGO.GetComponent<SimulatedPlayer>()._objectHolder;
		GameObject simulatedInteractableGO = GetInteractableGOFromID((InteractableObjectID)data.InteractableID, data.ObjectInstanceID);
		Interactable simulatedInteractable = simulatedInteractableGO.GetComponent<Interactable>();
		GameObject simulatedCloneGO = GetInteractableGOFromID((InteractableObjectID)data.ItemInHandID, 0);


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

	private void ReceiveInstantiateOnOther(NotMasterClientInstantiateData data)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableGOFromID((InteractableObjectID)data.ID, 0));
		AddInstanceToObjectList(instanceGO, data.UniqueInstanceID);

		if (data.Position != null)
		{
			instanceGO.transform.position = (Vector3)data.Position;
			instanceGO.transform.rotation = (Quaternion)data.Rotation;
		}
	}

	private void ReceiveInstantiateOnMaster(NotMasterClientInstantiateData data)
	{
		if(PhotonNetwork.IsMasterClient)
		{
			GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableGOFromID((InteractableObjectID)data.ID, 0));
			int uniqueInstanceID = AddInstanceToObjectList(instanceGO);

			if(data.Position != null)
			{
				instanceGO.transform.position = (Vector3)data.Position;
				instanceGO.transform.rotation = (Quaternion)data.Rotation;
			}

			RaiseOnInstantiate((InteractableObjectID)data.ID, data.Position, data.Rotation, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
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
				ReceiveCustomerSpawn((CustomerNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.ClockUpdate:
				ReceiveClockUpdate((ClockNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.InstantiateOnMaster:
				ReceiveInstantiateOnMaster((NotMasterClientInstantiateData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.InstantiateOnOther:
				ReceiveInstantiateOnOther((NotMasterClientInstantiateData)photonEvent.CustomData);
				break;

		}
	}

	#endregion ReceiveEvents

	#region Public - Methods

	public void SetInteractables(List<GameObject> go)
	{
		_interactables = go;
	}

	public void AddInteractable(GameObject go)
	{
		_interactables.Add(go);
	}

	public void SetPlayers(GameObject[] go)
	{
		_players = go;
	}

	public void SetPlayerInteractionInstance(PlayerInteraction pi)
	{
		_playerInteraction = pi;
	}

	#endregion Public - Methods

	#region Public - OnNotMasterClientInstantiate
	public void OnNotMasterClientInstantiate(GameObject prefabGO)
	{
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), null, null, LARJNetworkEvents.InstantiateOnMaster, 0);
	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation, LARJNetworkEvents raiseType)
	{
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), position, rotation, raiseType, 0);
	}

	#endregion Public - OnNotMasterClientInstantiate
}
