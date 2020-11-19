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

    public event Action<GameObject, InteractionType, bool> OnCustomerSpawn;

    [SerializeField, Range(0, 10)] private int _difficulty = 1;
    public int Difficulty
    {
        get => _difficulty;
        set
        {
            _difficulty = value;
            SetSpawnCooldown();
        }
    }

    IEnumerator Start()
    {
        for (int i = 0; i < noOfWaves; i++)
        {
            var currentNoOfCustomers = Mathf.RoundToInt(noOfCustomers * perWaveMultiplier * (i + 1));

            var count = 0;

            while (count < currentNoOfCustomers)
            {
                if (_difficulty > 0)
                {
                    yield return new WaitForSeconds(_spawnTimer);
                    SpawnCustomer();
                }
                count++;
            }
            yield return new WaitForSeconds(waveCooldown);
        }

    }
    private void SetSpawnCooldown()
    {
        _spawnTimer = 11 - _difficulty;
    }


    public GameObject SpawnNetworkedCustomer(InteractionType type, bool wantsMoney)
    {
        var go = _customerPool.GetObject();
        var customer = go.GetComponent<Customer>();

        customer.InteractionType = type;

        customer.WantsMoney = wantsMoney;

        go.transform.position = _spawnPoint.position;

        customer.despawn = _despawnPoint;

        return go;
    }

    private void SpawnCustomer()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
        {
            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();
            int i = UnityEngine.Random.Range(0, 3);
            bool wantsMoney = false;
            switch (i)
            {
                case 0:
                    customer.InteractionType = InteractionType.Hold;
                    break;
                case 1:
                    customer.InteractionType = InteractionType.Press;
                    wantsMoney = UnityEngine.Random.value > 0.5;
                    break;
                case 2:
                    customer.InteractionType = InteractionType.PressTheCorrectKeys;
                    break;
                default:
                    break;
            }

            customer.despawn = _despawnPoint;

            go.transform.position = _spawnPoint.position;

            customer.WantsMoney = wantsMoney;

            OnCustomerSpawn?.Invoke(go, customer.InteractionType, wantsMoney);
        }
    }
}
