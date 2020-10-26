using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnRate = 1f;
    [SerializeField] private List<Transform> _spawnPoints = null;
    [SerializeField] private List<ObjectPool> _vehiclePools = null;

    private float _timeToSpawn;
    private float _timer;

    void Awake()
    {
        _timeToSpawn = 1 / _spawnRate;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _timeToSpawn)
        {
            _timer = 0f;

            SpawnVehicle();
        }
    }

    private void SpawnVehicle()
    {
        GameObject obj = _vehiclePools[Random.Range(0, _vehiclePools.Count)].GetObject();

        Transform spawn = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        obj.transform.position = spawn.position;
        obj.transform.rotation = spawn.rotation;
        obj.transform.SetParent(transform);

        obj.GetComponent<Vehicle>().StartVehicle();
    }
}
