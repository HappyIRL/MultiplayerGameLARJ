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
    [SerializeField] private GameObject _networkSectionScreen = null;
    [SerializeField] private GameObject _gameTitle = null;

    [SerializeField] private LARJConnectToPhoton _larjConnectToPhoton;

    private Vector3 _gTSavedPos;
    private RectTransform _gTSavedRect;
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
        _sceneChanger.FadeToScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void EnterLocalSection()
	{
        _startPlayScreen.SetActive(true);
        _networkSectionScreen.SetActive(false);
        _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Local);
        RectTransform rt = (RectTransform)_gameTitle.transform;
        rt.sizeDelta = _gTSavedRect.sizeDelta;

    }

	public void EnterNetworkSection()
	{
        _startPlayScreen.SetActive(false);
        _networkSectionScreen.SetActive(true);
        _gTSavedPos = _gameTitle.transform.position;
        _gTSavedRect = (RectTransform)_gameTitle.transform;

        _gameTitle.transform.position = new Vector3(1600, _gameTitle.transform.position.y, _gameTitle.transform.position.z);
        RectTransform rt = (RectTransform)_gameTitle.transform;
        rt.sizeDelta = new Vector2(500, 300);
        _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Photon);
    }
}
