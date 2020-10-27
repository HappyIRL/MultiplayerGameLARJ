using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cracker : MonoBehaviour, IObjectPoolNotifier 
{
    public ObjectPool crackerPool;
    public int timeToCrack;

    private int _timer = 0;

    
    public void OnCreatedOrDequeuedFromPool(bool created)
    {
    }
    public void OnEnqueuedToPool()
    {
        StopCoroutine(Start());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            crackerPool.ReturnObject(this.gameObject);
        }
    }

    private IEnumerator Start()
    {
        while (_timer <= timeToCrack)
        {
            yield return new WaitForSeconds(1);
            _timer++;
        }
        // Takes money if complete
    }
}
