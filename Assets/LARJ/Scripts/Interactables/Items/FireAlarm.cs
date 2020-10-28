using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireAlarm : MonoBehaviour
{
    [SerializeField] private AudioClip _alarmSound = null;
    private AudioSource _audioSource;
    private SFXManager _sFXManager;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _alarmSound;
        _audioSource.loop = true;
        _sFXManager = SFXManager.Instance;
    }

    public void StartAlarm()
    {
        _sFXManager.PlaySound(_audioSource, _alarmSound);
    }
    public void StopAlarm()
    {
        _sFXManager.StopAudioSource(_audioSource);
    }
}
