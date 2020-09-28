using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTest : MonoBehaviour, IObjectPoolNotifier
{
    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        Debug.Log("Object dequeued from pool");
        StartCoroutine(DoReturnAfterDelay());
    }

    public void OnEnqueuedToPool()
    {
        Debug.Log("Enqueued to pool");
    }

    IEnumerator DoReturnAfterDelay()
    {
        yield return new WaitForSeconds(10.0f);

        gameObject.ReturnToPool();
    }
}
