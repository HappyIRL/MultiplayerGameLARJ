﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour, IObjectPoolNotifier
{
    private StateMachine _stateMachine;
    NavMeshAgent _agent;

    public CustomerSpawner customerSpawner;
    public ObjectPool customerPool;

    private Transform _deskWaypoint;
    private Transform _despawn;

    private readonly float _range = 3f;
    private int _queuePosition;
    private bool _isWaiting;

    private float _timer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;       
    }

    private void Start()
    {
        _deskWaypoint = customerSpawner.deskWaypoint;
        _despawn = customerSpawner.customerDespawnPoint;

        // register statemachine and grab neccessary components
        _stateMachine = new StateMachine();

        var Entry = _stateMachine.CreateState("Entry", EntryStart, EntryUpdate, EntryExit);

        var InQueue = _stateMachine.CreateState("InQueue", QueueStart, QueueUpdate, QueueExit);

        var AtDesk = _stateMachine.CreateState("AtDesk", AtDeskStart, AtDeskUpdate, AtDeskExit);

        var Leaving = _stateMachine.CreateState("Leaving", LeavingStart, LeavingUpdate, LeavingExit);

        _agent.enabled = true;
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    #region Entry State
    private void EntryStart()
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
    }
    private void EntryUpdate()
    {
        if (DidArriveAtDesk()) _stateMachine.TransitionTo("AtDesk");
    }
    private void EntryExit()
    {
        
    }

    #endregion
    #region InQueue State
    private void QueueStart()
    {
    }
    private void QueueUpdate()
    {
        if (_queuePosition == 0)
        {
            if (customerSpawner.deskIsFree)
            {
                customerSpawner.isFreeAtIndex[0] = true;
                _stateMachine.TransitionTo("Entry");
            }
        }
        else
        {
            WaitInLine();
        }
    }
    private void QueueExit()
    {
    }
    #endregion
    #region AtDesk State
    private void AtDeskStart()
    {
        _isWaiting = true;
        StartCoroutine("LeaveAfterDelay");
        var rot = transform.rotation.eulerAngles;
        _agent.updateRotation = false;
        transform.Rotate(-rot);

    }
    private void AtDeskUpdate()
    {
    }   
    private void AtDeskExit()
    {
        _agent.updateRotation = true;
    }
    #endregion
    #region Leaving State
    private void LeavingStart()
    {
        customerSpawner.deskIsFree = true;
        _timer = 0;
        _agent.destination = _despawn.position;//GameObject.Find("CustomerDespawn").GetComponent<Transform>().position;
    }

    private void LeavingUpdate()
    {
        var distanceToDespawn = Vector3.Distance(transform.position, _agent.destination);
        if (distanceToDespawn <= _range)
        {
            customerPool.ReturnObject(this.gameObject);
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void LeavingExit()
    {        
    }

    #endregion   

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
        _stateMachine.TransitionTo("Leaving");
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
        _stateMachine.TransitionTo("Leaving");
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
                _stateMachine.TransitionTo("InQueue");
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
            _stateMachine.TransitionTo("Entry");
        }
    }
}
