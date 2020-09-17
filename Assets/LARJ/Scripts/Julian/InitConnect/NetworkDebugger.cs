using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
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

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnSceneLoad()
	{
		GameObject go = new GameObject();
		go.name = "NetworkDebuggerGameObject";
		go.hideFlags = HideFlags.HideAndDontSave;
		go.AddComponent<NetworkDebugger>();

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
			GUI.Box(new Rect(10, 10, 300, 60), $"NetworkClientState: {PhotonNetwork.NetworkClientState}" +
				$"{Environment.NewLine}Ping: {PhotonNetwork.GetPing()}" +
				$"{Environment.NewLine}MasterClient : {PhotonNetwork.MasterClient}");
			GUI.Box(new Rect(10, 800, 600, 15 * _debugList.Count + 5), _debugString);

			switch (PhotonNetwork.NetworkClientState)
			{
				case ClientState.Joined:
					CreateGUIButton(new Rect(10, 80, 300, 20), "Leave Room", () => PhotonNetwork.LeaveRoom());
					break;

				case ClientState.JoinedLobby:
					CreateGUIButton(new Rect(10, 80, 300, 20), "Leave Lobby", () => PhotonNetwork.LeaveLobby());
					_roomName = GUI.TextField(new Rect(160, 110, 150, 20), _roomName);
					CreateGUIButton(new Rect(10, 110, 150, 20), "Join Room:", () => PhotonNetwork.JoinRoom(_roomName));
					GUI.Box(new Rect(10, 200, 300, 15 * _roomCount + 5), _roomInfos);
					break;

				case ClientState.ConnectedToMasterServer:
					CreateGUIButton(new Rect(10, 80, 300, 20), "Disconnect from Master", () => PhotonNetwork.Disconnect());
					CreateGUIButton(new Rect(10, 110, 300, 20), "Join Lobby", () => PhotonNetwork.JoinLobby());
					break;

				case ClientState.Disconnected:
					CreateGUIButton(new Rect(10, 80, 300, 20), "Connect to Master", () => PhotonNetwork.ConnectUsingSettings());
					break;
			}
		}
	}
}

