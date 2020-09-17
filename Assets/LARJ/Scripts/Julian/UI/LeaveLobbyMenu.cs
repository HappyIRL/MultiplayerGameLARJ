using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLobbyMenu : MonoBehaviour, IInRoomCallbacks
{
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
		Debug.Log("Left lobby");
	}

	private void ChangeUI()
	{
		_joinOrCreateRoomMenu.SetActive(false);
		_mainMenu.SetActive(true);
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log("Left Room");
	}

	public void OnPlayerEnteredRoom(Player newPlayer){}
	public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged){}
	public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps){}
	public void OnMasterClientSwitched(Player newMasterClient){}
}
