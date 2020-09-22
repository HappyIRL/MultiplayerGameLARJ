using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement Wave options
public class CustomerSpawner : MonoBehaviour
{
    
    
    // SPAWN OPTIONS
    [SerializeField] float delayInSecs = 5f;
    [SerializeField] bool randomizeSpawnTime = false;
    [SerializeField] float spawnAmount;
    [HideInInspector] Transform customerSpawnPoint;
    [HideInInspector] public Transform customerDespawnPoint;

    // CUSTOMER OPTIONS
    [SerializeField] public float patienceTimer;


    // ENUM WAVE? 

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
        var count = 0;
        while (count < spawnAmount)
        {
           

            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();
       
            customer.customerSpawner = this;
            customer.customerPool = _customerPool;            

            go.transform.position = customerSpawnPoint.position;

            if (randomizeSpawnTime)
            {
                var delay = Random.Range(0.1f, delayInSecs);
                yield return new WaitForSeconds(delay); 
                count++;
            }
            else
            {

                yield return new WaitForSeconds(delayInSecs);
                count++;
            }
        }
    }
}
