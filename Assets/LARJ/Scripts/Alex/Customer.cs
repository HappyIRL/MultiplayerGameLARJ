using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    StateMachine _stateMachine;
    public CustomerManager CM;
    NavMeshAgent _agent;
    public CustomerSpawner customerSpawner;
    public ObjectPool customerPool;

    private Transform _deskWaypoint;
    private Transform _despawn;
    //private List<Transform> _queueWaypoints;

    private float _range = 3f;
    private Transform _target;
    private int _queuePosition;

    private void Start()
    {
        // register statemachine and grab neccessary components
        _stateMachine = new StateMachine();
            //First state registered == initState
        var entering = _stateMachine.CreateState("entering");

        _agent = GetComponent<NavMeshAgent>();
        

        _deskWaypoint = CM.deskWaypoint;
        _despawn = CM.despawn;
        

       
        entering.onEnter = delegate 
        {
            Debug.Log("Customer Entering!");
            if (CM.deskIsFree) // Go get served
            {
                _stateMachine.TransitionTo("moveToDesk");                
            }            
        };
        entering.onFrame = delegate
        {
            for (int i = 0; i < CM.isFreeAtIndex.Length; i++)
            {
                if (CM.isFreeAtIndex[i])
                {
                    _agent.destination = CM.queueWaypoints[i].position;
                    _queuePosition = i;
                    //_customerManager.isFreeAtIndex[i] = false;
                    _stateMachine.TransitionTo("inQueue");
                    break;
                }
            }
        };

        var moveToDesk = _stateMachine.CreateState("moveToDesk");
        moveToDesk.onEnter = delegate
        {
            CM.deskIsFree = false;

            _agent.destination = new Vector3(
                _deskWaypoint.position.x,
                0,
                _deskWaypoint.position.z
                );
        };
        moveToDesk.onFrame = delegate
        {
            CheckDistanceToDesk();
        };

        var atDesk = _stateMachine.CreateState("atDesk");
        atDesk.onEnter = delegate
        {
            Debug.Log("Customer is now waiting at Desk");
            StartCoroutine("LeaveAfterDelay");
           
        };
        atDesk.onFrame = delegate
        {
            // Player interaction here
        };

        var inQueue = _stateMachine.CreateState("inQueue");
        inQueue.onEnter = delegate
        {
            Debug.LogFormat("Customer waiting in Line. Position:{0}", _queuePosition);
            CM.isFreeAtIndex[_queuePosition] = false;
        };
        inQueue.onFrame = delegate
        {           
            if(_queuePosition > 0)
            {
                if (CM.isFreeAtIndex[_queuePosition - 1])
                {
                    CM.isFreeAtIndex[_queuePosition] = true;
                    CM.isFreeAtIndex[_queuePosition - 1] = false;
                    _agent.destination = CM.queueWaypoints[_queuePosition - 1].position;
                    _queuePosition--;
                }
            }
            else
            {
                if (CM.deskIsFree)
                {
                    CM.isFreeAtIndex[0] = true;
                    _stateMachine.TransitionTo("moveToDesk");
                }
            }
                
            
            
        };

        var leaving = _stateMachine.CreateState("leaving");
        leaving.onEnter = delegate
        {
            CM.deskIsFree = true;
            _agent.destination = _despawn.position;//GameObject.Find("CustomerDespawn").GetComponent<Transform>().position;
            
        };
        leaving.onFrame = delegate
        {
            var distanceToDespawn = Vector3.Distance(transform.position, _agent.destination);
            if (distanceToDespawn <= _range)
            {
                _stateMachine.TransitionTo("pooled");
            }
        };

        var pooled = _stateMachine.CreateState("pooled");
        pooled.onEnter = delegate
        {
                customerPool.ReturnObject(this.gameObject);
            _stateMachine.TransitionTo("entering");
        };

        
    }
    private void Update()
    {
        _stateMachine.Update();
    }

    private void CheckDistanceToDesk()
    {
        var distanceToDesk = Vector3.Distance(transform.position, _deskWaypoint.position);       
        if (distanceToDesk <= _range)
        {
            _stateMachine.TransitionTo("atDesk");
        }
    }

   IEnumerator LeaveAfterDelay()
    {
        yield return new WaitForSeconds(customerSpawner.patienceTimer);
        _stateMachine.TransitionTo("leaving");
    }
}
