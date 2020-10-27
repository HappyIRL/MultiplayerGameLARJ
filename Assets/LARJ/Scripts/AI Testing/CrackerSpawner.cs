using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackerSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool _crackerPool;
    [SerializeField] private Transform _spawnPoint;
    public void SpawnCracker()
    {        
        var go = _crackerPool.GetObject();
        var cracker = go.GetComponent<Cracker>();
                    
        go.transform.position = _spawnPoint.position;       
    }
}
