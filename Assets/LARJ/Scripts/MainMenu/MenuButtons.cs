using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuButtons : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClickSound = null;
    [SerializeField] private SceneChanger _sceneChanger = null;

    [Header("Screens")]
    [SerializeField] private GameObject _mainMenuScreen = null;
    [SerializeField] private GameObject _startPlayScreen = null;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _buttonClickSound;

        OpenMainMenuScreen();
    }

    public void PlayButtonClickSound()
    {
        _audioSource.Play();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void OpenMainMenuScreen()
    {
        _mainMenuScreen.SetActive(true);
        _startPlayScreen.SetActive(false);
    }
    public void OpenStartPlayScreen()
    {
        _mainMenuScreen.SetActive(false);
        _startPlayScreen.SetActive(true);
    }

    public void StartLocalGame()
    {
        //TODo: Fade to play Scene
        //_sceneChanger.FadeToScene();
    }
}
