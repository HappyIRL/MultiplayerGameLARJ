using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CustomerSpawner : MonoBehaviour
{

    // SPAWN OPTIONS
    [SerializeField] int noOfWaves = 1;
    [SerializeField] int waveCooldown;
    [SerializeField] float spawnsPerWave;
    [SerializeField] float spawnCooldown = 5f;
    [SerializeField] bool randomizeSpawnTime = false;

    [SerializeField] public float patienceTimer;

    [HideInInspector] Transform customerSpawnPoint;
    [HideInInspector] public Transform customerDespawnPoint;
    private ObjectPool _customerPool;

    // CUSTOMER OPTIONS

        

    // References
    [HideInInspector] public List <Transform> queueWaypoints;
    [HideInInspector] public List<bool> isFreeAtIndex;
    
    [HideInInspector] public Transform deskWaypoint;
    [HideInInspector] public bool deskIsFree = true;

    void Awake()
    {
        customerSpawnPoint = GameObject.Find("CustomerSpawnPoint").GetComponent<Transform>();
        _customerPool = GameObject.Find("CustomerPool").GetComponent<ObjectPool>();
        deskWaypoint = GameObject.Find("DeskWaypoint").GetComponent<Transform>();
        var queueList =  GameObject.Find("QueueWaypoints").GetComponent<Transform>();
        foreach (Transform queueWaypoint in queueList)
        {
            queueWaypoints.Add(queueWaypoint);
            isFreeAtIndex.Add(true);
        }
        customerDespawnPoint = GameObject.Find("CustomerDespawn").GetComponent<Transform>();       
    }


    IEnumerator Start()
    {
        for (int i = 0; i < noOfWaves; i++)
        {
            var count = 0;
            while (count < spawnsPerWave)
            {
                SpawnCustomer();

                yield return StartCoroutine(DoRandomizeSpawnTime());
                count++;
            }
            yield return new WaitForSeconds(waveCooldown);
        }
        
    }

    private IEnumerator DoRandomizeSpawnTime()
    {
        if (randomizeSpawnTime)
        {
            var delay = UnityEngine.Random.Range(0.1f, spawnCooldown);
            yield return new WaitForSeconds(delay);
              
        }
        else
        {

            yield return new WaitForSeconds(spawnCooldown);
               
        }
    }

    private void SpawnCustomer()
    {
        var go = _customerPool.GetObject();
        var customer = go.GetComponent<Customer>();

        customer.customerSpawner = this;
        customer.customerPool = _customerPool;

        go.transform.position = customerSpawnPoint.position;
    }
}
