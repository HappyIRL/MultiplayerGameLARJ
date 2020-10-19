using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer), typeof(AudioSource)), Serializable]
public class Telephone : Interactable
{
    [Header("Telephone")]
    [Header("Ringing")]
    [SerializeField] private float _ringingTimeInSecs = 20f;
    [SerializeField] private float _lightSwitchTimeInSecs = 1f;
    [SerializeField] private InteractableObjectID _interactableID;

    [Header("References")]
    [SerializeField] private Material _redScreenMaterial = null;
    [SerializeField] private Material _standardScreenMaterial = null;
    [SerializeField] private AudioClip _ringingSound = null;

    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private bool _callAnswered = false;
    private Coroutine _lastCoroutine;

    public override void Awake()
    {
        base.Awake();
        InteractableID = _interactableID;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _ringingSound;
        
    }
    public override void Start()
    {
        base.Start();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void StartTelephoneRinging()
    {
        _audioSource.Play();
        _callAnswered = false;
        _lastCoroutine = StartCoroutine(TelephoneRingingCoroutine());
    }

    private IEnumerator TelephoneRingingCoroutine()
    {
        float timer = 0f;
        float lightSwitchTimer = _lightSwitchTimeInSecs;
        bool lightIsNormal = true;

        while (timer <= _ringingTimeInSecs)
        {
            timer += Time.deltaTime;
            if (timer >= lightSwitchTimer)
            {
                lightSwitchTimer = timer + _lightSwitchTimeInSecs;

                if (lightIsNormal)
                {
                    ChangeMaterial(_redScreenMaterial);
                    lightIsNormal = false;
                }
                else
                {
                    ChangeMaterial(_standardScreenMaterial);
                    lightIsNormal = true;
                }
            }
            yield return null;
        }
        if (!_callAnswered)
        {
            TaskManager.TaskManagerSingelton.OnTaskFailed(GetComponent<Task>());
        }
        EndCall();
    }

    private void EndCall()
    {
        _audioSource.Stop();
        _callAnswered = true;
        ChangeMaterial(_standardScreenMaterial);
        DisableButtonHints();
    }

    private void ChangeMaterial(Material material)
    {
        Material[] materials = _meshRenderer.materials;
        materials[_meshRenderer.materials.Length - 1] = material;
        _meshRenderer.materials = materials;
    }

    public void AnswerCall()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        EndCall();
        ChangeMaterial(_standardScreenMaterial);
    }

    public override void HoldingFailedEvent()
    {
        EndCall();
    }

    public override void HoldingFinishedEvent()
    {
        AnswerCall();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }

    public override void StartInteractible()
    {
        StartTelephoneRinging();
    }

    public override void StopInteractible()
    {
        EndCall();
    }

    public override void OnNetworkHoldingFinishedEvent()
    {
        AnswerCall();
    }
}
