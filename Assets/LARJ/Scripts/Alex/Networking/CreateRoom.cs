using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateRoom : MonoBehaviour
{
    private void OnGUI()
    {
        // if we are not inside the room let the player create a room

        if(PhotonNetwork.CurrentRoom == null)
        {

            // if create button room clicked
            {
                // create a room
                PhotonNetwork.CreateRoom("RoomNameHere", new Photon.Realtime.RoomOptions() { MaxPlayers = 4 }, null);

            }
        }
        else
        {
            // connected to room, display info



            // if disconnect button is clicked
            { PhotonNetwork.LeaveRoom(); }
        }
    }

}
