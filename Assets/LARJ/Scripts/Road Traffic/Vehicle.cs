using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PooledObject))]
public class Vehicle : MonoBehaviour, IObjectPoolNotifier
{
    [HideInInspector] public ObjectPool VehiclePool = null;
    [SerializeField] private float _speed = 5f;

    private Vector3 _targetPosition;

    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (created)
        {
            VehiclePool = gameObject.GetComponent<PooledObject>()._pool;
        }
    }

    public void OnEnqueuedToPool()
    {
        
    }
    public void StartVehicle()
    {
        _targetPosition = transform.position + transform.forward * 50f;
        StartCoroutine(MoveVehicle());
    }

    private void ReturnVehicleToPool()
    {
        VehiclePool.ReturnObject(gameObject);
    }
    private IEnumerator MoveVehicle()
    {
        while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
            yield return null;
        }

        ReturnVehicleToPool();
    }
}
