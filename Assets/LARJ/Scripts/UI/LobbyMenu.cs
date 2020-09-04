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
		_roomListingsMenu.ClearListings();
		PhotonNetwork.LeaveLobby();
	}
}
