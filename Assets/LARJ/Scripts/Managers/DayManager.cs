using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tasks;

[RequireComponent(typeof(AudioSource))]
public class DayManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject _dayFinishedScoreBoard = null;
    [SerializeField] private GameObject _pausedScreen = null;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _endScoreText = null;
    [SerializeField] private TextMeshProUGUI _failedTaskText = null;
    [SerializeField] private TextMeshProUGUI _completedTaskText = null;
    [SerializeField] private GameObject _newHighScoreText = null;

    [Header("References")]
    [SerializeField] private SceneChanger _sceneChanger = null;
    [SerializeField] private Score _score = null;
    [SerializeField] private TaskManager _taskManager = null;
    [SerializeField] private AudioClip _buttonClickSound = null;

    private AudioSource _audioSource;
    private SFXManager _sFXManager;
    private bool _canPressEsc = true;

    private void Awake()
    {
        _dayFinishedScoreBoard.SetActive(false);
        _pausedScreen.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _buttonClickSound;
    }
    private void Start()
    {
        _sFXManager = SFXManager.Instance;
    }

    public void ActivateDayFinishedScoreBoard()
    {
        _dayFinishedScoreBoard.SetActive(true);
        _score.SaveMoney();
        SetScoreTexts();
        Time.timeScale = 0;
    }

    private void SetScoreTexts()
    {
        int money = _score.ScoreCount;
        if (money > PlayerPrefs.GetInt("HighscoreMoney", 0))
        {
            _newHighScoreText.SetActive(true);
            PlayerPrefs.SetInt("HighscoreMoney", money);
        }
        else
        {
            _newHighScoreText.SetActive(false);
        }


        _endScoreText.text = $"Money: {money}";
        _failedTaskText.text = $"Failed Tasks: {_taskManager.FailedTaks}";
        _completedTaskText.text = $"Completed Tasks: {_taskManager.CompletedTasks}";
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        _canPressEsc = false;
        _sFXManager.PlaySound(_audioSource, _buttonClickSound);
        _sceneChanger.FadeToScene(0);
    }
    public void ReplayDay()
    {
        Time.timeScale = 1;
        _sFXManager.PlaySound(_audioSource, _buttonClickSound);
        _sceneChanger.FadeToScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PauseGame()
    {
        _sFXManager.PlaySound(_audioSource, _buttonClickSound);
        _pausedScreen.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        _sFXManager.PlaySound(_audioSource, _buttonClickSound);
        Time.timeScale = 1;
        _pausedScreen.SetActive(false);
    }
    public void PressESCInteraction()
    {
        if (_canPressEsc)
        {
            if (_dayFinishedScoreBoard.activeSelf) return;

            if (_pausedScreen.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
