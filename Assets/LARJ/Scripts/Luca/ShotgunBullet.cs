using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShotgunBullet : MonoBehaviour, IObjectPoolNotifier
{
    public ObjectPool ShotgunBulletPool = null;
    [SerializeField] private float _lifeTime = 2f;

    private Rigidbody _rb;
    private Coroutine _lastCoroutine;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
        _lastCoroutine = StartCoroutine(BulletLifeCoroutine());
        _rb.AddForce(transform.forward * 5f);      
    }
    private IEnumerator BulletLifeCoroutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        ShotgunBulletPool.ReturnObject(gameObject);
    }
}
