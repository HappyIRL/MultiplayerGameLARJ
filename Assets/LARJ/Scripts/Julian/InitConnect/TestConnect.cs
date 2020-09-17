using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviour, IConnectionCallbacks
{
	private void Start()
	{
		PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
		PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
		PhotonNetwork.ConnectUsingSettings();
	}
	  
    public void OnConnectedToMaster()
	{
		Debug.Log($"OnConnectedToMaster was called. Connected with Nick: {PhotonNetwork.LocalPlayer.NickName}");
	}

	public void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected from server: " + cause.ToString());
	}

	public void OnConnected() { }
	public void OnRegionListReceived(RegionHandler regionHandler) { }
	public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
	public void OnCustomAuthenticationFailed(string debugMessage) { }
}
