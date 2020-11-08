using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cracker : MonoBehaviour, IObjectPoolNotifier 
{
    public ObjectPool crackerPool;
    [HideInInspector] public CrackerSpawner CrackerSpawner = null;
    [SerializeField] private Animator _animator = null;

    [Header("Progressbar")]
    [SerializeField] private Image _progressbar = null;
    [SerializeField] private Image _progressbarBG = null;

    private int _timeToCrack = 10;
    private int _timer = 0;

    private Coroutine _crackingCoroutine;
    
    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        _timer = 0;
        _animator.Play("SafeCracker");
        UpdateProgressbar();
    }
    public void OnEnqueuedToPool()
    {
        
    }

    public void StartCracking()
    {
        _crackingCoroutine = StartCoroutine(CrackingCoroutine());
        ActivateProgressbar();
    }
    private IEnumerator CrackingCoroutine()
    {
        while (_timer <= _timeToCrack)
        {
            yield return new WaitForSeconds(1);
            _timer++;
            UpdateProgressbar();
        }

        CrackerSpawner.AllStealed = true;
        DeactivateProgressbar();
        StealMoney();
        StartEscapeAnimation();
    }
    private void StealMoney()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + transform.forward, transform.forward, out hit))
        {
            if (hit.collider.gameObject.tag == "Safe")
            {
                SafeMoneyCounter safeMoneyCounter = hit.collider.gameObject.GetComponentInChildren<SafeMoneyCounter>();
                if (safeMoneyCounter != null) safeMoneyCounter.StealAllMoney();
            }
        }
    }
    private void StartEscapeAnimation()
    {
        _animator.SetTrigger("Escape");
    }
    public void OnEscaped()
    {
        StopCracker();
    }

    public void StopCracker()
    {
        if (_crackingCoroutine != null) StopCoroutine(_crackingCoroutine);
        DeactivateProgressbar();
        CrackerSpawner.CloseHole();
        crackerPool.ReturnObject(this.gameObject);
    }
    private void UpdateProgressbar()
    {
        _progressbar.fillAmount = (float)_timer / (float)_timeToCrack;
    }
    private void ActivateProgressbar()
    {
        _progressbar.gameObject.SetActive(true);
        _progressbarBG.gameObject.SetActive(true);
    }
    private void DeactivateProgressbar()
    {
        _progressbar.gameObject.SetActive(false);
        _progressbarBG.gameObject.SetActive(false);
    }

}
