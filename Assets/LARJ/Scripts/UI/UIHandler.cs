using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuScreen = null;
    [SerializeField] private GameObject _startPlayScreen = null;
    [SerializeField] private GameObject _networkSectionScreen = null;
    [SerializeField] private GameObject _gameTitle = null;
    [SerializeField] private GameObject _connectionDialog;
    [SerializeField] private GameObject _failedToConnectDialog;
    [SerializeField] private GameObject _waitingForPlayersScreen;
    [SerializeField] private GameObject _startGamebutton;

    [SerializeField] private AudioClip _buttonClickSound = null;

    [SerializeField] private SceneChanger _sceneChanger = null;

    [SerializeField] private LARJConnectToPhoton _larjConnectToPhoton;

    [SerializeField] private List<GameObject> _playerImageGOs = new List<GameObject>();

    [SerializeField] private TMP_Text _waitingForX;


    private List<Image> _playerImages = new List<Image>();
    private Vector3 _gTSavedPos;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _buttonClickSound;
        _gTSavedPos = _gameTitle.transform.position;

        foreach (GameObject go in _playerImageGOs)
        {
            _playerImages.Add(go.GetComponent<Image>());
        }

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

    public void StartGame()
    {
        _sceneChanger.FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void EnterLocalSection()
    {
        _startPlayScreen.SetActive(true);
        _networkSectionScreen.SetActive(false);
        _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Local);
        _gameTitle.transform.position = _gTSavedPos;
    }

    public void TryEnterNetworkSection()
    {
        if(PhotonNetwork.IsConnected)
		{
            EnableConnectingDialog(true);
            _larjConnectToPhoton.SwitchToNetworkState(LARJNetworkState.Photon);
		}
        else
		{
            EnableFailedToConnectDialog(true);
        }
    }

    public void EnableFailedToConnectDialog(bool enable)
    {
       _failedToConnectDialog.SetActive(enable);
    }

    public void EnableConnectingDialog(bool enable)
    {
        _connectionDialog.SetActive(enable);
    }

    public void EnterNetworkSection()
    {
        EnableConnectingDialog(false);
        _startPlayScreen.SetActive(false);
        _networkSectionScreen.SetActive(true);
        _gameTitle.transform.position = new Vector3(1600, _gameTitle.transform.position.y, _gameTitle.transform.position.z);
    }
    public void WaitingRoomJoined()
    {
        _waitingForPlayersScreen.SetActive(true);
        _networkSectionScreen.SetActive(false);
    }
    
    public void WaitingRoomLeft()
	{
        _waitingForPlayersScreen.SetActive(false);
    }

    public void UpdatePlayerList()
    {
        //needs rework
        foreach (Image img in _playerImages)
        {
            img.color = Color.red;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            _playerImages[i].color = Color.green;
        }

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            _waitingForX.text = "Waiting For Host";
            if (PhotonNetwork.IsMasterClient)
            {
                _startGamebutton.SetActive(true);
            }
        }
        else
        {
            _waitingForX.text = "Waiting For Players";
            _startGamebutton.SetActive(false);
        }
    }
}
