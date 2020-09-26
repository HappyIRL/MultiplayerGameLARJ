using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private float _health = 10f;
    [SerializeField] private Transform _child1;
    [SerializeField] private Transform _child2;

    private float _maxHealth;
    private bool _isExtinguishing = false;
    private Coroutine _lastCoroutine;


    private void Update()
    {
        if (!_isExtinguishing)
        {
            if (_health < _maxHealth)
            {
                _health += 1f * Time.deltaTime;
                _child1.localScale = Vector3.one * (_health / _maxHealth);
                _child2.localScale = Vector3.one * (_health / _maxHealth);
            }
        }
    }

    private void OnEnable()
    {
        _maxHealth = _health;
        _child1.localScale = Vector3.one;
        _child2.localScale = Vector3.one;
    }

    public void TryToExtinguish(float damage)
    {
        _isExtinguishing = true;

        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        _health -= damage;
        _child1.localScale = Vector3.one * (_health / _maxHealth);
        _child2.localScale = Vector3.one * (_health / _maxHealth);

        if (_health <= 0)
        {
            DestroyFire();
        }

        _lastCoroutine = StartCoroutine(WaitToGrowCoroutine());
    }

    private IEnumerator WaitToGrowCoroutine()
    {
        yield return new WaitForSeconds(5f);
        _isExtinguishing = false;
    }

    private void DestroyFire()
    {
        Destroy(gameObject);
    }
}
