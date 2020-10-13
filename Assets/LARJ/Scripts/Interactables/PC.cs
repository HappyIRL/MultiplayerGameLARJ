using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource)), Serializable]
public class PC : Interactable
{
    [Header("PC")]
    [SerializeField] private Image _progressbar = null;
    [SerializeField] private Image _progressbarBackground = null;
    [SerializeField] private List<AudioClip> _singleKeyboardTypingSounds = null;

    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;
    private AudioSource _audioSource;
    private int _pressCount = 0;
    private Coroutine _lastCoroutine;

    public override void Awake()
    {
        base.Awake();
        InteractableID = (InteractableObjectID)_interactableID;
    }

    public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        DisableUI();
    }

    private void DisableUI()
    {
        _progressbar.gameObject.SetActive(false);
        _progressbarBackground.gameObject.SetActive(false);
    }

    private void StartTyping()
    {
        if (_lastCoroutine != null) StopCoroutine(_lastCoroutine);

        _pressCount++;
        _audioSource.clip = _singleKeyboardTypingSounds[UnityEngine.Random.Range(0, _singleKeyboardTypingSounds.Count)];
        _audioSource.Play();
        UpdateUI();
        _lastCoroutine = StartCoroutine(WaitToDisableUI());

        if (_pressCount >= PressCountToFinishTask)
        {
            FinishTyping();
        }
    }

    private void StopTyping()
    {
        _audioSource.Stop();
        DisableUI();
    }

    private void FinishTyping()
    {
        _pressCount = 0;
        StopTyping();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }

    private void UpdateUI()
    {
        _progressbar.gameObject.SetActive(true);
        _progressbarBackground.gameObject.SetActive(true);
        _progressbar.fillAmount = (float)((float)_pressCount / (float)PressCountToFinishTask);
    }

    private IEnumerator WaitToDisableUI()
    {
        yield return new WaitForSeconds(3f);
        DisableUI();
    }

    public override void MultiPressEvent()
    {
        StartTyping();
    }

    public override void OnNetworkFinishedEvent()
    {
        StopTyping();
    }

}

