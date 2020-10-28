using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Mug : Interactable
{
    [Header("Tea")]
    [SerializeField] private Transform _teaImageCanvas = null;
    [SerializeField] private Transform _topFillPoint = null;
    [SerializeField] private Transform _bottomFillPoint = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private AudioClip _drinkingSound = null;

    [Header("Progressbar")]
    [SerializeField] private GameObject _progressbarBG = null;
    [SerializeField] private Image _progressbar = null;

    private Coroutine _fillMugCoroutine;
    private AudioSource _audioSource;
    private bool _isSomethingIn = false;
    private SFXManager _sFXManager;
    [SerializeField] private InteractableObjectID _interactableID;

    public override void Awake()
    {
        base.Awake();
        InteractableID = _interactableID;
        AlwaysInteractable = true;
    }

    public override void Start()
    {
        base.Start();
        _teaImageCanvas.position = _bottomFillPoint.position;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _drinkingSound;
        _sFXManager = SFXManager.Instance;
        DisableUI();
    }

    #region tea drinking
    public void FillMug(AudioSource teaMachineAudioSource)
    {
        _fillMugCoroutine = StartCoroutine(FillMugCoroutine(teaMachineAudioSource));
        _isSomethingIn = true;
    }
    public void StopFillingMug()
    {
        if (_fillMugCoroutine != null)
        {
            StopCoroutine(_fillMugCoroutine);
        }
        DisableUI();
    }
    private void StartDrinking()
    {
        _animator.enabled = true;
        _animator.Play("Drinking");

        if (_isSomethingIn)
        {
            _sFXManager.PlaySound(_audioSource, _drinkingSound);
            ApplyRandomTeaEffect();
        }

        _animator.SetBool("StopDrinking", false);
        _animator.SetBool("StartDrinking", true);
        _isSomethingIn = false;
    }


    private void StopDrinking()
    {
        _sFXManager.StopAudioSource(_audioSource);
        _animator.SetBool("StartDrinking", false);
        _animator.SetBool("StopDrinking", true);

        _teaImageCanvas.position = _bottomFillPoint.position;
    }
    private IEnumerator FillMugCoroutine(AudioSource teaMachineAudioSource)
    {
        float maxDistance = Mathf.Abs(_bottomFillPoint.position.y - _topFillPoint.position.y);
        UpdateUI(Mathf.Abs(_teaImageCanvas.position.y - _bottomFillPoint.position.y) / maxDistance);

        while (_teaImageCanvas.position.y < _topFillPoint.position.y)
        {
            _teaImageCanvas.position = new Vector3(_teaImageCanvas.position.x, Mathf.MoveTowards(_teaImageCanvas.position.y, _topFillPoint.position.y, Time.deltaTime * 0.1f), _teaImageCanvas.position.z);
            UpdateUI(Mathf.Abs(_teaImageCanvas.position.y - _bottomFillPoint.position.y) / maxDistance);
            yield return null;
        }

        teaMachineAudioSource.Stop();
        DisableUI();
    }
    #endregion

    private void UpdateUI(float progress)
    {
        _progressbar.gameObject.SetActive(true);
        _progressbarBG.SetActive(true);
        _progressbar.fillAmount = progress;
    }
    private void DisableUI()
    {
        _progressbar.gameObject.SetActive(false);
        _progressbarBG.SetActive(false);
    }
    public override void MousePressEvent()
    {
        StartDrinking();
    }
    public override void MouseReleaseEvent()
    {
        StopDrinking();
    }

    #region tea effects
    private void ApplyRandomTeaEffect()
    {
        if (PlayerWhoPickedThisUp == null) return;

        PlayerMovement playerMovement = PlayerWhoPickedThisUp.GetComponent<PlayerMovement>();

        if (UnityEngine.Random.value > 0.5f)
        {
            //good effect
            if (UnityEngine.Random.value > 0.5f)
            {
                playerMovement.ApplySpeedEffect();
            }
            else
            {
                playerMovement.ApplyDashEffect();
            }
        }
        else
        {
            //bad effect
            playerMovement.ApplyBadEffect();
        }
    }
   
    #endregion
}
