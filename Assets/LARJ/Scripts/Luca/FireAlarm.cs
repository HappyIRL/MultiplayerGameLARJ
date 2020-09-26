using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireAlarm : MonoBehaviour
{
    [SerializeField] private AudioClip _alarmSound = null;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _alarmSound;
        _audioSource.loop = true;
    }

    public void StartAlarm()
    {
        _audioSource.Play();
    }
    public void StopAlarm()
    {
        _audioSource.Stop();
    }
}
