using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IQueueUpdateNotifier
    {
        void OnQueueUpdated();

        void OnEnqueuedToQueue();

        void OnLeftQueue();

    }
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance;

    [Header ("Desks")]
    [SerializeField] private List<Transform> _deskWaypoints;
    public List<Transform> DeskWaypoints { get { return _deskWaypoints; } }
    [SerializeField] private int _activeDesks;
    private Dictionary<Transform, bool> _deskKvps = new Dictionary<Transform, bool>();
    public Dictionary<Transform, bool> DeskKvps { get { return _deskKvps; } }

    [Header("Queue")]
    [SerializeField] private Transform _queueStart;
    private Queue<GameObject> _customerQueue = new Queue<GameObject>();
    public Queue<GameObject> CustomerQueue { get { return _customerQueue; } }
    private List<Vector3> _queueWaypoints = new List<Vector3>();
    public List<Vector3> QueueWaypoints { get { return _queueWaypoints; } }




    private void Awake()
    {
        instance = this;
        
        for (int i = 0; i < _deskWaypoints.Count; i++)
        {
            if (i < _activeDesks)
            {
                _deskKvps.Add(_deskWaypoints[i], true);
            }
            else
            {
                _deskKvps.Add(_deskWaypoints[i], false);
            }
        }

        _queueWaypoints.Add(_queueStart.position);
    }
    public void EnqueueCustomer(GameObject go)
    {
        var notifiers = go.GetComponents<IQueueUpdateNotifier>();
        
        _customerQueue.Enqueue(go);
        
        foreach (var notifier in notifiers)
        {
            notifier.OnEnqueuedToQueue();
        }

        if (_customerQueue.Count == _queueWaypoints.Count)
        {
            var prevPos = _queueWaypoints[_customerQueue.Count - 1];
            _queueWaypoints.Add(new Vector3(prevPos.x + 1.5f, prevPos.y, prevPos.z));
        }

        
    }
    public void DequeueCustomer()
    {
        if (_customerQueue.Count>0)
        {
            var dequeuedCustomer = _customerQueue.Dequeue();
            var notifiers = dequeuedCustomer.GetComponents<IQueueUpdateNotifier>();

            foreach (var notifier in notifiers)
            {
                notifier.OnLeftQueue();
            }
            var queuedObjects = _customerQueue.ToArray();

            foreach (var queuedObject in queuedObjects)
            {
                var updateNotifiers = queuedObject.GetComponents<IQueueUpdateNotifier>();
                foreach (var updateNotifier in updateNotifiers)
                {
                    updateNotifier.OnQueueUpdated();
                }
            }
        }               
    }
    public void LeftDesk(Transform deskWp)
    {
        _deskKvps[deskWp] = true;
    }
    public void JoinedDesk(Transform deskWp)
    {
        _deskKvps[deskWp] = false;
    }


}
