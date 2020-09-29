using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PooledObject))]
public class Garbage : MonoBehaviour, IObjectPoolNotifier
{
    [SerializeField] private ObjectPool _garbagePool = null;
    [SerializeField] private int _strokesToClean = 5;
    [SerializeField] private Image _healthbar = null;
    [SerializeField] private Image _healthbarBackground = null;
    private int _strokes = 0;

    public void Clean()
    {
        _strokes--;
        UpdateHealthbar();

        if (_strokes <= 0)
        {
            _garbagePool.ReturnObject(gameObject);
        }
    }

    private void UpdateHealthbar()
    {
        _healthbar.gameObject.SetActive(true);
        _healthbarBackground.gameObject.SetActive(true);
        _healthbar.fillAmount = (float)((float)_strokes / (float)_strokesToClean);
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

        _strokes = _strokesToClean;
        UpdateHealthbar();
        _healthbar.gameObject.SetActive(false);
        _healthbarBackground.gameObject.SetActive(false);
    }
}
