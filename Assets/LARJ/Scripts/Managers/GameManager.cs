﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tasks;
using Photon.Pun.UtilityScripts;
using TMPro;
using Photon.Pun;

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
        if(_playerInputManager.playerCount > 0) CalculateDifficulties();
    }
    #endregion

    private void CalculateDifficulties()
    {
        //Check if Local or Network!

        int playerCount;

        if (!PhotonNetwork.IsConnected)
            playerCount = _playerInputManager.playerCount;
        else
            playerCount = 2;
        
        //get current day (number)
        currentDay = PlayerPrefs.GetInt($"{_levelType}CurrentDay", 1);

        //Set day time (random start & end?)/ Length of day based on day number

        //Calculate Difficulty on FireSpawner, CustomerSpawner, CrackerSpawner

        if (playerCount == 1)
        {
            switch (currentDay)
            {
                case 1:
                    SetDifficulties(0, 0, 1, 1, 1);
                    break;
                case 2:
                    SetDifficulties(0, 1, 1, 1, 1);
                    break;
                case 3:
                    SetDifficulties(0, 1, 1, 1, 2);
                    break;
                case 4:
                    SetDifficulties(1, 1, 1, 2, 2);
                    break;
                default:
                    SetDifficulties(2, 2, 2, 2, 2);
                    break;
            }
        }
        else if (playerCount == 2)
        {
            switch (currentDay)
            {
                case 1:
                    SetDifficulties(0, 0, 1, 1, 2);
                    break;
                case 2:
                    SetDifficulties(0, 1, 1, 2, 2);
                    break;
                case 3:
                    SetDifficulties(0, 2, 2, 2, 2);
                    break;
                case 4:
                    SetDifficulties(1, 2, 2, 2, 2);
                    break;
                case 5:
                    SetDifficulties(2, 2, 2, 2, 2);
                    break;
                case 6:
                    SetDifficulties(3, 3, 3, 3, 2);
                    break;
                default:
                    SetDifficulties(3, 3, 3, 3, 3);
                    break;
            }
        }
        else if (playerCount == 3)
        {
            switch (currentDay)
            {
                case 1:
                    SetDifficulties(0, 1, 1, 1, 2);
                    break;
                case 2:
                    SetDifficulties(1, 1, 2, 2, 2);
                    break;
                case 3:
                    SetDifficulties(2, 2, 2, 2, 2);
                    break;
                case 4:
                    SetDifficulties(2, 2, 2, 3, 3);
                    break;
                case 5:
                    SetDifficulties(3, 3, 4, 4, 3);
                    break;
                case 6:
                    SetDifficulties(4, 4, 4, 4, 3);
                    break;
                case 7:
                    SetDifficulties(5, 5, 5, 5, 4);
                    break;

                default:
                    SetDifficulties(6, 6, 6, 6, 4);
                    break;
            }
        }
        else if (playerCount == 4)
        {
            switch (currentDay)
            {
                case 1:
                    SetDifficulties(1, 1, 1, 1, 2);
                    break;
                case 2:
                    SetDifficulties(2, 2, 2, 2, 2);
                    break;
                case 3:
                    SetDifficulties(3, 3, 3, 3, 3);
                    break;
                case 4:
                    SetDifficulties(4, 4, 4, 4, 3);
                    break;
                case 5:
                    SetDifficulties(5, 5, 5, 5, 4);
                    break;
                case 6:
                    SetDifficulties(6, 6, 6, 6, 4);
                    break;
                case 7:
                    SetDifficulties(7, 7, 7, 7, 5);
                    break;
                case 8:
                    SetDifficulties(8, 8, 8, 8, 5);
                    break;
                case 9:
                    SetDifficulties(8, 8, 8, 8, 5);
                    break;

                default:
                    SetDifficulties(8, 8, 8, 8, 6);
                    break;
            }
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
        StartCoroutine(WaitToDeactivateDayText(3f));
    }
    private IEnumerator WaitToDeactivateDayText(float time)
    {
        yield return new WaitForSeconds(time);
        _sceneChangerDayText.gameObject.SetActive(false);
    }
}
