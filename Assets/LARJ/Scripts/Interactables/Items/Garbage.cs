using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PooledObject))]
public class Garbage : MonoBehaviour, IObjectPoolNotifier
{
    public int StrokesToClean = 3;
    [SerializeField] private ObjectPool _garbagePool = null;

    private Image _healthbar = null;
    private Image _healthbarBackground = null;
    private int _strokes = 0;
    private float _timeToDeactivateUI = 3f;
    private Coroutine _lastCoroutine;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Garbage");
    }
    void Start()
    {
        GarbageHealthbar garbageHealthbar = GetComponentInChildren<GarbageHealthbar>();
        _healthbar = garbageHealthbar.Healthbar;
        _healthbarBackground = garbageHealthbar.HealthbarBackground;

        DeactivateUI();
        _strokes = StrokesToClean;
    }

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
