using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tasks;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelType _levelType = LevelType.Level1;
    
    [Header("Spawner")] // Set Difficulty on spawners at Start based on day and players
    [SerializeField] private CrackerSpawner _crackerSpawner = null;
    [SerializeField] private FireSpawner _fireSpawner = null;
    [SerializeField] private CustomerSpawner _customerSpawner = null;

    [Header("Manager")] 
    [SerializeField] private DayTimeManager _dayTimeManager = null;// Set day start/end time and how long the day should be
    [SerializeField] private CustomerManager _customerManager = null;//active desks
    [SerializeField] private PlayerInputManager _playerInputManager = null; //Get local Player Count & reset Difficulty when someone joins
    [SerializeField] private TaskManager _taskManager = null;//Set Task delay

    public LevelType LevelType { get => _levelType; }


    private void Start()
    {
        CalculateDifficulties();
    }

    #region Called by PlayerInputManager
    public void OnLocalPlayerJoined()
    {
        CalculateDifficulties();
    }
    public void OnLocalPlayerLeft()
    {
        CalculateDifficulties();
    }
    #endregion

    private void CalculateDifficulties()
    {
        int playerCount = _playerInputManager.playerCount;

        //get current day (number)
        int currentDay = PlayerPrefs.GetInt($"{_levelType}CurrentDay", 1);

        //Set day time (random start & end?)/ Length of day based on day number

        //Calculate Difficulty on FireSpawner, CustomerSpawner, CrackerSpawner
    }

}
