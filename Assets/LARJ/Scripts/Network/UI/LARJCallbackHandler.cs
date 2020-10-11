using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LARJCallbackHandler : MonoBehaviourPunCallbacks
{
	[SerializeField] private UIHandler _uiHandler;

	public delegate void LARJOnRoomCreatedEvent();
	public event LARJOnRoomCreatedEvent LARJOnRoomCreated;

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		//Show error code for disconnect on other screen
		Debug.Log("Disconnected from server: " + cause.ToString());
	}

	public override void OnJoinedLobby()
	{
		//Show lobby screen
		_uiHandler.EnterNetworkSection();
	}

	public override void OnCreatedRoom()
	{
		LARJOnRoomCreated?.Invoke();
		Debug.Log("Created room successfully. " + this);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("Failed creation of room. " + message + this);
		PhotonNetwork.JoinLobby();
		Debug.Log("Joined Lobby");
	}
}
