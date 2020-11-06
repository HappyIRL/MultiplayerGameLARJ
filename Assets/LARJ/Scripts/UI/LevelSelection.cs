using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [Header("Level Selection")]
    [SerializeField] private List<MenuLevel> _levels = null;
    [SerializeField] private Button _leftButton = null;
    [SerializeField] private Button _rightButton = null;
    [SerializeField] private TextMeshProUGUI _moneyText = null;
    private int _currentIndex = 0;

    private void Awake()
    {
        ActivateLevel();
        UpdateMoneyText();
    }
    private void ActivateLevel()
    {
        if (_currentIndex == 0)
        {
            _leftButton.interactable = false;
        }
        else if(_currentIndex == _levels.Count - 1)
        {
            _rightButton.interactable = false;
        }
        else
        {
            _leftButton.interactable = true;
            _rightButton.interactable = true;
        }

        for (int i = 0; i < _levels.Count; i++)
        {
            if (i == _currentIndex) _levels[i].gameObject.SetActive(true);
            else _levels[i].gameObject.SetActive(false);
        }

    }   
    public void ClickOnLeftArrow()
    {
        if(_currentIndex > 0)
        {
            _currentIndex--;
            ActivateLevel();
        }
    }
    public void ClickOnRightArrow()
    {
        if (_currentIndex < _levels.Count - 1)
        {
            _currentIndex++;
            ActivateLevel();
        }
    }
    public void UpdateMoneyText()
    {
        _moneyText.text = PlayerPrefs.GetInt("TotalMoneyScore", 0).ToString();
    }
}
