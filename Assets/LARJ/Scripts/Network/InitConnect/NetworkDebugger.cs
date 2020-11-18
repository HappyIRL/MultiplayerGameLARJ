using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class NetworkDebugger : MonoBehaviourPunCallbacks
{

	private bool _toggleDebugUI;
	private string _roomName;
	private string _roomInfos;
	private int _roomCount;
	private string _debugString;
	private List<String> _debugList = new List<String>();
	private byte _maxPlayersInRoom = 4;
	private LARJConnectToPhoton _larjConnectToPhoton;
	GUIStyle _debugBoxSytle = new GUIStyle();
	GUIContent _content = new GUIContent();
	private int index = 0;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnSceneLoad()
	{
		GameObject go = new GameObject();
		go.name = "NetworkDebuggerGameObject";
		//go.hideFlags = HideFlags.HideInHierarchy;
		go.AddComponent<NetworkDebugger>();
		DontDestroyOnLoad(go);
	}

	private void Start()
	{
		_larjConnectToPhoton = FindObjectOfType<LARJConnectToPhoton>();
		_debugBoxSytle.alignment = TextAnchor.MiddleLeft;
		_debugBoxSytle.wordWrap = true;
		_debugBoxSytle.fontSize = 15;
	}

	private void Update()
	{
		if(Keyboard.current[Key.F3].wasPressedThisFrame)
		{
			_larjConnectToPhoton = FindObjectOfType<LARJConnectToPhoton>();
			_toggleDebugUI = !_toggleDebugUI;
		}
	}

	public void CreateGUIButton(Rect rect, string x, Action action)
	{
		if (GUI.Button(rect, x))
		{
			action.Invoke();
		}
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		_roomInfos = "";
		_roomCount = roomList.Count;
		foreach (RoomInfo info in roomList)
		{
			_roomInfos += $"{info.Name} {info.PlayerCount}/{info.MaxPlayers} Joinable: {info.IsOpen}{Environment.NewLine}";
		}
	}

	private void AddToDebugList(string bug)
	{
		if (_debugList.Count > 10)
		{
			_debugList.RemoveRange(0, _debugList.Count / 2);
		}

		_debugList.Add(index + ". " + bug);
		_debugString = "";
		foreach (string debug in _debugList)
		{
			_debugString = $"{debug}{Environment.NewLine}{_debugString}";
			_content.text = _debugString;
		}
		index++;
	}

	private string GetMasterClient()
	{
		if (PhotonNetwork.IsMasterClient)
			return "Me, myself and I";
		else
		{
			if (PhotonNetwork.MasterClient == null)
				return "Not Connected";
			else
				return PhotonNetwork.MasterClient.ToString();
		}
	}

	#region Photon-Event-Callbacks
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		AddToDebugList(message);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		AddToDebugList(message);
	}

	public override void OnConnected()
	{
		base.OnConnected();
		AddToDebugList("Connected");
	}
	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
		AddToDebugList("Created room");
	}
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		AddToDebugList("Joined room");
	}
	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		AddToDebugList("Connected to master");
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		AddToDebugList("Disconnected - cause: " + cause.ToString());
	}
	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		AddToDebugList("Joined lobby");
	}

	public override void OnLeftLobby()
	{
		base.OnLeftLobby();
		AddToDebugList("Left lobby");
	}
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		AddToDebugList("Left room");
	}
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		base.OnJoinRandomFailed(returnCode, message);
		AddToDebugList("Join random failed - message: " + message);
	}
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		AddToDebugList(otherPlayer + " left room");
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		AddToDebugList(newPlayer + " entered room");
	}
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		AddToDebugList(newMasterClient + " is the new master");
	}
	public override void OnRegionListReceived(RegionHandler regionHandler)
	{
		base.OnRegionListReceived(regionHandler);
	}
	#endregion Photon-Event-Callbacks

	private void OnGUI()
	{
		GUI.color = Color.white;

		if (_toggleDebugUI)
		{
			GUI.Box(new Rect(1600, 10, 300, 60), $"NetworkClientState: {PhotonNetwork.NetworkClientState}" +
			$"{Environment.NewLine}Ping: {PhotonNetwork.GetPing()}" +
			$"{Environment.NewLine}MasterClient : {GetMasterClient()}");
			//Network debug window
			GUI.Box(new Rect(1300, 80, 300, _debugBoxSytle.fontSize * _debugList.Count + 5), _content, _debugBoxSytle);

			switch (PhotonNetwork.NetworkClientState)
			{
				//In room
				case ClientState.Joined:
					//Leave room button
					CreateGUIButton(new Rect(1600, 80, 300, 20), "Leave Room", () => PhotonNetwork.LeaveRoom());
					break;
				
				//In Lobby
				case ClientState.JoinedLobby:
					//Leave lobby button
					CreateGUIButton(new Rect(1600, 80, 300, 20), "Leave Lobby", () => PhotonNetwork.LeaveLobby());
					//Room name enter field
					_roomName = GUI.TextField(new Rect(1760, 110, 150, 20), _roomName);
					//Room enter button
					RoomOptions options = new RoomOptions{ MaxPlayers = _maxPlayersInRoom };
					CreateGUIButton(new Rect(1600, 110, 150, 20), "Join/Create Room:", () => PhotonNetwork.JoinOrCreateRoom(_roomName, options, TypedLobby.Default));
					//Room info list
					GUI.Box(new Rect(1600, 200, 300, 15 * _roomCount + 5), _roomInfos);
					break;

				//Connected to Photon
				case ClientState.ConnectedToMasterServer:
					//Disconnect button
					CreateGUIButton(new Rect(1600, 80, 300, 20), "Disconnect from Master", () => _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Local));
					//Join lobby button
					CreateGUIButton(new Rect(1600, 110, 300, 20), "Join Lobby", () => PhotonNetwork.JoinLobby());
					break;

				//Disconnected from Photon
				case ClientState.Disconnected:
					//Connect button
					CreateGUIButton(new Rect(1600, 80, 300, 20), "Connect to Master", () => PhotonNetwork.ConnectUsingSettings());
					break;
				case ClientState.PeerCreated:
					//Enable Networking
					CreateGUIButton(new Rect(1600, 80, 300, 20), "Switch Networking On", () => _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Photon));
					break;
			}
		}
	}
}

