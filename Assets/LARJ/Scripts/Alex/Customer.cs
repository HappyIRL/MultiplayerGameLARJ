using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    StateMachine _stateMachine;
    public CustomerManager CM;
    NavMeshAgent _agent;
    // Needed in Scene:
    private Transform _deskWaypoint;
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
        

       
        entering.onEnter = delegate 
        {
            Debug.Log("Customer Entering!");
            if (CM.deskIsFree)
            {
                _agent.destination = new Vector3(
                _deskWaypoint.position.x,
                0,
                _deskWaypoint.position.z
                );
                CM.deskIsFree = false;
            }
            else
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
            }
        };
        entering.onFrame = delegate
        {            
                var distanceToDesk = Vector3.Distance(transform.position, _deskWaypoint.position);            
                if (distanceToDesk <= _range)
                {
                    _stateMachine.TransitionTo("atDesk");
                }
        };        

        var atDesk = _stateMachine.CreateState("atDesk");
        atDesk.onEnter = delegate
        {
            Debug.Log("Customer is now waiting");            
        };
        atDesk.onFrame = delegate
        {
            // if coundown is up, transition to angry

            // if task was completed during countdown, customer leaves
        };

        var inQueue = _stateMachine.CreateState("inQueue");
        inQueue.onEnter = delegate
        {
            Debug.LogFormat("Customer waiting in Line. Position:{0}", _queuePosition);
            CM.isFreeAtIndex[_queuePosition] = false;
        };
        inQueue.onFrame = delegate
        {

        };


        
    }
    private void Update()
    {
        _stateMachine.Update();
    }

   





}
