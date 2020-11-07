using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Spawner")] // Set Difficulty on spawners at Start based on day and players
    [SerializeField] private CrackerSpawner _crackerSpawner = null;
    [SerializeField] private FireSpawner _fireSpawner = null;
    [SerializeField] private CustomerSpawner _customerSpawner = null;

    [Header("Manager")] 
    [SerializeField] private DayTimeManager _dayTimeManager = null;// Set day start/end time and how long the day should be
    [SerializeField] private CustomerManager _customerManager = null;//active desks
    [SerializeField] private PlayerInputManager _playerInputManager = null; //Get local Player Count & reset Difficulty when someone joins


    private void Start()
    {
        CalculateDifficulty();
    }

    #region Called by PlayerInputManager
    public void OnLocalPlayerJoined()
    {
        CalculateDifficulty();
    }
    public void OnLocalPlayerLeft()
    {
        CalculateDifficulty();
    }
    #endregion

    private void CalculateDifficulty()
    {
        int playerCount = _playerInputManager.playerCount;

        //get current day (number)
        //Set day time (random start & end?)/ Length of day based on day number
        //Calculate Difficulty on FireSpawner, CustomerSpawner, CrackerSpawner
    }

}
