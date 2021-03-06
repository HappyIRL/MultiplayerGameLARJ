﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3;
    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashDistance = 25f;
    [SerializeField] private float _dashSpeed = 10;
    [SerializeField] private Transform _bodyTransform = null;
    [SerializeField] private ParticleSystem _moveParticles = null;
    [SerializeField] private ParticleSystem _dashParticles = null;
    [SerializeField] private AudioClip _dashSound = null;
    [SerializeField] private AudioSource _audioSource = null;

    private BoxCollider _boxCollider;
    private CharacterController _controller;
    private PlayerTeaEffects _playerTeaEffects;

    private Vector3 _moveDirection = Vector3.zero;
    private Vector2 _inputVector = Vector2.zero;
    private float _initialYPosition;
    private float _dashTimer = 0f;
    private bool _isDashOnCooldown = false;
    private bool _inDash = false;
    private int _revertMovementMultiplicator = 1;
    private float _prevSpeed;
    private float _prevDashCooldown;
    private Coroutine _speedEffectCoroutine;
    private Coroutine _dashEffectCoroutine;
    private Coroutine _badEffectCoroutine;

    private SFXManager _sFXManager;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _controller = GetComponent<CharacterController>();
        _playerTeaEffects = GetComponent<PlayerTeaEffects>();
        _initialYPosition = transform.position.y;
        _prevSpeed = _movementSpeed;
        _prevDashCooldown = _dashCooldown;
    }
    private void Start()
    {
        _sFXManager = SFXManager.Instance;     
    }
    public int GetPlayerIndex()
    {
        return _playerIndex;
    }
    private void OnMove(InputValue inputValue)
    {
        _inputVector = inputValue.Get<Vector2>();

    }
    private void Update()
    {

        if (!_inDash)
        {
            Move();
        }
        UpdateDashCooldown();
    }

    private void UpdateDashCooldown()
    {
        if (_isDashOnCooldown)
        {
            _dashTimer += Time.deltaTime;
            if (_dashTimer >= _dashCooldown)
            {
                _isDashOnCooldown = false;
                _dashTimer = 0;
            }
        }
    }

    private void Move()
    {
        _moveDirection = new Vector3(_inputVector.x, 0, _inputVector.y) * _revertMovementMultiplicator;
        if (_moveDirection != Vector3.zero)
        {
            _bodyTransform.forward = _moveDirection * -1;
            _boxCollider.center = _bodyTransform.forward * -1;

            if (!_moveParticles.isPlaying) _moveParticles.Play();
        }
        else
        {
            _moveParticles.Stop();
        }


        _moveDirection = transform.TransformDirection(_moveDirection);
        _controller.Move(_moveDirection * _movementSpeed * Time.deltaTime);
        if (transform.position.y != _initialYPosition)
        {
            transform.position = new Vector3(transform.position.x, _initialYPosition, transform.position.z);
            //transform.Translate(new Vector3(transform.position.x, _initialYPosition, transform.position.z) - transform.position);
        }
    }
    private void OnDash(InputValue inputValue)
    {
        if (inputValue.isPressed && !_isDashOnCooldown)
        {
            StartCoroutine(Dash());
            _dashParticles.Play();
            _audioSource.volume = 0.1f;
            _sFXManager.PlaySound(_audioSource, _dashSound);
        }
    }
    IEnumerator Dash()
    {

        float i = 0;
        _inDash = true;
        _dashTimer = 0f;
        _isDashOnCooldown = true;
        while (i <= 1)
        {
            _moveDirection.y = 0;
            _controller.Move(_moveDirection * _dashDistance * Time.deltaTime);
            i += Time.deltaTime * _dashSpeed;
            yield return null;
        }
        _inDash = false;
        yield return null;
    }

    #region tea effects
    public void ApplySpeedEffect()
    {
        _playerTeaEffects.PlaySpeedParticles();
        if (_speedEffectCoroutine != null)
        {
            StopCoroutine(_speedEffectCoroutine);
        }
        _speedEffectCoroutine = StartCoroutine(WaitToRemoveSpeedEffect());
        _movementSpeed = 6f;
    }
    public void ApplyDashEffect()
    {
        _playerTeaEffects.PlayDashParticles();
        if (_dashEffectCoroutine != null)
        {
            StopCoroutine(_dashEffectCoroutine);
        }
        _dashEffectCoroutine = StartCoroutine(WaitToRemoveDashEffect());
        _dashCooldown = 0.1f;
    }
    public void ApplyBadEffect()
    {
        _playerTeaEffects.PlayBadParticles();
        if (_badEffectCoroutine != null)
        {
            StopCoroutine(_badEffectCoroutine);
        }
        _badEffectCoroutine = StartCoroutine(WaitToRemoveBadEffect());
        _revertMovementMultiplicator = -1;
    }
    public void RemoveAllEffects()
    {
        _movementSpeed = _prevSpeed;
        _dashCooldown = _prevDashCooldown;
        _revertMovementMultiplicator = 1;
        _playerTeaEffects.StopSpeedParticles();
        _playerTeaEffects.StopDashParticles();
        _playerTeaEffects.StopBadParticles();
        if (_speedEffectCoroutine != null)
            StopCoroutine(_speedEffectCoroutine);
        if (_dashEffectCoroutine != null)
            StopCoroutine(_dashEffectCoroutine);
        if (_badEffectCoroutine != null)
            StopCoroutine(_badEffectCoroutine);
    }
    private IEnumerator WaitToRemoveSpeedEffect()
    {
        yield return new WaitForSeconds(30f);
        _movementSpeed = _prevSpeed;
        _playerTeaEffects.StopSpeedParticles();
    }
    private IEnumerator WaitToRemoveDashEffect()
    {
        yield return new WaitForSeconds(30f);
        _dashCooldown = _prevDashCooldown;
        _playerTeaEffects.StopDashParticles();
    }
    private IEnumerator WaitToRemoveBadEffect()
    {
        yield return new WaitForSeconds(30f);
        _revertMovementMultiplicator = 1;
        _playerTeaEffects.StopBadParticles();
    }
    #endregion
}
