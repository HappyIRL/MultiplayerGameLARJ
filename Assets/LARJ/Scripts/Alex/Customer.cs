using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour, IObjectPoolNotifier
{
    StateMachine _stateMachine;
    NavMeshAgent _agent;
    GameObject _go;
   
    public CustomerSpawner customerSpawner;
    public ObjectPool customerPool;

    private Transform _deskWaypoint;
    private Transform _despawn;

    private Transform _transform;
    private float _range = 3f;
    private Vector3 _target;
    private int _queuePosition;
    private bool _isWaiting;

    private float _timer;
    

    private void Start()
    {

        // register statemachine and grab neccessary components
        _stateMachine = new StateMachine();
            //First state registered == initState
        var entering = _stateMachine.CreateState("entering");

        _agent = GetComponent<NavMeshAgent>();
        _deskWaypoint = customerSpawner.deskWaypoint;
        _despawn = customerSpawner.customerDespawnPoint;

    #region ENTER STATE
        entering.onEnter = delegate 
        {
            GetComponent<Renderer>().material.color = Color.white;

            if (customerSpawner.deskIsFree) // Go get served
            {                                
                MoveToDesk();                   
            }
            else
            {
                QueueUp();                
            }
        };
        entering.onFrame = delegate
        {            
            if (DidArriveAtDesk()) _stateMachine.TransitionTo("atDesk");            
        };

        #endregion

    #region IN QUEUE STATE

        var inQueue = _stateMachine.CreateState("inQueue");
        inQueue.onEnter = delegate
        {

        };
        inQueue.onFrame = delegate
        {            
            if (_queuePosition == 0)
            {
                if (customerSpawner.deskIsFree)
                {
                    customerSpawner.isFreeAtIndex[0] = true;
                    _stateMachine.TransitionTo("entering");
                }
            }
            else
            {
                WaitInLine();
            }
        };
#endregion

    #region AT DESK STATE
        var atDesk = _stateMachine.CreateState("atDesk");
        atDesk.onEnter = delegate
        {                       
            _isWaiting = true;
            StartCoroutine("LeaveAfterDelay");
            
        };
        atDesk.onFrame = delegate
        {
            
        };

#endregion

    #region LEAVING STATE
        var leaving = _stateMachine.CreateState("leaving");
        leaving.onEnter = delegate
        {
            customerSpawner.deskIsFree = true;
            _timer = 0;
            _agent.destination = _despawn.position;//GameObject.Find("CustomerDespawn").GetComponent<Transform>().position;
            
        };
        leaving.onFrame = delegate
        {
            var distanceToDespawn = Vector3.Distance(transform.position, _agent.destination);
            if (distanceToDespawn <= _range)
            {                
                customerPool.ReturnObject(this.gameObject);
                GetComponent<Renderer>().material.color = Color.white;
            }
        };
#endregion
    } 
    private void WaitInLine()
    {        
        var posInFront = _queuePosition - 1;
        for (int i = 0; i < customerSpawner.queueWaypoints.Count; i++)
        {
            if (customerSpawner.isFreeAtIndex[posInFront])
            {
                customerSpawner.isFreeAtIndex[_queuePosition] = true;
                customerSpawner.isFreeAtIndex[posInFront] = false;
                _agent.destination = customerSpawner.queueWaypoints[posInFront].position;
                _queuePosition--;               
                break;
            }
        }
        
    }
    public void OnEnterTalk()
    {
        StopCoroutine("LeaveAfterDelay");
    }
    public void OnFinishedTalk()
    {
        // Log Successful Talk
        var color = Color.green;
        GetComponent<Renderer>().material.color = color;
        _stateMachine.TransitionTo("leaving");
    }
    public void OnFailedTalk()
    {
        StartCoroutine("LeaveAfterDelay");
    }
    
    IEnumerator LeaveAfterDelay()
    {
        while (_timer < customerSpawner.patienceTimer)
        {
            _timer += Time.deltaTime;
            yield return null;
        }
        // Log Failed Task
        var color = Color.red;
        GetComponent<Renderer>().material.color = color;

        _isWaiting = false;
        _stateMachine.TransitionTo("leaving");
    }


    private void Update()
    {
        _stateMachine.Update();
    }

    private void MoveToDesk()
    {
        _agent.destination = new Vector3(
                            _deskWaypoint.position.x,
                            0,
                            _deskWaypoint.position.z);
        customerSpawner.deskIsFree = false;         
    }
    private void QueueUp()
    {
        for (int i = 0; i < customerSpawner.queueWaypoints.Count; i++)
        {
            if (customerSpawner.isFreeAtIndex[i])
            {
                _agent.destination = customerSpawner.queueWaypoints[i].position;
                _queuePosition = i;
                customerSpawner.isFreeAtIndex[i] = false;
                _stateMachine.TransitionTo("inQueue");
                break;
            }
        }
    }
   

    private bool DidArriveAtDesk()
    {
        var distanceToDesk = Vector3.Distance(transform.position, _deskWaypoint.position);
        
        if (distanceToDesk <= _range)
        {
            return true;            
        }
        else
        {
            return false;
        }
    }

    public void OnEnqueuedToPool()
    {
       
    }

    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (!created)
        {
            _stateMachine.TransitionTo("entering");
        }
    }
}
