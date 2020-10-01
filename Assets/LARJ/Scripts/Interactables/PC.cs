using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class PC : Interactable
{
    [Header("PC")]
    [SerializeField] private AudioClip _keyboardTypingSound = null;
    private AudioSource _audioSource;

    public override void Start()
    {
        base.Start();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _keyboardTypingSound;
    }

    private void StartTyping()
    {
        _audioSource.Play();
    }

    private void StopTyping()
    {
        _audioSource.Stop();
    }

    public override void HoldingStartedEvent()
    {
        StartTyping();
    }

    public override void HoldingFailedEvent()
    {
        StopTyping();
    }

    public override void HoldingFinishedEvent()
    {
        StopTyping();
    }

    public override void PressEvent()
    {
        
    }

    public override void MousePressEvent()
    {
        
    }

    public override void MouseReleaseEvent()
    {
        
    }
}

