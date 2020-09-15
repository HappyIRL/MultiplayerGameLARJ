using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLobbyMenu : MonoBehaviour
{

	//private RoomListingsMenu _roomListingsMenu;

	[SerializeField] private GameObject _joinOrCreateRoomMenu;
	[SerializeField] private RoomListingsMenu _roomListingsMenu;
	[SerializeField] private GameObject _mainMenu;

	public void OnClick_LeaveLobby()
	{
		//Julian: Ich denke das diese Reference bzw das aufrufen der ClearListings() Methode hier nicht gut in das Bild des OOP passt.
		_roomListingsMenu.ClearListings();

		if(PhotonNetwork.InRoom)
			PhotonNetwork.LeaveRoom();

		PhotonNetwork.LeaveLobby();
		_joinOrCreateRoomMenu.SetActive(false);
		gameObject.SetActive(false);
		_mainMenu.SetActive(true);
		Debug.Log("Left Lobby");
	}
}
