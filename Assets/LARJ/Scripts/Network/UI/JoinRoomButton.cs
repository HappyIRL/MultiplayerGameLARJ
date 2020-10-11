using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
	public string RoomName { get; private set; }

	private UIHandler _uiHandler;

	private void Start()
	{
		_uiHandler = FindObjectOfType<UIHandler>();
	}
	public void OnClick_JoinRoom()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.JoinedLobby))
			return;

		_uiHandler.EnableConnectingDialog(true);
		PhotonNetwork.JoinRoom(RoomName);
	}

	public void SetRoomName(string s)
	{
		RoomName = s;
	}
}
