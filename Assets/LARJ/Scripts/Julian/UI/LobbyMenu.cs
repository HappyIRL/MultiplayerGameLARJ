using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{

	[SerializeField] private RoomListingsMenu _roomListingsMenu;

	public void JoinLobby()
	{
		PhotonNetwork.JoinLobby();
	}
	
	public void LeaveLobby()
	{
		//Julian: Ich denke das diese Reference bzw das aufrufen der ClearListings() Methode hier nicht gut in das Bild des OOP passt.
		_roomListingsMenu.ClearListings();
		PhotonNetwork.LeaveLobby();
	}
}
