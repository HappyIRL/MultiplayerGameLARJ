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

    private Queue<GameObject> _inactiveObjects = new Queue<GameObject>();

    public GameObject GetObject()
    {
        if(_inactiveObjects.Count > 0)
        {
            var dequeuedObject = _inactiveObjects.Dequeue();

            dequeuedObject.transform.parent = null;
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
        go.transform.parent = this.transform;

        _inactiveObjects.Enqueue(go);
    }
}