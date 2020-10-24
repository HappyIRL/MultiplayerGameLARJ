using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Tasks;
using Photon.Pun;

public class Customer : Interactable, IObjectPoolNotifier, IQueueUpdateNotifier
{
    private StateMachine _stateMachine;
    NavMeshAgent _agent;       
    
    private Transform _deskWaypoint;

    public Transform despawn;

    private readonly float _range = 3f;
    private int _queuePosition;
    private bool _isWaiting;

    private float _timer;

    private CustomerManager cm;

   
    public override void Awake()
    {        
        base.Awake();
        InteractableID = (InteractableObjectID)73;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;       
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    public override void Start()
    {
        base.Start();

        _stateMachine = new StateMachine();
        var Entry = _stateMachine.CreateState("Entry", EntryStart);
        var InQueue = _stateMachine.CreateState("InQueue", QueueStart);
        var MoveToDesk = _stateMachine.CreateState("MoveToDesk", MoveToDeskStart, MoveToDeskUpdate);
        var AtDesk = _stateMachine.CreateState("AtDesk", AtDeskStart, AtDeskUpdate, AtDeskExit);
        var Leaving = _stateMachine.CreateState("Leaving", LeavingStart, LeavingUpdate);

        _agent.enabled = true;
    }

    

    #region Entry State
    private void EntryStart()
    {
        GetComponent<Renderer>().material.color = Color.white;
        cm = CustomerManager.instance;
                
        _stateMachine.TransitionTo("InQueue");        
    }   

    #endregion
    #region InQueue State
    private void QueueStart()
    {
        cm.EnqueueCustomer(this.gameObject);
        if (cm.DeskKvps.ContainsValue(true))
        {
            _stateMachine.TransitionTo("MoveToDesk");
            Debug.Log("Gointodesk");
            cm.DequeueCustomer();
        }

    }    
    public void OnEnqueuedToQueue() // GET IN QUEUE POS
    {
        _queuePosition = cm.CustomerQueue.Count;

        _agent.destination = cm.QueueWaypoints[_queuePosition - 1];
    }

    public void OnQueueUpdated() // MOVE UP THE QUEUE
    {
        _queuePosition--;
        _agent.destination = cm.QueueWaypoints[_queuePosition - 1];
    }    

    public void OnLeftQueue() // MOVE TO DESK
    {
        var desks = cm.DeskWaypoints;
        foreach (var desk in desks)
        {
            if (cm.DeskKvps[desk])
            {
                _agent.destination = desk.position;
                cm.JoinedDesk(desk);
                _deskWaypoint = desk;
                _stateMachine.TransitionTo("MoveToDesk");
                return;
            }
        }
    }

    #endregion
    private void MoveToDeskStart()
    {

    } 
    private void MoveToDeskUpdate()
    {
        if (DidArriveAtDesk())
        {
            _stateMachine.TransitionTo("AtDesk");
        }
    }  
    #region AtDesk State
    private void AtDeskStart()
    {
        _timer = 0;
        StartCoroutine("LeaveAfterDelay");
        var rot = transform.rotation.eulerAngles;
        _agent.updateRotation = false;
        transform.Rotate(-rot);
        if(!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            TaskManager.TaskManagerSingelton.StartTask(GetComponent<Task>());
    }
    private void AtDeskUpdate()
    {

    }
    private void AtDeskExit()
    {
        _agent.updateRotation = true;
        cm.LeftDesk(_deskWaypoint);
    }  
    IEnumerator LeaveAfterDelay()
    {
        while (_timer < 10) // PATIENCE ADDED HERE
        {
            _timer += Time.deltaTime;
            yield return null;
        }
        // Log Failed Task
        var color = Color.red;
        GetComponent<Renderer>().material.color = color;
               
        _stateMachine.TransitionTo("Leaving");
    }  

    #endregion
    #region Leaving State
    private void LeavingStart()
    {
        _agent.destination = despawn.position;       
        cm.DequeueCustomer();
    } // MOVE TO DESPAWN

    private void LeavingUpdate()
    {
        var distanceToDespawn = Vector3.Distance(transform.position, _agent.destination);
        if (distanceToDespawn <= _range)
        {
            PooledGameObjectExtensions.ReturnToPool(this.gameObject);
            GetComponent<Renderer>().material.color = Color.white;
        }
    } // DESPAWN ON ARRIVAL


    #endregion   
   
    private void OnEnterTalk()
    {
    }
    private void OnFinishedTalk()
    {
        // Log Successful Talk
        var color = Color.green;
        GetComponent<Renderer>().material.color = color;
        _stateMachine.TransitionTo("Leaving");
    } // SUCCESSFUL TALK

    private void OnFailedTalk()
    {
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

    #region Interactable Events
    public override void HoldingStartedEvent()
    {
        base.HoldingStartedEvent();
        OnEnterTalk();
    }

    public override void HoldingFailedEvent()
    {
        base.HoldingFailedEvent();
        OnFailedTalk();
    }

    public override void HoldingFinishedEvent()
    {
        base.HoldingFinishedEvent();
        OnFinishedTalk();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }
    public override void OnNetworkHoldingFinishedEvent()
    {
        OnFinishedTalk();
    }

   
    #endregion
}
