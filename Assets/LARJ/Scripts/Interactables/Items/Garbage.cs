using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Garbage : MonoBehaviour
{
    [SerializeField] private int _strokesToClean = 5;
    [SerializeField] private Image _healthbar = null;
    [SerializeField] private Image _healthbarBackground = null;
    private int _strokes = 0;

    private void OnEnable()
    {
        _strokes = _strokesToClean;
        UpdateHealthbar();
        _healthbar.gameObject.SetActive(false);
        _healthbarBackground.gameObject.SetActive(false);
    }

    public void Clean()
    {
        _strokes--;
        UpdateHealthbar();

        if (_strokes <= 0)
        {
            //TODO: return to pool

            Destroy(gameObject);
        }
    }

    private void UpdateHealthbar()
    {
        _healthbar.gameObject.SetActive(true);
        _healthbarBackground.gameObject.SetActive(true);
        _healthbar.fillAmount = (float)((float)_strokes / (float)_strokesToClean);
    }
}
