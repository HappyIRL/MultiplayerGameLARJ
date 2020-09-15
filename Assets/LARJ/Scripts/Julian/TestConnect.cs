using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
	[SerializeField] private string _gameVersion = "0.0.1";
	[SerializeField]
	private string _nickName = "Tom";
	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
		PhotonNetwork.GameVersion = _gameVersion;
		PhotonNetwork.NickName = _nickName;
	}

	  
    public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster was called. " + this);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected from server: " + cause.ToString());
	}  
	
}
