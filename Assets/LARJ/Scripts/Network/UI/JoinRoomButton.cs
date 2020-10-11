using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
	public string RoomName { get; private set; }

    public void OnClick_JoinRoom()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.JoinedLobby))
			return;

		PhotonNetwork.JoinRoom(RoomName);
	}

	public void SetRoomName(string s)
	{
		RoomName = s;
	}
}
