using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [SerializeField] private Toggle _sFXToggle = null;

    private AudioSource _audioSource;
    private static SFXManager _instance;
    private bool _sFXOn = true;
    public static SFXManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        _audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("SFXOn", 1) == 1)
        {
            _sFXToggle.isOn = false;
            EnableSound();
        }
        else
        {
            _sFXToggle.isOn = true;
            DisableSound();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (_sFXOn)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
    public void PlaySound(AudioSource audioSource, AudioClip clip)
    {
        if (_sFXOn)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void StopAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void EnableSound()
    {
        _sFXOn = true;
        PlayerPrefs.SetInt("SFXOn", 1);
    }
    public void DisableSound()
    {
        _sFXOn = false;
        PlayerPrefs.SetInt("SFXOn", 0);
    }
    public void OnPressSFXToggle()
    {
        if (_sFXOn) DisableSound();
        else EnableSound();
    }
}
