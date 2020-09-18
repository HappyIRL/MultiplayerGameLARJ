using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool _customerPool;
    [SerializeField] float spawnTimer = 5f;
    [SerializeField] bool randomizeSpawnTime = false;
    [SerializeField] float spawnAmount;
    [SerializeField] Transform customerSpawnPoint;

    [SerializeField] CustomerManager _cm;


    IEnumerator Start()
    {
        var count = 0;
        while (count < spawnAmount)
        {
            count++;

            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();
            customer.CM = _cm;
            go.transform.position = customerSpawnPoint.position;

            if (randomizeSpawnTime)
            {
                var delay = Random.Range(0.1f, spawnTimer);
                yield return new WaitForSeconds(delay);
            }
            else
            {

                yield return new WaitForSeconds(spawnTimer);
            }
        }
    }
}
