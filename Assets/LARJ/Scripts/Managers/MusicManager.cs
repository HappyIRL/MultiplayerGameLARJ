using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Toggle _musicToggle = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _backgroundMusic = null;

    private static MusicManager _instance;
    private bool _musicOn = true;
    public static MusicManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = 0.01f;
        _audioSource.clip = _backgroundMusic;

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
        _audioSource.Play();
        PlayerPrefs.SetInt("MusicOn", 1);
    }
    public void DisableMusic()
    {
        _musicOn = false;
        _audioSource.Stop();
        PlayerPrefs.SetInt("MusicOn", 0);
    }
    public void OnPressMusicToggle()
    {
        if (_musicOn) DisableMusic();
        else EnableMusic();
    }
}
