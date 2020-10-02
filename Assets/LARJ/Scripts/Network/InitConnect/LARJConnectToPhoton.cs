using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum LARJNetworkStatus
{
	Local,
	Photon
}

public class LARJConnectToPhoton : MonoBehaviourPunCallbacks
{
	public delegate void LARJNetworkSwitchHandler(LARJNetworkStatus status);
	public event LARJNetworkSwitchHandler LARJNetworkStatusEvent;

	//Called with a LARJNetworkStatus. Invokes LARJNetworkStatusEvent.
	public void SwitchToNetworkState(LARJNetworkStatus status)
	{
		switch(status)
		{
			case LARJNetworkStatus.Local:
				PhotonNetwork.Disconnect();
				LARJNetworkStatusEvent?.Invoke(LARJNetworkStatus.Local);
				break;

			case LARJNetworkStatus.Photon:
				PhotonNetwork.NickName = MasterManager.Instance.GameSettings.NickName;
				PhotonNetwork.GameVersion = MasterManager.Instance.GameSettings.GameVersion;
				PhotonNetwork.ConnectUsingSettings();
				LARJNetworkStatusEvent?.Invoke(LARJNetworkStatus.Photon);
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
