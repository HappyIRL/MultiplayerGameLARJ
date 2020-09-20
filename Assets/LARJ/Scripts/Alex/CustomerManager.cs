using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform[] queueWaypoints;
    public bool[] isFreeAtIndex;

    public Transform deskWaypoint;
    public bool deskIsFree = true;

    public Transform despawn;
        

    void Start()
    {
        isFreeAtIndex.Length.Equals(queueWaypoints.Length);
        for (int i = 0; i < queueWaypoints.Length; i++)
        {
            isFreeAtIndex[i].Equals(true);
        }
    }

    

}
