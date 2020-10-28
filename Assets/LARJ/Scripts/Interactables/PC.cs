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
    [SerializeField] private List<AudioClip> _singleKeyboardTypingSounds = null;

    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;
    private AudioSource _audioSource;
    private int _pressCount = 0;
    private Coroutine _lastCoroutine;
    private SFXManager _sFXManager;

    public override void Awake()
    {
        base.Awake();
        InteractableID = InteractableObjectID.PC;
    }

    public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _sFXManager = SFXManager.Instance;
        DisableUI();
    }

    private void DisableUI()
    {
        Progressbar.gameObject.SetActive(false);
        ProgressbarBackground.gameObject.SetActive(false);
    }

    private void StartTyping()
    {
        if (_lastCoroutine != null) StopCoroutine(_lastCoroutine);

        _pressCount++;
        AudioClip clip = _singleKeyboardTypingSounds[UnityEngine.Random.Range(0, _singleKeyboardTypingSounds.Count)];
        _sFXManager.PlaySound(_audioSource, clip);
        UpdateUI();
        _lastCoroutine = StartCoroutine(WaitToDisableUI());

        if (_pressCount >= PressCountToFinishTask)
        {
            FinishTyping();
        }
    }

    private void StopTyping()
    {
        _sFXManager.StopAudioSource(_audioSource);
        DisableUI();
    }

    private void FinishTyping()
    {
        _pressCount = 0;
        StopTyping();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
        DisableButtonHints();
    }

    private void UpdateUI()
    {
        Progressbar.gameObject.SetActive(true);
        ProgressbarBackground.gameObject.SetActive(true);
        Progressbar.fillAmount = (float)((float)_pressCount / (float)PressCountToFinishTask);
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

    public override void OnNetworkHoldingFinishedEvent()
    {
        StopTyping();
    }

}

