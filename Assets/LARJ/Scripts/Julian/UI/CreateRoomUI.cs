using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateRoomUI : MonoBehaviour, IMatchmakingCallbacks
{
	[SerializeField] private TextMeshProUGUI _roomName;
	[SerializeField] private byte _maxPlayersInRoom = 4;
	[SerializeField] private TextMeshProUGUI _placeHolder;

	public void OnClick_CreateRoom()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.JoinedLobby))
			return;

		RoomOptions options = new RoomOptions
		{
			MaxPlayers = _maxPlayersInRoom
		};

		PhotonNetwork.CreateRoom(_roomName.text, options, TypedLobby.Default);
	}

	public void OnCreatedRoom()
	{
		Debug.Log("Created room successfully. " + this);
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("Failed creation of room. " + message + this);
		PhotonNetwork.JoinLobby();
		Debug.Log("Joined Lobby");
	}

	public void OnFriendListUpdate(List<FriendInfo> friendList) { }
	public void OnJoinedRoom() { }
	public void OnJoinRandomFailed(short returnCode, string message) { }
	public void OnJoinRoomFailed(short returnCode, string message) { }
	public void OnLeftRoom() { }
}