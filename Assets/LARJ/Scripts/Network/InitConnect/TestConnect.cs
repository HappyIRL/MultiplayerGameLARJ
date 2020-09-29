using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
	[SerializeField] private NetworkDebugger _networkDebugger;
	private void Start()
	{
		_networkDebugger = FindObjectOfType<NetworkDebugger>();

		if (_networkDebugger != null)
			_networkDebugger.NetworkEnabled += StartNetworking;
	}

	private void OnDestroy()
	{
		if (_networkDebugger != null)
			_networkDebugger.NetworkEnabled -= StartNetworking;
	}
	private void StartNetworking(bool enabled)
	{
		if(enabled)
		{
			PhotonNetwork.NickName = MasterManager.Instance.GameSettings.NickName;
			PhotonNetwork.GameVersion = MasterManager.Instance.GameSettings.GameVersion;
			PhotonNetwork.ConnectUsingSettings();
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
