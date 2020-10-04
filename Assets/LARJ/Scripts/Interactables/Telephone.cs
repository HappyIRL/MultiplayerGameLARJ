using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer), typeof(AudioSource)), Serializable]
public class Telephone : Interactable
{
    [Header("Telephone")]
    [Header("Ringing")]
    [SerializeField] private float _ringingTimeInSecs = 20f;
    [SerializeField] private float _lightSwitchTimeInSecs = 1f;
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;

    [Header("References")]
    [SerializeField] private Material _redScreenMaterial = null;
    [SerializeField] private Material _standardScreenMaterial = null;
    [SerializeField] private AudioClip _ringingSound = null;

    [Header("Events")]
    [SerializeField] private UnityEvent _failedToAnswerEvent;
    [SerializeField] private UnityEvent _completedEvent;

    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private bool _callAnswered = false;
    private Coroutine _lastCoroutine;


    public override void Start()
    {
        base.Start();
        interactableID = (InteractableObjectID)_interactableID;
        _meshRenderer = GetComponent<MeshRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _ringingSound;
    }

    public void StartTelephoneRinging()
    {
        _audioSource.Play();
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
            _failedToAnswerEvent.Invoke();
        }

        EndCall();
    }

    private void EndCall()
    {
        _audioSource.Stop();
        ChangeMaterial(_standardScreenMaterial);
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

        _completedEvent.Invoke();
    }

    public override void HoldingStartedEvent()
    {
        
    }

    public override void HoldingFailedEvent()
    {
        EndCall();
    }

    public override void HoldingFinishedEvent()
    {
        AnswerCall();
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

    public override void EnableInteractible()
    {
        enabled = true;
        StartTelephoneRinging();
        //Whast else do I need here?
        
    }

    public override void DisableInteractible()
    {
        EndCall();
        enabled = false;
        //What else needs to be disabled?
    }
}
