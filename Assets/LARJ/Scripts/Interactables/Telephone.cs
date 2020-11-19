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
    [SerializeField] private GameObject _redScreenImage = null;
    [SerializeField] private AudioClip _ringingSound = null;

    private AudioSource _audioSource;
    private bool _callAnswered = false;
    private Coroutine _lastCoroutine;
    private SFXManager _sFXManager;
    float _timer = 0f;

    public override void Awake()
    {
        base.Awake();
        InteractableID = _interactableID;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _ringingSound;
        AlwaysInteractable = false;
        
    }
    public override void Start()
    {
        base.Start();
        _sFXManager = SFXManager.Instance;
    }

    public void StartTelephoneRinging()
    {
        _sFXManager.PlaySound(_audioSource, _ringingSound);
        _callAnswered = false;
        _timer = 0f;
        _lastCoroutine = StartCoroutine(TelephoneRingingCoroutine());
    }

    private IEnumerator TelephoneRingingCoroutine()
    {
        float lightSwitchTimer = _lightSwitchTimeInSecs;
        bool lightIsNormal = true;

        while (_timer <= _ringingTimeInSecs)
        {
            _timer += Time.deltaTime;
            if (_timer >= lightSwitchTimer)
            {
                lightSwitchTimer = _timer + _lightSwitchTimeInSecs;

                if (lightIsNormal)
                {
                    ChangeMaterial(false);
                    lightIsNormal = false;
                }
                else
                {
                    ChangeMaterial(true);
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
        _sFXManager.StopAudioSource(_audioSource);
        ChangeMaterial(false);
        DisableButtonHints();
    }

    private void ChangeMaterial(bool activateRedScreen)
    {
        if (activateRedScreen)
        {
            _redScreenImage.SetActive(true);
        }
        else
        {
            _redScreenImage.SetActive(false);
        }
    }

    public void AnswerCall()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
        _callAnswered = true;

        EndCall();
        ChangeMaterial(false);
    }

    public override void HoldingStartedEvent()
    {
        base.HoldingStartedEvent();

        Task task = GetComponent<Task>();
        task.StopTaskCoolDown();
        task.TaskUI.StopTaskUITimer();
        if (_lastCoroutine != null) StopCoroutine(_lastCoroutine);
    }
    public override void HoldingFailedEvent()
    {
        base.HoldingFailedEvent();

        Task task = GetComponent<Task>();
        task.StartTaskCooldown();
        task.TaskUI.StartTaskUITimer();
        _lastCoroutine = StartCoroutine(TelephoneRingingCoroutine());

        EndCall();
    }

    public override void HoldingFinishedEvent()
    {
        base.HoldingFinishedEvent();

        AnswerCall();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
        TaskManager.TaskManagerSingelton.StartRandomFollowUpTask();
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
        base.HoldingFinishedEvent();

        AnswerCall();
    }
}
