using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    StateMachine _stateMachine;
    NavMeshAgent _agent;
    // Object Customer is looking to go to
    [SerializeField] Transform customerDesk;
    [SerializeField] float deskRange = 3f;

    private void Start()
    {
        _stateMachine = new StateMachine();
        _agent = GetComponent<NavMeshAgent>();
        

        // First Registered state will be initial state
        var coming = _stateMachine.CreateState("coming");

        coming.onEnter = delegate 
        { 
            Debug.Log("Customer entered the bank & is on his way to the desk"); 
        };
        coming.onFrame = delegate
        {
            // walk to the queue at counter
            _agent.destination = new Vector3(
                customerDesk.position.x,
                0,
                customerDesk.position.z
                );

            // find distance to counter
            var distanceToDesk = Vector3.Distance(transform.position, customerDesk.position);
            // when in range, transition to waiting
            if(distanceToDesk <= deskRange)
            {
                _stateMachine.TransitionTo("waiting");
            }
            


        };        

        var waiting = _stateMachine.CreateState("waiting");
        waiting.onEnter = delegate
        {
            Debug.Log("Customer is now waiting");
            // start wait countdown
        };
        waiting.onFrame = delegate
        {
            // if coundown is up, transition to angry

            // if task was completed during countdown, customer leaves
        };
        
    }
    private void Update()
    {
        _stateMachine.Update();
    }





}
