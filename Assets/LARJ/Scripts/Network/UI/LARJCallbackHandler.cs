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
		_uiHandler.EnterNetworkSection();
	}

	public override void OnCreatedRoom()
	{
		_uiHandler.EnableConnectingDialog(false);
		LARJOnRoomCreated?.Invoke();
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		_uiHandler.UpdatePlayerList();
	}
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		_uiHandler.UpdatePlayerList();
	}

	public override void OnJoinedRoom()
	{
		_uiHandler.EnableConnectingDialog(false);
		_uiHandler.WaitingRoomJoined();
		_uiHandler.UpdatePlayerList();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		_uiHandler.EnableConnectingDialog(false);
		PhotonNetwork.JoinLobby();
	}
}
