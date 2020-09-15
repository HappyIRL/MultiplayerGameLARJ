using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScreen : MonoBehaviourPunCallbacks
{ 
    Vector2 lobbyScroll = Vector2.zero;
    RoomInfo[] rooms;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Join Random", GUILayout.Width(200f)))
        {
            PhotonNetwork.JoinRandomRoom();
        }

        //Create new room
        if (GUILayout.Button("Create Room", GUILayout.Width(200f)))
        {
            PhotonNetwork.CreateRoom(PlayerPrefs.GetString("Username") + "'s Room", new Photon.Realtime.RoomOptions() { MaxPlayers = 4 }, null);
        }

        if(rooms == null || rooms.Length == 0)
        {
            GUILayout.Label("No Rooms available");
        }      
        else
        {
            lobbyScroll = GUILayout.BeginScrollView(lobbyScroll, GUILayout.Width(220f), GUILayout.ExpandHeight(true));

            foreach(RoomInfo room in rooms)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(200f));

                GUILayout.Label(room.Name + " - " + room.PlayerCount + "/" + room.MaxPlayers);

                if (GUILayout.Button("Enter"))
                {
                    PhotonNetwork.JoinRoom(room.Name);
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        // TODO Show friends list mgmt page        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom(PlayerPrefs.GetString("Username") + "'s Room", new Photon.Realtime.RoomOptions() { MaxPlayers = 4 }, null);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        rooms = roomList.ToArray();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        PhotonNetwork.LoadLevel("ChatRoom");
    }

}
