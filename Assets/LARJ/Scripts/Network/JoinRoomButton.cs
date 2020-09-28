using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
	public string RoomName = "";

    public void OnClick_JoinRoom()
	{
		PhotonNetwork.JoinRoom(RoomName);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log($"Successfully joined room; Name: {RoomName}");
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		throw new System.NotImplementedException();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log($"OnJoinRoomFailed; Code: {returnCode}; Message: {message}");
	}
}
