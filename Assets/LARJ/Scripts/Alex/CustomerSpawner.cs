using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement Wave options
    public enum SpawnType
    {
        Wave,
        Single
    }
public class CustomerSpawner : MonoBehaviour
{

    // SPAWN OPTIONS
    [SerializeField] int waveCount = 1;
    [SerializeField] int timeBetweenWaves;
    [SerializeField] float delayInSecs = 5f;
    [SerializeField] bool randomizeSpawnTime = false;
    [SerializeField] float spawnAmount;
    [HideInInspector] Transform customerSpawnPoint;
    [HideInInspector] public Transform customerDespawnPoint;

    // CUSTOMER OPTIONS
    [SerializeField] public float patienceTimer;

        

    // References
    private ObjectPool _customerPool;
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
        for (int i = 0; i < waveCount; i++)
        {
            var count = 0;
            while (count < spawnAmount)
            {
                SpawnCustomer();

                yield return StartCoroutine(DoRandomizeSpawnTime());
                count++;
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        
    }

    private IEnumerator DoRandomizeSpawnTime()
    {
        if (randomizeSpawnTime)
        {
            var delay = UnityEngine.Random.Range(0.1f, delayInSecs);
            yield return new WaitForSeconds(delay);
              
        }
        else
        {

            yield return new WaitForSeconds(delayInSecs);
               
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
