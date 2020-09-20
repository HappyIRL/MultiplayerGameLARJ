using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement Wave options
public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool _customerPool;
    [SerializeField] CustomerManager _cm;

    // SPAWN OPTIONS
    [SerializeField] float spawnTimer = 5f;
    [SerializeField] bool randomizeSpawnTime = false;
    [SerializeField] float spawnAmount;
    [SerializeField] Transform customerSpawnPoint;

    // CUSTOMER OPTIONS
    [SerializeField] public float patienceTimer;


    IEnumerator Start()
    {
        var count = 0;
        while (count < spawnAmount)
        {
           

            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();
            customer.CM = _cm;
            customer.customerSpawner = this;
            customer.customerPool = _customerPool;            

            go.transform.position = customerSpawnPoint.position;

            if (randomizeSpawnTime)
            {
                var delay = Random.Range(0.1f, spawnTimer);
                yield return new WaitForSeconds(delay); 
                count++;
            }
            else
            {

                yield return new WaitForSeconds(spawnTimer);
                count++;
            }
        }
    }
}
