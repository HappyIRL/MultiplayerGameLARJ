﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevel : MonoBehaviour
{
    private enum LevelType
    {
        Level1,
        Level2,
        Level3
    }

    public int Cost = 0;
    public bool Bought = false;
    [SerializeField] private LevelType _levelType;
    [SerializeField] private GameObject _lockScreen = null;
    [SerializeField] private GameObject _costUI = null;
    [SerializeField] private Button _playlevelButton = null;
    [SerializeField] private TextMeshProUGUI _costText = null;

    private void Awake()
    {
        if (Bought)
        {
            UnlockLevel();
        }
        else
        {
            if (PlayerPrefs.GetInt($"{_levelType}Bought", 0) == 1) UnlockLevel();
            else LockLevel();
        }
    }

    private void UnlockLevel()
    {
        Bought = true;
        _lockScreen.SetActive(false);
        _costUI.SetActive(false);
        _playlevelButton.interactable = true;
    }
    private void LockLevel()
    {
        Bought = false;
        _lockScreen.SetActive(true);
        _costUI.SetActive(true);
        _playlevelButton.interactable = false;
        _costText.text = Cost.ToString();
    }

    public void BuyLevel()
    {
        int money = PlayerPrefs.GetInt("TotalMoneyScore", 0);

        if(money >= Cost)
        {
            UnlockLevel();
            money -= Cost;
            PlayerPrefs.SetInt("TotalMoneyScore", money);
            PlayerPrefs.SetInt($"{_levelType}Bought", 1);
        }
    }
}
