using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum LARJNetworkState
{
	Local,
	Photon
}

public class LARJConnectToPhoton : MonoBehaviourPunCallbacks
{

	//Called with a LARJNetworkStatus. Invokes LARJNetworkStatusEvent.
	private void Start()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public void SwitchToNetworkState(LARJNetworkState state)
	{
		switch(state)
		{
			case LARJNetworkState.Local:
				PhotonNetwork.Disconnect();
				break;

			case LARJNetworkState.Photon:
				PhotonNetwork.NickName = MasterManager.Instance.GameSettings.NickName;
				PhotonNetwork.GameVersion = MasterManager.Instance.GameSettings.GameVersion;
				PhotonNetwork.ConnectUsingSettings();
				break;
		}
	}

    public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
		Debug.Log($"OnConnectedToMaster was called. Connected with Nick: {PhotonNetwork.LocalPlayer.NickName}");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected from server: " + cause.ToString());
	}
}
