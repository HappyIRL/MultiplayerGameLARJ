using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    StateMachine _stateMachine;
    // Object Customer is looking to go to
    [SerializeField] GameObject _customerDesk;

    private void Start()
    {
        _stateMachine = new StateMachine();

        var coming = _stateMachine.CreateState("coming");
        var inLine = _stateMachine.CreateState("inLine");
        var waiting = _stateMachine.CreateState("waiting");
        var leaving = _stateMachine.CreateState("leaving");
        var angry = _stateMachine.CreateState("angry");

        coming.onEnter = delegate 
        { 
            Debug.Log("Customer entered the bank & is on his way to the desk"); 
        };
        coming.onFrame = delegate
        {
            // walks to the queue at counter

            // when in range, transition to inLine
        };

        inLine.onEnter = delegate
        {
            Debug.Log("Customer is now in line waiting");
        };
        inLine.onFrame = delegate
        {
            // waits behind other customers if there are any

            // walks to desk if free, transition to waiting
        };

        waiting.onEnter = delegate
        {
            // start wait countdown
        };
        waiting.onFrame = delegate
        {
            // if coundown is up, transition to angry

            // if task was completed during countdown, customer leaves
        };

        


    }





}
