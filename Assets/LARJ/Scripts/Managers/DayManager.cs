using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DayManager : MonoBehaviour
{
    [SerializeField] private GameObject _dayFinishedScoreBoard = null;
    [SerializeField] private TextMeshProUGUI _endScoreText = null;
    [SerializeField] private SceneChanger _sceneChanger = null;
    [SerializeField] private Score _score = null;
    [SerializeField] private AudioClip _buttonClickSound = null;

    private AudioSource _audioSource;

    private void Awake()
    {
        _dayFinishedScoreBoard.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _buttonClickSound;
    }

    public void ActivateDayFinishedScoreBoard()
    {
        _dayFinishedScoreBoard.SetActive(true);
        _endScoreText.text = $"Money: {_score.ScoreCount}";
    }
    public void GoToMainMenu()
    {
        _audioSource.Play();
        _sceneChanger.FadeToScene(0);
    }
    public void ReplayDay()
    {
        _audioSource.Play();
        _sceneChanger.FadeToScene(SceneManager.GetActiveScene().buildIndex);
    }
}
