using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum LevelType
{
    Level1,
    Level2,
    Level3
}

public class MenuLevel : MonoBehaviour
{

    public int Cost = 0;
    public bool Bought = false;
    [SerializeField] private LevelType _levelType;
    [SerializeField] private GameObject _lockScreen = null;
    [SerializeField] private GameObject _costUI = null;
    [SerializeField] private Button _playlevelButton = null;
    [SerializeField] private TextMeshProUGUI _costText = null;
    [SerializeField] private TextMeshProUGUI _dayNumberText = null;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _buySound = null;

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
        _dayNumberText.gameObject.SetActive(true);
    }
    private void LockLevel()
    {
        Bought = false;
        _lockScreen.SetActive(true);
        _costUI.SetActive(true);
        _playlevelButton.interactable = false;
        _costText.text = Cost.ToString();
        _dayNumberText.gameObject.SetActive(false);
    }

    public void BuyLevel()
    {
        int money = PlayerPrefs.GetInt("TotalMoneyScore", 0);

        if(money >= Cost)
        {
            UnlockLevel();

            SFXManager.Instance.PlaySound(_audioSource, _buySound);

            money -= Cost;
            PlayerPrefs.SetInt("TotalMoneyScore", money);
            PlayerPrefs.SetInt($"{_levelType}Bought", 1);
        }
    }
}
