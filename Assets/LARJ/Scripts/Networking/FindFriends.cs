using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindFriends : MonoBehaviour
{
    public string[] Friends = new string[0];

    void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "PlayerNameHere";

        PhotonNetwork.FindFriends(Friends);
    }
    
    private void OnGUI()
    {
        if (!PhotonNetwork.IsConnected) return;

        
    }
   
}
