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

    public override void Start()
    {
        base.Start();
        _teaImageCanvas.position = _bottomFillPoint.position;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _drinkingSound;
        DisableUI();
    }

    public void FillMug()
    {
        _fillMugCoroutine = StartCoroutine(FillMugCoroutine());
    }
    public void StopFillingMug()
    {
        if (_fillMugCoroutine != null)
        {
            StopCoroutine(_fillMugCoroutine);
        }
    }
    private void StartDrinking()
    {
        _audioSource.Play();
        _animator.SetBool("StopDrinking", false);
        _animator.SetBool("StartDrinking", true);
    }
    private void StopDrinking()
    {
        _audioSource.Stop();
        _animator.SetBool("StartDrinking", false);
        _animator.SetBool("StopDrinking", true);

        _teaImageCanvas.position = _bottomFillPoint.position;
    }
    private IEnumerator FillMugCoroutine()
    {
        float maxDistance = Mathf.Abs(_teaImageCanvas.position.y - _topFillPoint.position.y);

        while (_teaImageCanvas.position.y < _topFillPoint.position.y)
        {
            _teaImageCanvas.position = new Vector3(_teaImageCanvas.position.x, Mathf.Lerp(_teaImageCanvas.position.y, _topFillPoint.position.y, Time.deltaTime), _teaImageCanvas.position.z);
            UpdateUI(_teaImageCanvas.position.y/ maxDistance);
            yield return null;
        }
    }
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
}
