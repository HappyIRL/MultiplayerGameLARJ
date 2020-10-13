using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PooledObject))]
public class Garbage : MonoBehaviour, IObjectPoolNotifier
{
    public int StrokesToClean = 5;
    [SerializeField] private ObjectPool _garbagePool = null;
    [SerializeField] private Image _healthbar = null;
    [SerializeField] private Image _healthbarBackground = null;
    private int _strokes = 0;
    private float _timeToDeactivateUI = 3f;
    private Coroutine _lastCoroutine;

    public void Clean()
    {
        if (_lastCoroutine != null) StopCoroutine(_lastCoroutine);

        _strokes--;
        UpdateHealthbar();

        _lastCoroutine = StartCoroutine(WaitToDeactiveCoroutine());

        if (_strokes <= 0)
        {
            if (_garbagePool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _garbagePool.ReturnObject(gameObject);
            }
        }
    }

    private IEnumerator WaitToDeactiveCoroutine()
    {
        yield return new WaitForSeconds(_timeToDeactivateUI);
        DeactivateUI();
    }

    public void SetImages(Image healthbar, Image healthbarBackground)
    {
        _healthbar = healthbar;
        _healthbarBackground = healthbarBackground;
        DeactivateUI();

        _strokes = StrokesToClean;
    }

    private void DeactivateUI()
    {
        _healthbar.fillAmount = 1;
        _healthbar.gameObject.SetActive(false);
        _healthbarBackground.gameObject.SetActive(false);
    }

    private void UpdateHealthbar()
    {
        _healthbar.gameObject.SetActive(true);
        _healthbarBackground.gameObject.SetActive(true);
        _healthbar.fillAmount = (float)((float)_strokes / (float)StrokesToClean);
    }

    public void OnEnqueuedToPool()
    {

    }

    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (created)
        {
            _garbagePool = GetComponent<PooledObject>()._pool;
        }

        _strokes = StrokesToClean;
        UpdateHealthbar();
        _healthbar.gameObject.SetActive(false);
        _healthbarBackground.gameObject.SetActive(false);
    }
}
