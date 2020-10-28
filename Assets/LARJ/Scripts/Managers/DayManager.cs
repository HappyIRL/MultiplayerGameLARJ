using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DayManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject _dayFinishedScoreBoard = null;
    [SerializeField] private GameObject _pausedScreen = null;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _endScoreText = null;
    [SerializeField] private SceneChanger _sceneChanger = null;
    [SerializeField] private Score _score = null;
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
        _endScoreText.text = $"Money: {_score.ScoreCount}";
        Time.timeScale = 0;
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
