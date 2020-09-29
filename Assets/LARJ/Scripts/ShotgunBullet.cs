using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PooledObject))]
public class ShotgunBullet : MonoBehaviour, IObjectPoolNotifier
{
    public ObjectPool ShotgunBulletPool = null;
    [SerializeField] private float _lifeTime = 2f;

    [HideInInspector] public Rigidbody Rb = null;
    private Coroutine _lastCoroutine;

    private void Awake()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
        ShotgunBulletPool.ReturnObject(gameObject);
    }

    public void OnEnqueuedToPool()
    {

    }

    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (created)
        {
            ShotgunBulletPool = gameObject.GetComponent<PooledObject>()._pool;
            Rb = GetComponent<Rigidbody>();
        }

        _lastCoroutine = StartCoroutine(BulletLifeCoroutine());    
    }
    private IEnumerator BulletLifeCoroutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        ShotgunBulletPool.ReturnObject(gameObject);
    }
}
