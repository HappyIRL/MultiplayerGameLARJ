using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using System;

public class FilterRooms : MonoBehaviour
{
    public static RoomInfo[] FilterLobbyRooms( RoomInfo[] src, bool includeFull, Hashtable properties)
    {
        // use where expression to filter rooms
        return src.Where(room =>
           (
           filterRoom(room, includeFull, properties)
           )
           ).ToArray();
    }

    private static bool filterRoom(RoomInfo src, bool includeFull, Hashtable properties)
    {
        bool includesFull = (src.PlayerCount >= src.MaxPlayers || includeFull);

        bool includesProps = true;

        if (properties != null)
        {
            foreach(object key in properties)
            {
                if (!src.CustomProperties.ContainsKey(key))
                {
                    includesProps = false;
                    break;
                }

                if(src.CustomProperties[key] != properties[key])
                {
                    includesProps = false;
                    break;
                }
            }
        }
        return includesFull && includesProps;
    }
}
