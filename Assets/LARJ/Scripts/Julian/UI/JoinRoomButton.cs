using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomButton : MonoBehaviour, IMatchmakingCallbacks
{
	public string RoomName = "";

    public void OnClick_JoinRoom()
	{
		PhotonNetwork.JoinRoom(RoomName);
	}

	public void OnJoinedRoom()
	{
		Debug.Log($"Successfully joined room; Name: {RoomName}");
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
		throw new System.NotImplementedException();
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log($"OnJoinRoomFailed; Code: {returnCode}; Message: {message}");
	}

	public void OnLeftRoom() { }
	public void OnCreatedRoom() { }
	public void OnCreateRoomFailed(short returnCode, string message) { }
	public void OnFriendListUpdate(List<FriendInfo> friendList) { }
}
