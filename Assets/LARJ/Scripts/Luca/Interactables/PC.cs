using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class PC : MonoBehaviour
{
    [SerializeField] private AudioClip _keyboardTypingSound = null;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _keyboardTypingSound;
    }

    public void StartTyping()
    {
        _audioSource.Play();
    }

    public void StopTyping()
    {
        _audioSource.Stop();
    }
}

