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
	ClockUpdate = 132,
	InstantiateOnMaster = 133,
	InstantiateOnOther = 134,
	SyncInteractablesFromMaster = 135,
	NotifyMasterOnSceneLoad = 136

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
	private GameObject[] _players = new GameObject[4];
	private List<Interactable> _interactables = new List<Interactable>();
	private List<GameObject> _syncOnStartGOs = new List<GameObject>();


	public GameObject HealthbarCanvasPrefab;
	private LARJNetworkID _myID = 0;
	private LARJNetworkID _simulatedID = (LARJNetworkID)4;
	private GameObject _simulatedPlayerGO = null;
	private GameObject _myPlayer;
	private CustomerSpawner _customerSpawner;
	private Dictionary<int, GameObject> _instanceIDs = new Dictionary<int, GameObject>();

	//_uniqueCustomerID == 0 returns the object in scene.
	private int _uniqueInstanceID = 0;
	private DayTimeManager _dayTimeManager;

	private void Awake()
	{
		PhotonNetwork.AddCallbackTarget(this);
		_customerSpawner = FindObjectOfType<CustomerSpawner>();
	}

	private void Start()
	{
		_myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		_myPlayer = GetPlayerFromID(_myID);
		PlayerInteraction playerInteraction = _myPlayer.GetComponent<PlayerInteraction>();

		_customerSpawner.OnCustomerSpawn += OnCustomerSpawn;
		playerInteraction.LARJInteractableUse += RaiseNetworkedInteractable;
		playerInteraction.OnNetworkTaskEvent += RaiseNetworkedTask;
		_dayTimeManager = FindObjectOfType<DayTimeManager>();

		if (!PhotonNetwork.IsMasterClient)
			RaiseOnSceneLoad();
	}

	private void Update()
	{
		UpdateLocalPlayerController();
		if (PhotonNetwork.IsMasterClient)
		{
			UpdateLocalTime();
		}
	}
	#region Private - Methods

	private void OnCustomerSpawn(GameObject go, InteractionType type)
	{
		RaiseNetworkedCustomer(AddInstanceToObjectLists(go), type);
	}

	private int AddInstanceToObjectLists(GameObject go)
	{
		_uniqueInstanceID += 1;

		_instanceIDs.Add(_uniqueInstanceID, go);
		go.GetComponentInChildren<Interactable>().UniqueInstanceID = _uniqueInstanceID;

		return _uniqueInstanceID;
	}

	private void AddInstanceToObjectList(GameObject go, int id)
	{
		if (!_instanceIDs.ContainsKey(id))
		{
			_instanceIDs.Add(id, go);
			go.GetComponentInChildren<Interactable>().UniqueInstanceID = id;
		}
	}
	private GameObject GetInteractableGOFromID(int objectInstanceID)
	{
		if (_instanceIDs.ContainsKey(objectInstanceID))
		{
			return _instanceIDs[objectInstanceID];
		}
		return null;
	}

	private GameObject GetInteractableSceneGOFromID(InteractableObjectID prefabID)
	{
		switch (prefabID)
		{
			case InteractableObjectID.Broom:
				return _interactables[0].gameObject;
			case InteractableObjectID.Telephone1:
				return _interactables[1].gameObject;
			case InteractableObjectID.Telephone2:
				return _interactables[2].gameObject;
			case InteractableObjectID.FireExtinguisher:
				return _interactables[3].gameObject;
			case InteractableObjectID.PC:
				return _interactables[4].gameObject;
			case InteractableObjectID.Printer:
				return _interactables[5].gameObject;
			case InteractableObjectID.Shotgun:
				return _interactables[6].gameObject;
			case InteractableObjectID.WaterCooler:
				return _interactables[7].gameObject;
			case InteractableObjectID.CleaningSpray:
				return _interactables[8].gameObject;
			case InteractableObjectID.Money:
				return _interactables[9].transform.parent.gameObject;
			case InteractableObjectID.Money2:
				return _interactables[10].transform.parent.gameObject;
			case InteractableObjectID.Mug:
				return _interactables[11].transform.parent.gameObject;
			case InteractableObjectID.Mug2:
				return _interactables[12].transform.parent.gameObject;
			case InteractableObjectID.Mug3:
				return _interactables[13].transform.parent.gameObject;
			case InteractableObjectID.Mug4:
				return _interactables[14].transform.parent.gameObject;
			case InteractableObjectID.Stamp:
				return _interactables[15].transform.parent.gameObject;
			case InteractableObjectID.Stamp2:
				return _interactables[16].transform.parent.gameObject;
			case InteractableObjectID.Paper:
				return _interactables[17].gameObject;
			default:
				return null;
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

	private InteractableObjectID GetInteractableIDOfGameObject(GameObject prefabGO)
	{
		Interactable interactable = prefabGO.GetComponentInChildren<Interactable>();
		return interactable.InteractableID;
	}

	#endregion Private - Methods

	#region Private - RaiseNetworkEvents

	private void RaiseOnSceneLoad()
	{

		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};

		ClientNetworkData clientNetworkData = new ClientNetworkData()
		{
			ID = (byte)_myID
		};

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.NotifyMasterOnSceneLoad, clientNetworkData, raiseEventOptions, sendOptions);
	}

	private void RaiseOnStartInteractablePositions(GameObject go, byte objectID)
	{
		Vector3 position = go.transform.position;
		Vector3 rotation = go.transform.eulerAngles;
		Vector3 localScale = go.transform.localScale;

		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};

		InteractableTransformNetworkData interactableTransform = new InteractableTransformNetworkData()
		{
			Position = position,
			Rotation = rotation,
			LocalScale = localScale,
			ObjectIndex = objectID
		};

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.SyncInteractablesFromMaster, interactableTransform, raiseEventOptions, sendOptions);
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

		if (_myPlayer != null)
		{
			ClientNetworkData clientNetworkData = new ClientNetworkData()
			{
				ID = (byte)_myID,
				Position = _myPlayer.transform.position,
				Rotation = _myPlayer.transform.Find("BaseCharacter").eulerAngles
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

		ClockNetworkData clockNetworkData = new ClockNetworkData()
		{
			Time = time
		};
		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.ClockUpdate, clockNetworkData, raiseEventOptions, sendOptions);
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
		TaskNetworkData taskNetworkData = new TaskNetworkData()
		{
			ID = (byte)_myID,
			TaskState = (byte)state,
			InteractableID = (byte)id,
			ObjectInstanceID = objectInstanceID
		};
		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.TaskUpdate, taskNetworkData, raiseEventOptions, sendOptions);
	}

	private void RaiseNetworkedCustomer(int _uniqueInstanceID, InteractionType type)
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
			Type = type
		};

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.CustomerSpawn, interactableNetworkData, raiseEventOptions, sendOptions);
	}

	private void RaiseNetworkedInteractable(InteractableUseType type, int objectInstanceID, InteractableObjectID itemInHandID)
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

		GameObject interactable_go = GetInteractableGOFromID(objectInstanceID);

		if (interactable_go != null)
		{
			InteractableNetworkData interactableNetworkData = new InteractableNetworkData()
			{
				ID = (byte)_myID,
				InteractableUseID = (byte)type,
				ItemInHandID = (byte)itemInHandID,
				ObjectInstanceID = objectInstanceID

			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.InteractableUpdate, interactableNetworkData, raiseEventOptions, sendOptions);
		}
	}

	private void RaiseOnInstantiate(InteractableObjectID id, Vector3? position, Quaternion? rotation, Vector3 localScale, LARJNetworkEvents instantiationType, int uniqueInstanceID)
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
			UniqueInstanceID = uniqueInstanceID,
			LocalScale = localScale
		};

		PhotonNetwork.RaiseEvent((byte)instantiationType, notMasterClientInstantiateData, raiseEventOptions, sendOptions);

	}

	#endregion Private - RaiseNetworkEvents

	#region Private - ReceiveEvents

	private void ReceiveUpdatePC(ClientNetworkData data)
	{
		if (_simulatedID != (LARJNetworkID)data.ID)
		{
			_simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
			_simulatedID = (LARJNetworkID)data.ID;
		}

		if (_simulatedPlayerGO != null)
		{
			_simulatedPlayerGO.transform.position = data.Position;
			_simulatedPlayerGO.GetComponent<PlayerCharacterAppearance>()._baseCharacter.transform.eulerAngles = data.Rotation;
		}
	}

	private void ReceiveSimulatedPlayerPickUp(GameObject simulatedInteractableGO, Interactable simulatedInteractable, GameObject simulatedPlayerObjectHolder)
	{
		simulatedInteractable.PickUpObject(simulatedPlayerObjectHolder.transform, simulatedInteractableGO);
	}

	private void ReceiveSimulatedPlayerDrop(Interactable simulatedInteractable)
	{
		simulatedInteractable.DropObject();

		if (simulatedInteractable.CanInteractWhenPickedUp)
		{
			simulatedInteractable.DisablePickedUpButtonHints();
		}
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

	private void ReceiveSimulatedPlayerFinishHold(Interactable simulatedInteractable, InteractableObjectID itemInHand)
	{
		if(itemInHand == InteractableObjectID.None)
			simulatedInteractable.OnNetworkHoldingFinishedEvent();
	}

	private void ReceiveSimulatedPlayerMousePress(Interactable simulatedInteractable)
	{
		simulatedInteractable.MousePressEvent();
	}

	private void ReceiveSimulatedPlayerMouseRelease(Interactable simulatedInteractable)
	{
		simulatedInteractable.MouseReleaseEvent();
	}

	private void ReceivePressTheCorrectKeysFinished(Interactable simulatedInteractable)
	{
		simulatedInteractable.PressTheCorrectKeysFinishedEvent();
	}

	private void ReceivePressTheCorrectKeysFailed(Interactable simulatedInteractable)
	{
		simulatedInteractable.PressTheCorrectKeysFailedEvent();
	}

	private void ReceiveTaskEvent(TaskNetworkData data)
	{
		Interactable interactable = GetInteractableGOFromID(data.ObjectInstanceID).GetComponentInChildren<Interactable>();
		Task task = interactable.GetComponentInChildren<Task>();
		TaskManagerUI taskManagerUI = TaskManager.TaskManagerSingelton.TaskManagerUI;
		Score score = TaskManager.TaskManagerSingelton.Score;

		switch ((LARJTaskState)data.TaskState)
		{
			case LARJTaskState.TaskComplete:
				if (AllowedInteractables.Instance.Interactables.Contains(interactable))
					if (!interactable.AlwaysInteractable)
						AllowedInteractables.Instance.Interactables.Remove(interactable);

				task.IsTaskActive = false;
				task.StopTask();
				taskManagerUI.RemoveUITask(task.TaskUI);
				score.UpdateScore(task.GetRewardMoney, true);
				break;

			case LARJTaskState.TaskFailed:
				if (AllowedInteractables.Instance.Interactables.Contains(interactable))
					if (!interactable.AlwaysInteractable)
						AllowedInteractables.Instance.Interactables.Remove(interactable);
				task.IsTaskActive = false;
				task.StopTask();
				taskManagerUI.RemoveUITask(task.TaskUI);
				score.UpdateScore(task.GetLostMoneyOnFail, false);
				break;

			case LARJTaskState.TaskStart:
				if (!AllowedInteractables.Instance.Interactables.Contains(interactable))
					AllowedInteractables.Instance.AddInteractable(interactable);
				task.TaskUI = taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
				task.IsTaskActive = true;
				task.StartTask();
				break;
		}
	}

	private void ReceiveCustomerSpawn(CustomerNetworkData data)
	{
		AddInstanceToObjectList(_customerSpawner.SpawnNetworkedCustomer(data.Type), data.UniqueInstanceID);
	}

	private void ReceiveInteractableEvent(InteractableNetworkData data)
	{
		GameObject simulatedPlayerGO = GetPlayerFromID((LARJNetworkID)data.ID);
		GameObject simulatedPlayerObjectHolder = simulatedPlayerGO.GetComponentInChildren<PlayerCharacterAppearance>()._objectHolder;
		InteractableObjectID itemInHand = (InteractableObjectID)data.ItemInHandID;
		Interactable simulatedInteractable = GetInteractableGOFromID(data.ObjectInstanceID).GetComponentInChildren<Interactable>();
		GameObject simulatedInteractableGO = simulatedInteractable.gameObject;


		InteractableUseType type = (InteractableUseType)data.InteractableUseID;

		switch (type)
		{
			case InteractableUseType.Drop:
				ReceiveSimulatedPlayerDrop(simulatedInteractable);
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
				ReceiveSimulatedPlayerFinishHold(simulatedInteractable, itemInHand);
				break;
			case InteractableUseType.MousePress:
				ReceiveSimulatedPlayerMousePress(simulatedInteractable);
				break;
			case InteractableUseType.MouseRelease:
				ReceiveSimulatedPlayerMouseRelease(simulatedInteractable);
				break;
			case InteractableUseType.PressTheCorrectKeysFinished:
				ReceivePressTheCorrectKeysFinished(simulatedInteractable);
				break;
			case InteractableUseType.PressTheCorrectKeysFailed:
				ReceivePressTheCorrectKeysFailed(simulatedInteractable);
				break;
		}
	}

	private void ReceiveClockUpdate(ClockNetworkData data)
	{
		_dayTimeManager.SetTimeForAllClocks(data.Time / 60, data.Time % 60);
		_dayTimeManager.SetLights();
	}

	private void ReceiveInstantiateOnOther(NotMasterClientInstantiateData data)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableSceneGOFromID((InteractableObjectID)data.ID), true);
		AddInstanceToObjectList(instanceGO, data.UniqueInstanceID);

		if (data.Position != null)
		{
			instanceGO.transform.position = (Vector3)data.Position;
			instanceGO.transform.rotation = (Quaternion)data.Rotation;
			instanceGO.transform.localScale = data.LocalScale;
		}
	}

	private void ReceiveInstantiateOnMaster(NotMasterClientInstantiateData data)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableSceneGOFromID((InteractableObjectID)data.ID), true);
			instanceGO.transform.localScale = data.LocalScale;
			int uniqueInstanceID = AddInstanceToObjectLists(instanceGO);

			if (data.Position != null)
			{
				instanceGO.transform.position = (Vector3)data.Position;
				instanceGO.transform.rotation = (Quaternion)data.Rotation;
			}

			RaiseOnInstantiate((InteractableObjectID)data.ID, data.Position, data.Rotation, data.LocalScale, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);

			if((InteractableObjectID)data.ID == InteractableObjectID.Paper) TaskManager.TaskManagerSingelton.StartTask(instanceGO.GetComponentInChildren<Task>());
		}

	}

	private void ReceiveInteractableTransformOfMasterClient(InteractableTransformNetworkData data)
	{
		GameObject goToSync = _syncOnStartGOs[data.ObjectIndex];
		goToSync.transform.position = data.Position;
		goToSync.transform.eulerAngles = data.Rotation;
		goToSync.transform.localScale = data.LocalScale;
	}

	public void OnEvent(EventData photonEvent)
	{
		LARJNetworkEvents eventCode = (LARJNetworkEvents)photonEvent.Code;

		switch (eventCode)
		{
			case LARJNetworkEvents.PCUpdate:
				ReceiveUpdatePC((ClientNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.InteractableUpdate:
				ReceiveInteractableEvent((InteractableNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.TaskUpdate:
				ReceiveTaskEvent((TaskNetworkData)photonEvent.CustomData);
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
			case LARJNetworkEvents.SyncInteractablesFromMaster:
				ReceiveInteractableTransformOfMasterClient((InteractableTransformNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.NotifyMasterOnSceneLoad:
				for (int i = 0; i < _syncOnStartGOs.Count; i++)
				{
					RaiseOnStartInteractablePositions(_syncOnStartGOs[i], (byte)i);
				}
				break;

		}
	}

	#endregion ReceiveEvents

	#region Public - Methods

	public void SetInteractables(List<Interactable> interactables, List<GameObject> syncOnStartGOs)
	{
		_interactables = interactables;
		_syncOnStartGOs = syncOnStartGOs;

		foreach (Interactable interactable in interactables)
		{
			AddInstanceToObjectLists(interactable.gameObject);
		}
	}

	public void SetPlayers(GameObject[] go)
	{
		_players = go;
	}
	#endregion Public - Methods

	#region Public - OnNotMasterClientInstantiate
	public void OnNotMasterClientInstantiate(GameObject prefabGO)
	{
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), null, null, prefabGO.transform.localScale, LARJNetworkEvents.InstantiateOnMaster, (int)prefabGO.GetComponentInChildren<Interactable>().InteractableID);
	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), position, rotation, prefabGO.transform.localScale, LARJNetworkEvents.InstantiateOnMaster, (int)prefabGO.GetComponentInChildren<Interactable>().InteractableID);
	}

	public GameObject OnMasterClientInstantiate(GameObject prefabGO)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(prefabGO, true);
		int uniqueInstanceID = AddInstanceToObjectLists(instanceGO);
		RaiseOnInstantiate(GetInteractableIDOfGameObject(instanceGO), null, null, instanceGO.transform.localScale, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
		return instanceGO;
	}

	public GameObject OnMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(prefabGO, position, rotation, true);
		int uniqueInstanceID = AddInstanceToObjectLists(instanceGO);
		RaiseOnInstantiate(GetInteractableIDOfGameObject(instanceGO), position, rotation, instanceGO.transform.localScale, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
		return instanceGO;
	}

	#endregion Public - OnNotMasterClientInstantiate

}
