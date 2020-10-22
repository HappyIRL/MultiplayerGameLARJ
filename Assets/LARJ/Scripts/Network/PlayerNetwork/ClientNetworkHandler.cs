﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.UI;

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
	[SerializeField] private GameObject[] _players = new GameObject[4];
	[SerializeField] private List<Interactable> _interactables = new List<Interactable>();

	public GameObject HealthbarCanvasPrefab;
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
		PhotonNetwork.AddCallbackTarget(this);
		_myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber - 1;
		SubscribeToEvents();
		if (!PhotonNetwork.IsMasterClient)
			RaiseOnSceneLoad();
	}

	private void Update()
	{
		UpdateLocalPlayerController();
		if(PhotonNetwork.IsMasterClient)
		{
			UpdateLocalTime();
		}
	}
	#region Private - Methods
	private void SubscribeToEvents()
	{
		_playerFromID = GetPlayerFromID(_myID);
		_customerSpawner = FindObjectOfType<CustomerSpawner>();
		_playerInteraction = _playerFromID.GetComponent<PlayerInteraction>();
		_dayTimeManager = FindObjectOfType<DayTimeManager>();
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

	//Check all of these functions and if applied 0 check for what object needs to be in place
	private GameObject GetInteractableGOFromID(int objectInstanceID)
	{
		if (_instanceIDs.ContainsKey(objectInstanceID))
		{
			return _instanceIDs[objectInstanceID];
		}
		return null;
	}

	private GameObject GetInteractableGOFromID(InteractableObjectID prefabID)
	{
		switch(prefabID)
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
				return _interactables[9].gameObject;
			case InteractableObjectID.Money2:
				return _interactables[10].gameObject;
			case InteractableObjectID.Mug:
				return _interactables[11].gameObject;
			case InteractableObjectID.Mug2:
				return _interactables[12].gameObject;
			case InteractableObjectID.Mug3:
				return _interactables[13].gameObject;
			case InteractableObjectID.Mug4:
				return _interactables[14].gameObject;
			case InteractableObjectID.Stamp:
				return _interactables[15].gameObject;
			case InteractableObjectID.Stamp2:
				return _interactables[16].gameObject;
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
		Interactable interactable = prefabGO.GetComponent<Interactable>();
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

	private void RaiseOnStartInteractablePositions()
	{
		foreach(Interactable interactable in _interactables)
		{
			Vector3 position = interactable.gameObject.transform.position;
			Vector3 rotation = interactable.gameObject.transform.eulerAngles;
			Vector3 localScale = interactable.gameObject.transform.localScale;

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
				ObjectID = (byte)interactable.InteractableID
			};

			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.SyncInteractablesFromMaster, interactableTransform, raiseEventOptions, sendOptions);
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

		GameObject interactable_go = GetInteractableGOFromID(objectInstanceID);

		if(interactable_go != null)
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

	private void RaiseOnInstantiate(InteractableObjectID id, Vector3? position, Quaternion? rotation,Vector3 localScale, LARJNetworkEvents instantiationType, int uniqueInstanceID)
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
		if (simulatedCloneGO == null)
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

	private void ReceiveTaskUpdate(TaskNetworkData data)
	{
		_myPlayer = GetPlayerFromID(_myID);
		_playerInteraction = _myPlayer.GetComponent<PlayerInteraction>();
		Debug.Log(data.ObjectInstanceID);
		Debug.Log(GetInteractableGOFromID(data.ObjectInstanceID));
		Interactable interactable = GetInteractableGOFromID(data.ObjectInstanceID).GetComponent<Interactable>();
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
				_playerInteraction.AllowedInteractibles.Add(GetInteractableGOFromID(data.ObjectInstanceID).GetComponent<Interactable>());
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
		GameObject simulatedInteractableGO = GetInteractableGOFromID(data.ObjectInstanceID);
		Interactable simulatedInteractable = simulatedInteractableGO.GetComponent<Interactable>();
		GameObject simulatedCloneGO = GetInteractableGOFromID((InteractableObjectID)data.ItemInHandID);


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
		_dayTimeManager.SetLights();
	}

	private void ReceiveInstantiateOnOther(NotMasterClientInstantiateData data)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableGOFromID((InteractableObjectID)data.ID));
		AddInstanceToObjectList(instanceGO, data.UniqueInstanceID);
		instanceGO.transform.localScale = data.LocalScale;


		HighlightInteractables highlightInteractables = FindObjectOfType<HighlightInteractables>();
		GameObject healthbarCanvas = Instantiate(HealthbarCanvasPrefab);

		healthbarCanvas.transform.SetParent(instanceGO.transform);
		healthbarCanvas.transform.position = transform.position + Vector3.up;

		instanceGO.layer = LayerMask.NameToLayer("Garbage");
		Interactable interactable = instanceGO.GetComponent<Interactable>();
		Garbage garbage = instanceGO.AddComponent<Garbage>();

		highlightInteractables.AddInteractable(interactable);

		Image background = healthbarCanvas.transform.GetChild(0).GetComponent<Image>();
		Image healthbar = background.transform.GetChild(0).GetComponent<Image>();

		//garbage.SetHealthbarImages(healthbar, background);
		garbage.StrokesToClean = 3;
		interactable.EnableColliders();


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
			GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(GetInteractableGOFromID((InteractableObjectID)data.ID));
			instanceGO.transform.localScale = data.LocalScale;
			int uniqueInstanceID = AddInstanceToObjectList(instanceGO);

			if(data.Position != null)
			{
				instanceGO.transform.position = (Vector3)data.Position;
				instanceGO.transform.rotation = (Quaternion)data.Rotation;
			}

			RaiseOnInstantiate((InteractableObjectID)data.ID, data.Position, data.Rotation, data.LocalScale,LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
		}

	}

	private void ReceiveInteractableTransformOfMasterClient(InteractableTransformNetworkData data)
	{
		GameObject interactable = GetInteractableGOFromID((InteractableObjectID)data.ObjectID);
		interactable.transform.position = data.Position;
		interactable.transform.eulerAngles = data.Rotation;
		interactable.transform.localScale = data.LocalScale;
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
			case LARJNetworkEvents.SyncInteractablesFromMaster:
				ReceiveInteractableTransformOfMasterClient((InteractableTransformNetworkData)photonEvent.CustomData);
				break;
			case LARJNetworkEvents.NotifyMasterOnSceneLoad:
				RaiseOnStartInteractablePositions();
				break;

		}
	}

	#endregion ReceiveEvents

	#region Public - Methods

	public void SetInteractables(List<Interactable> go)
	{
		_interactables = go;
		foreach(Interactable interactable in _interactables)
		{
			AddInstanceToObjectList(interactable.gameObject);
		}
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
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), null, null, prefabGO.transform.localScale, LARJNetworkEvents.InstantiateOnMaster, (int)prefabGO.GetComponent<Interactable>().InteractableID);
	}

	public void OnNotMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), position, rotation, prefabGO.transform.localScale, LARJNetworkEvents.InstantiateOnMaster, (int)prefabGO.GetComponent<Interactable>().InteractableID);
	}

	public GameObject OnMasterClientInstantiate(GameObject prefabGO)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(prefabGO);
		int uniqueInstanceID = AddInstanceToObjectList(instanceGO);
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), null, null, instanceGO.transform.localScale, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
		return instanceGO;
	}

	public GameObject OnMasterClientInstantiate(GameObject prefabGO, Vector3 position, Quaternion rotation)
	{
		GameObject instanceGO = InstantiateManager.Instance.ForceLocalInstantiate(prefabGO, position, rotation);
		int uniqueInstanceID = AddInstanceToObjectList(instanceGO);
		RaiseOnInstantiate(GetInteractableIDOfGameObject(prefabGO), position, rotation, instanceGO.transform.localScale, LARJNetworkEvents.InstantiateOnOther, uniqueInstanceID);
		return instanceGO;
	}

	#endregion Public - OnNotMasterClientInstantiate

}
