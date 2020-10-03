using Photon.Pun;
using UnityEngine;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
	public string RoomName { get; private set; }

    public void OnClick_JoinRoom()
	{
		PhotonNetwork.JoinRoom(RoomName);
	}

	public void SetRoomName(string s)
	{
		RoomName = s;
	}
}
