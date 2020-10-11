using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.Events;

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
    private GameObject _papergameObject;

	public override void Awake()
	{
		base.Awake();
        InteractableID = (InteractableObjectID)_interactableID;
    }

	public override void Start()
    {
        base.Start();
        
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
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
        PlaySound(_printerOutSound);
        _audioSource.loop = false;
        _papergameObject = Instantiate(_paperPrefab, _paperSpawnPoint.position, _paperSpawnPoint.rotation);
    }
    private void FinishPrinting(GameObject objectToSpawn)
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
        PlaySound(_printerOutSound);
        _audioSource.loop = false;
        GameObject obj = Instantiate(objectToSpawn, _paperSpawnPoint.position, _paperSpawnPoint.rotation);
        obj.layer = LayerMask.GetMask("Garbage");
    }

    private void CancelPrinting()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
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
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }
    public override void HoldingFinishedEvent(GameObject pickUpObject)
    {
        FinishPrinting(pickUpObject);
    }

    public override void OnNetworkFinishedEvent()
    {
        FinishPrinting();
    }

    public override void StopInteractible()
    {
        CancelPrinting();
    }
}
