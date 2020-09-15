using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connecting to Photon and getting a list of rooms
public class ConnectToPhoton : MonoBehaviourPunCallbacks
{
	public GameObject LobbyScreen;

	private string username = "";

	private bool connecting = false;
	private string error = null;
	[SerializeField] private string _gameVersion = "0.0.1";


	//List<RoomInfo> rooms;
	//   private bool includeFullRooms;
	//   private Hashtable filterRoomProperties;

	private void Start()
	{
		username = PlayerPrefs.GetString("Username", "");		
	}
	private void OnGUI()
	{
		if (connecting)
		{
			GUILayout.Label("Connecting...");
			return;
		}

		if (error != null)
		{
			GUILayout.Label("Failed to connect: " + error);
		}

		GUILayout.Label("Username");
		username = GUILayout.TextField(username, GUILayout.Width(200f));


		if (GUILayout.Button("Connect"))
		{
			PlayerPrefs.SetString("Username", username);

			connecting = true;

			PhotonNetwork.NickName = username;
			PhotonNetwork.GameVersion = _gameVersion;
			//PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "v.1.0";
			PhotonNetwork.ConnectUsingSettings();
		}
	}
    public override void OnConnected()
    {
        base.OnConnected();
		connecting = false;
		gameObject.SetActive(false);
		LobbyScreen.SetActive(true);
    }
	
	
	//public override void OnRoomListUpdate(List<RoomInfo> roomList)
	//{
	//	base.OnRoomListUpdate(roomList);
	//	rooms = roomList;

	//}
	public override void OnDisconnected(DisconnectCause cause)
	{
		// some error occured, 'cause' is an enum of error		
		connecting = false;
		error = cause.ToString();
	}
}
