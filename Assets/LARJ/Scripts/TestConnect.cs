using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
	[SerializeField] private string _gameVersion = "0.0.1";

	private void Start()
	{
		PhotonNetwork.GameVersion = _gameVersion;	// Says in documentation to either "set PhotonNetwork.GameVersion just AFTER 
													// calling PhotonNetwork.ConnectUsingSettings()" or "set PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion 
													// BEFORE calling PhotonNetwork.ConnectUsingSettings()"
		PhotonNetwork.ConnectUsingSettings();
	}

	  
    public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster was called. " + this);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		// some error occured, 'cause' is an enum of error
		Debug.Log("Disconnected from server: " + cause.ToString());
	}  
	
}
