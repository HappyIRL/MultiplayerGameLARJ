﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PooledObject))]
public class Fire : MonoBehaviour, IObjectPoolNotifier
{
    [HideInInspector] public ObjectPool FirePool = null;

    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _chanceOfSpawningAnotherFire = 0.01f;
    [SerializeField] private Transform _child1;
    [SerializeField] private Transform _child2;
    [SerializeField] private Image _healthbarImage = null;
    [SerializeField] private Image _healthbarBackground = null;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _fireSound = null;

    private float _health;
    private bool _isExtinguishing = false;
    private Coroutine _lastCoroutine;
    private SFXManager _sFXManager;

    private void Awake()
    {
        _audioSource.volume = 0.1f;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
    }
    private void Start()
    {
        _sFXManager = SFXManager.Instance;
    }


    private void Update()
    {
        if (!_isExtinguishing)
        {
            if (_health < _maxHealth)
            {
                _health += 1f * Time.deltaTime;
                _child1.localScale = Vector3.one * (_health / _maxHealth);
                _child2.localScale = Vector3.one * (_health / _maxHealth);

                UpdateHealthbar();
            }
            else
            {
                _healthbarImage.gameObject.SetActive(false);
                _healthbarBackground.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateHealthbar()
    {
        _healthbarImage.gameObject.SetActive(true);
        _healthbarBackground.gameObject.SetActive(true);
        _healthbarImage.fillAmount = _health / _maxHealth;
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

        UpdateHealthbar();

        if (_health <= 0)
        {
            DestroyFire();
        }
        else
        {
            _lastCoroutine = StartCoroutine(WaitToGrowCoroutine());
        }

    }

    private IEnumerator WaitToGrowCoroutine()
    {
        yield return new WaitForSeconds(5f);
        _isExtinguishing = false;
    }

    private void DestroyFire()
    {
        StopFireAudio();
        FirePool.ReturnObject(gameObject);
    }

    public void OnEnqueuedToPool()
    {
        
    }

    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (created)
        {
            FirePool = GetComponent<PooledObject>()._pool;
        }

        _health = _maxHealth;
        _child1.localScale = Vector3.one;
        _child2.localScale = Vector3.one;

        UpdateHealthbar();
        _healthbarImage.gameObject.SetActive(false);
        _healthbarBackground.gameObject.SetActive(false);

        if (_sFXManager == null) _sFXManager = SFXManager.Instance;

        PlayFireAudio();
    }

    private void PlayFireAudio()
    {
        _sFXManager.PlaySound(_audioSource, _fireSound);
    }
    private void StopFireAudio()
    {
        _sFXManager.StopAudioSource(_audioSource);
    }
}
