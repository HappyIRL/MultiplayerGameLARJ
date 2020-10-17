using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPoolNotifier
{
    void OnEnqueuedToPool();

    void OnCreatedOrDequeuedFromPool(bool created);
}
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    private Queue<GameObject> InactiveObjects = new Queue<GameObject>();

    public GameObject GetObject()
    {
        if(InactiveObjects.Count > 0)
        {
            var dequeuedObject = InactiveObjects.Dequeue();

            dequeuedObject.transform.SetParent(null);
            dequeuedObject.SetActive(true);

            var notifiers = dequeuedObject.GetComponents<IObjectPoolNotifier>();
            
            foreach (var notifier in notifiers)
            {
                notifier.OnCreatedOrDequeuedFromPool(false);
            }

            return dequeuedObject;
        }
        else
        {
            //Don't go over the InstantiateManager at this place, since other code already prevents the non-masterclient from spawning this.
            var newObject = Instantiate(_prefab);
            var poolTag = newObject.GetComponent<PooledObject>();

            poolTag._pool = this;
            poolTag.hideFlags = HideFlags.HideInInspector;

            var notifiers = newObject.GetComponents<IObjectPoolNotifier>();
            foreach(var notifier in notifiers)
            {
                notifier.OnCreatedOrDequeuedFromPool(true);
            }
            return newObject;
        }
    }
    public void ReturnObject(GameObject go)
    {
        var notifiers = go.GetComponents<IObjectPoolNotifier>();

        foreach(var notifier in notifiers)
        {
            notifier.OnEnqueuedToPool();
        }

        go.SetActive(false);
        go.transform.SetParent(transform);

        InactiveObjects.Enqueue(go);
    }
}