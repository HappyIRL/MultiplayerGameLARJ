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
				break;

			case LARJNetworkState.Photon:
				PhotonNetwork.NickName = MasterManager.Instance.GameSettings.NickName;
				PhotonNetwork.GameVersion = MasterManager.Instance.GameSettings.GameVersion;
				PhotonNetwork.ConnectUsingSettings();
				break;
		}
	}
}
