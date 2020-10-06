using Photon.Pun;
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

    [HideInInspector] public Transform customerSpawnPoint;
    [HideInInspector] public Transform customerDespawnPoint;

    public delegate void LARjCustomerSpawnEvent(GameObject go);
    public event LARjCustomerSpawnEvent OnCustomerSpawn;

    private ObjectPool _customerPool;
    private bool _isLocal = true;

    // CUSTOMER OPTIONS



    // References
    [HideInInspector] public List <Transform> queueWaypoints;
    [HideInInspector] public List<bool> isFreeAtIndex;
    
    [HideInInspector] public Transform deskWaypoint;
    [HideInInspector] public bool deskIsFree = true;

    [SerializeField] private HighlightInteractables _highlightInteractables = null;

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
        LARJConnectToPhoton larjConnectToPhoton = FindObjectOfType<LARJConnectToPhoton>();
        larjConnectToPhoton.LARJNetworkStatusEvent += OnLARJNetworkStatusChange;

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

	private void OnLARJNetworkStatusChange(LARJNetworkState state)
	{
		switch(state)
		{
            case LARJNetworkState.Local:
                _isLocal = true;
                break;
            case LARJNetworkState.Photon:
                _isLocal = false;
                break;
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

    public GameObject SpawnNetworkedCustomer()
	{
        var go = _customerPool.GetObject();
        var customer = go.GetComponent<Customer>();

        customer.customerSpawner = this;
        customer.customerPool = _customerPool;

        go.transform.position = customerSpawnPoint.position;

        if (_highlightInteractables != null)
        {
            _highlightInteractables.AddInteractables(go.GetComponent<Interactable>());
        }

        return go;
	}

    private void SpawnCustomer()
    {
        if(_isLocal || PhotonNetwork.IsMasterClient)
		{
            var go = _customerPool.GetObject();
            var customer = go.GetComponent<Customer>();

            customer.customerSpawner = this;
            customer.customerPool = _customerPool;

            go.transform.position = customerSpawnPoint.position;

            if (_highlightInteractables != null)
            {
                _highlightInteractables.AddInteractables(go.GetComponent<Interactable>());
            }
            OnCustomerSpawn?.Invoke(go);
            Debug.Log("Spawned Customer, should only happen on MasterClient");
        }
    }
}
