using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomerSpawner : MonoBehaviour
{
    [Header("Wave System")]
    // SPAWN OPTIONS
    [SerializeField] int noOfWaves = 1;
    [SerializeField] int waveCooldown = 1;

    [Header("Customer")]
    [SerializeField] private ObjectPool _customerPool;
    [SerializeField] int noOfCustomers = 1;
    [SerializeField] [Range(1, 2)] float perWaveMultiplier = 1;

    // Customer Spawn rate
    private int _spawnTimer = 3;

    //[SerializeField] float patienceTimer = 10;
    //[SerializeField] [Range(0, 1)] float patienceShrinkage = 1;

    [Header("Spawn Waypoints")]
    [SerializeField] Transform _spawnPoint;
    [SerializeField] Transform _despawnPoint;
    public Transform DespawnPoint { get { return _despawnPoint; } }

    private bool _isLocal = true;
    public delegate void LARjCustomerSpawnEvent(GameObject go);
    public event LARjCustomerSpawnEvent OnCustomerSpawn;

    IEnumerator Start()
    {
        if (PhotonNetwork.IsConnected)
            _isLocal = false;
        else
            _isLocal = true;


        for (int i = 0; i < noOfWaves; i++)
        {
            var currentNoOfCustomers = Mathf.RoundToInt(noOfCustomers * perWaveMultiplier * (i + 1));

            var count = 0;

            while (count < currentNoOfCustomers)
            {
                yield return new WaitForSeconds(_spawnTimer);
                SpawnCustomer();
                count++;
            }
            yield return new WaitForSeconds(waveCooldown);
        }

    }


    public GameObject SpawnNetworkedCustomer()
    {
        var go = _customerPool.GetObject();
        var customer = go.GetComponent<Customer>();
        int i = UnityEngine.Random.Range(0, 3);
        switch (i)
        {
            case 0:
                customer.InteractionType = InteractionType.Hold;
                break;
            case 1:
                customer.InteractionType = InteractionType.Press;
                break;
            case 2:
                customer.InteractionType = InteractionType.PressTheCorrectKeys;
                break;
            default:
                break;
        }
        go.transform.position = _spawnPoint.position;

        return go;
    }

    private void SpawnCustomer()
    {
        if (_isLocal || PhotonNetwork.IsMasterClient)
        {
            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();
            int i = UnityEngine.Random.Range(0, 3);
            switch (i)
            {
                case 0:
                    customer.InteractionType = InteractionType.Hold;
                    break;
                case 1:
                    customer.InteractionType = InteractionType.Press;
                    break;
                case 2:
                    customer.InteractionType = InteractionType.PressTheCorrectKeys;
                    break;
                default:
                    break;
            }

            customer.despawn = _despawnPoint;

            go.transform.position = _spawnPoint.position;

            OnCustomerSpawn?.Invoke(go);
        }
    }
}
