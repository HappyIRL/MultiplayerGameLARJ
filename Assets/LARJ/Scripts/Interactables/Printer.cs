using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource)), Serializable]
public class Printer : Interactable
{
    [Header("Paper")]
    [Header("Printer")]
    [SerializeField] private GameObject _paperPrefab = null;
    [SerializeField] private List<Transform> _possiblePrinterOutputPoints = null;
    private Transform _printerOutputPoint = null;

    [Header("References")]
    [SerializeField] private GameObject _healtbarCanvasPrefab = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip _printerInSound = null;
    [SerializeField] private AudioClip _printerInProgressSound = null;
    [SerializeField] private AudioClip _printerOutSound = null;

    private AudioSource _audioSource;
    private Coroutine _lastCoroutine;
    private GameObject _papergameObject;
    private SFXManager _sFXManager;

	public override void Awake()
	{
		base.Awake();
        InteractableID = InteractableObjectID.Printer;
        AlwaysInteractable = true;
    }

	public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _sFXManager = SFXManager.Instance;
        SetPrinterOutputPoint();
    }
    private void SetPrinterOutputPoint()
    {
        for (int i = 0; i < _possiblePrinterOutputPoints.Count; i++)
        {
            if (transform.position.x < _possiblePrinterOutputPoints[i].position.x)
            {
                if (CheckIfPointIsValid(_possiblePrinterOutputPoints[i].position, Vector3.right))
                {
                    _printerOutputPoint = _possiblePrinterOutputPoints[i];
                    return;
                }
            }
            else if (transform.position.x > _possiblePrinterOutputPoints[i].position.x)
            {
                if (CheckIfPointIsValid(_possiblePrinterOutputPoints[i].position, Vector3.left))
                {
                    _printerOutputPoint = _possiblePrinterOutputPoints[i];
                    return;
                }
            }
            else if (transform.position.z < _possiblePrinterOutputPoints[i].position.z)
            {
                if (CheckIfPointIsValid(_possiblePrinterOutputPoints[i].position, Vector3.forward))
                {
                    _printerOutputPoint = _possiblePrinterOutputPoints[i];
                    return;
                }
            }
            else if (transform.position.z > _possiblePrinterOutputPoints[i].position.z)
            {
                if (CheckIfPointIsValid(_possiblePrinterOutputPoints[i].position, Vector3.back))
                {
                    _printerOutputPoint = _possiblePrinterOutputPoints[i];
                    return;
                }
            }
        }
    }
    private bool CheckIfPointIsValid(Vector3 position, Vector3 direction)
    {
        position.y = 0.5f;

        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit))
        {
            if (Vector3.Distance(position, hit.point) < 2f)
            {
                return false;
            }
        }

        return true;
    }

    private void PlaySound(AudioClip clip)
    {
        _sFXManager.PlaySound(_audioSource, clip);
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

        GameObject paper = InstantiateManager.Instance.Instantiate(_paperPrefab, _printerOutputPoint.position, _printerOutputPoint.rotation);


        if(paper != null)
		{
            if(UnityEngine.Random.value < 0.5f) TaskManager.TaskManagerSingelton.StartTask(paper.GetComponent<Task>());
            PlaySound(_printerOutSound);
            _audioSource.loop = false;
            DisableButtonHints();
		}
    }
    private void FinishPrinting(GameObject objectToSpawn)
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        GameObject go = InstantiateManager.Instance.Instantiate (objectToSpawn, _printerOutputPoint.position, _printerOutputPoint.rotation);

        if(go != null)
		{
            PlaySound(_printerOutSound);
            _audioSource.loop = false;
            InstantiateManager.Instance.SpawnGarbageHealthbar(go);
            DisableButtonHints();
        }

    }

    private void CancelPrinting()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
        _sFXManager.StopAudioSource(_audioSource);
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

    public override void OnNetworkHoldingFinishedEvent()
    {
        FinishPrinting();
    }

    public override void StopInteractible()
    {
        CancelPrinting();
    }
}
