using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLobbyMenu : MonoBehaviourPunCallbacks
{

	//private RoomListingsMenu _roomListingsMenu;

	[SerializeField] private GameObject _joinOrCreateRoomMenu;
	[SerializeField] private RoomListingsMenu _roomListingsMenu;
	[SerializeField] private GameObject _mainMenu;

	public void OnClick_LeaveLobby()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.JoinedLobby))
			return;
		LeaveLobby();
		ChangeUI();
	}

	private void LeaveLobby()
	{
		PhotonNetwork.LeaveLobby();
		Debug.Log("Left lobby and cleared room list");
	}

	private void ChangeUI()
	{
		_joinOrCreateRoomMenu.SetActive(false);
		_mainMenu.SetActive(true);
	}

	public override void OnLeftRoom()
	{
		Debug.Log("Left Room");
	}
}
