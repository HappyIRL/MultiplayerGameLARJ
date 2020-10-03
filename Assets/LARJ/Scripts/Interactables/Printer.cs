using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class Printer : Interactable
{
    [Header("Paper")]
    [Header("Printer")]
    [SerializeField] private GameObject _paperPrefab = null;
    [SerializeField] private Transform _paperSpawnPoint = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip _printerInSound = null;
    [SerializeField] private AudioClip _printerInProgressSound = null;
    [SerializeField] private AudioClip _printerOutSound = null;
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;

    private AudioSource _audioSource;
    private Coroutine _lastCoroutine;

    public override void Start()
    {
        base.Start();
        interactableID = (InteractableObjectID)_interactableID;
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void StartPrinting()
    {
        _lastCoroutine = StartCoroutine(StartPrintingSoundCoroutine());
    }
    private IEnumerator StartPrintingSoundCoroutine()
    {
        _audioSource.loop = false;
        PlaySound(_printerInSound);
        yield return new WaitForSeconds(_printerInSound.length);
        PlaySound(_printerInProgressSound);
        _audioSource.loop = true;
    }

    private void FinishPrinting()
    {
        StopCoroutine(_lastCoroutine);
        PlaySound(_printerOutSound);
        _audioSource.loop = false;

        Instantiate(_paperPrefab, _paperSpawnPoint.position, _paperSpawnPoint.rotation);
    }
    private void CancelPrinting()
    {
        StopCoroutine(_lastCoroutine);
        _audioSource.Stop();
        _audioSource.loop = false;
    }

    public override void HoldingStartedEvent()
    {
        StartPrinting();
    }

    public override void HoldingFailedEvent()
    {
        CancelPrinting();
    }

    public override void HoldingFinishedEvent()
    {
        FinishPrinting();
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
