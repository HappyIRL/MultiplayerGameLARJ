using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

	public bool _networkingEnabled = false;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnSceneLoad()
	{
		GameObject go = new GameObject();
		go.name = "NetworkDebuggerGameObject";
		go.hideFlags = HideFlags.HideInHierarchy;
		go.AddComponent<NetworkDebugger>();
	}

	private void Start()
	{
		var x = FindObjectsOfType<LARJConnectToPhoton>();
		if (x.Length > 1) { Debug.LogError($"Found more than one of {x}"); }
		if(x == null) { Debug.LogError($"Can't find the LARJConnectTophoton.cs"); }

		else { _larjConnectToPhoton = x[0]; }
	}

	private void Update()
	{
		if(Keyboard.current[Key.F3].wasPressedThisFrame)
		{
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
		_debugList.Add(bug);
		_debugString = "";
		foreach(string debug in _debugList)
		{
			_debugString += $"ERROR: {debug}{Environment.NewLine}";
		}
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		AddToDebugList(message);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		AddToDebugList(message);
	}

	private void OnGUI()
	{
		if(_toggleDebugUI)
		{
			GUI.color = Color.black;

			//NetworkClientState
			GUI.Box(new Rect(10, 10, 300, 60), $"NetworkClientState: {PhotonNetwork.NetworkClientState}" +
				$"{Environment.NewLine}Ping: {PhotonNetwork.GetPing()}" +
				$"{Environment.NewLine}MasterClient : {PhotonNetwork.MasterClient}");
			
			//Network debug window
			GUI.Box(new Rect(10, 800, 600, 15 * _debugList.Count + 5), _debugString);

			switch (PhotonNetwork.NetworkClientState)
			{
				//In room
				case ClientState.Joined:
					//Leave room button
					CreateGUIButton(new Rect(10, 80, 300, 20), "Leave Room", () => PhotonNetwork.LeaveRoom());
					break;
				
				//In Lobby
				case ClientState.JoinedLobby:
					//Leave lobby button
					CreateGUIButton(new Rect(10, 80, 300, 20), "Leave Lobby", () => PhotonNetwork.LeaveLobby());
					//Room name enter field
					_roomName = GUI.TextField(new Rect(160, 110, 150, 20), _roomName);
					//Room enter button
					RoomOptions options = new RoomOptions{ MaxPlayers = _maxPlayersInRoom };
					CreateGUIButton(new Rect(10, 110, 150, 20), "Join/Create Room:", () => PhotonNetwork.JoinOrCreateRoom(_roomName, options, TypedLobby.Default));
					//Room info list
					GUI.Box(new Rect(10, 200, 300, 15 * _roomCount + 5), _roomInfos);
					break;

				//Connected to Photon
				case ClientState.ConnectedToMasterServer:
					//Disconnect button
					CreateGUIButton(new Rect(10, 80, 300, 20), "Disconnect from Master", () => _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkStatus.Local));
					//Join lobby button
					CreateGUIButton(new Rect(10, 110, 300, 20), "Join Lobby", () => PhotonNetwork.JoinLobby());
					break;

				//Disconnected from Photon
				case ClientState.Disconnected:
					//Connect button
					CreateGUIButton(new Rect(10, 80, 300, 20), "Connect to Master", () => PhotonNetwork.ConnectUsingSettings());
					break;
				case ClientState.PeerCreated:
					//Enable Networking
					CreateGUIButton(new Rect(10, 80, 300, 20), "Switch Networking On", () => _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkStatus.Photon));
					break;
			}
		}
	}
}

