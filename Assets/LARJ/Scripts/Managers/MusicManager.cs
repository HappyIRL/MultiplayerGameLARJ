using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Toggle _musicToggle = null;

    private static MusicManager _instance;
    private bool _musicOn = true;
    public static MusicManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;

        if (PlayerPrefs.GetInt("MusicOn", 1) == 1)
        {
            _musicToggle.isOn = false;
            EnableMusic();
        }
        else
        {
            _musicToggle.isOn = true;
            DisableMusic();
        }
    }

    public void EnableMusic()
    {
        _musicOn = true;
        PlayerPrefs.SetInt("MusicOn", 1);
    }
    public void DisableMusic()
    {
        _musicOn = false;
        PlayerPrefs.SetInt("MusicOn", 0);
    }
    public void OnPressMusicToggle()
    {
        if (_musicOn) DisableMusic();
        else EnableMusic();
    }
}
