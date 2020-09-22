using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
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

    

    private void Start()
    {

        // register statemachine and grab neccessary components
        _stateMachine = new StateMachine();
            //First state registered == initState
        var entering = _stateMachine.CreateState("entering");

        _agent = GetComponent<NavMeshAgent>();
        _deskWaypoint = customerSpawner.deskWaypoint;
        _despawn = customerSpawner.customerDespawnPoint;


        entering.onEnter = delegate 
        {            
            if (customerSpawner.deskIsFree) // Go get served
            {
                _stateMachine.TransitionTo("moveToDesk");                
            }
            else
            {
                QueueUp();   
            }
        };
        entering.onFrame = delegate
        {
                     
        };

        var moveToDesk = _stateMachine.CreateState("moveToDesk");
        moveToDesk.onEnter = delegate
        {
            MoveToDesk();                   
        };
        moveToDesk.onFrame = delegate
        {          
            CheckDistanceToDesk();            
        };

        var atDesk = _stateMachine.CreateState("atDesk");
        atDesk.onEnter = delegate
        {
            // Get ready for interaction             
            _isWaiting = true;
            StartCoroutine("LeaveAfterDelay");
        };
        atDesk.onFrame = delegate
        {
            // check for Player interaction here
            //if (Player is interacting with Customer)
            {

                StopCoroutine("LeaveAfterDelay");

            }

        };

        var inQueue = _stateMachine.CreateState("inQueue");
        inQueue.onEnter = delegate
        {
            Debug.LogFormat("Customer waiting in Line. Position:{0}", _queuePosition);
            customerSpawner.isFreeAtIndex[_queuePosition] = false;
        };
        inQueue.onFrame = delegate
        {           
            if(_queuePosition > 0)
            {
                if (customerSpawner.isFreeAtIndex[_queuePosition - 1])
                {
                    customerSpawner.isFreeAtIndex[_queuePosition] = true;
                    customerSpawner.isFreeAtIndex[_queuePosition - 1] = false;
                    _agent.destination = customerSpawner.queueWaypoints[_queuePosition - 1].position;
                    _queuePosition--;
                }
            }
            else
            {
                if (customerSpawner.deskIsFree)
                {
                    customerSpawner.isFreeAtIndex[0] = true;
                    _stateMachine.TransitionTo("moveToDesk");
                }
            }
                
            
            
        };

        var leaving = _stateMachine.CreateState("leaving");
        leaving.onEnter = delegate
        {
            customerSpawner.deskIsFree = true;
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
                //_customerManager.isFreeAtIndex[i] = false;
                _stateMachine.TransitionTo("inQueue");
                break;
            }
        }
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

    public void LeaveHappy()
    {
        StopCoroutine("LeaveAfterDelay");
        this.GetComponent<Renderer>().material.SetColor("_happy", Color.green);
        _stateMachine.TransitionTo("leaving");
    }
}
