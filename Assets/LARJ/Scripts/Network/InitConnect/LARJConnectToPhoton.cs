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
	public delegate void LARJNetworkStatusHandler(LARJNetworkState state);
	public event LARJNetworkStatusHandler LARJNetworkStatusEvent;

	//Called with a LARJNetworkStatus. Invokes LARJNetworkStatusEvent.
	public void SwitchToNetworkState(LARJNetworkState state)
	{
		switch(state)
		{
			case LARJNetworkState.Local:
				PhotonNetwork.Disconnect();
				LARJNetworkStatusEvent?.Invoke(LARJNetworkState.Local);
				break;

			case LARJNetworkState.Photon:
				PhotonNetwork.NickName = MasterManager.Instance.GameSettings.NickName;
				PhotonNetwork.GameVersion = MasterManager.Instance.GameSettings.GameVersion;
				PhotonNetwork.ConnectUsingSettings();
				LARJNetworkStatusEvent?.Invoke(LARJNetworkState.Photon);
				break;
		}
	}

    public override void OnConnectedToMaster()
	{
		Debug.Log($"OnConnectedToMaster was called. Connected with Nick: {PhotonNetwork.LocalPlayer.NickName}");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected from server: " + cause.ToString());
	}
}
