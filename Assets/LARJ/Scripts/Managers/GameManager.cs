using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tasks;
using Photon.Pun.UtilityScripts;
using TMPro;

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

    [Header("DayUI")]
    [SerializeField] private TextMeshProUGUI _sceneChangerDayText = null;
    private int currentDay;

    public LevelType LevelType { get => _levelType; }


    private void Start()
    {
        CalculateDifficulties();
        ShowDayText();
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
        //Check if Local or Network!


        int playerCount = _playerInputManager.playerCount;

        //get current day (number)
        currentDay = PlayerPrefs.GetInt($"{_levelType}CurrentDay", 1);

        //Set day time (random start & end?)/ Length of day based on day number

        //Calculate Difficulty on FireSpawner, CustomerSpawner, CrackerSpawner

        if (currentDay == 1)
        {

        }
        else
        {

        }
    }
    private void SetDifficulties(int crackerDifficulty, int fireDifficulty, int taskDifficulty, int customerDifficulty, int activeDesks)
    {
        _crackerSpawner.Difficulty = crackerDifficulty;
        _fireSpawner.Difficulty = fireDifficulty;
        _taskManager.Difficulty = taskDifficulty;
        _customerSpawner.Difficulty = customerDifficulty;
        _customerManager.ActiveDesks = activeDesks;
    }
    private void SetDayTime(int start, int end, int realTimeLength)
    {
        _dayTimeManager.SetStartValues(start, end, realTimeLength);
    }

    private void ShowDayText()
    {
        _sceneChangerDayText.gameObject.SetActive(true);
        _sceneChangerDayText.text = $"Day {currentDay}";
        _sceneChangerDayText.CrossFadeAlpha(0,3f, true);
    }
}
