using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource)), Serializable]
public class PC : Interactable
{
    [Header("PC")]
    [SerializeField] private AudioClip _keyboardTypingSound = null;
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;
    [SerializeField] private UnityEvent OnEMailComplete;
    private AudioSource _audioSource;

    public override void Start()
    {
        base.Start();
        interactableID = (InteractableObjectID)_interactableID;
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
        OnEMailComplete.Invoke();
    }

}

