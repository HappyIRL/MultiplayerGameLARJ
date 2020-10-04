using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3;
    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashDistance = 25f;
    [SerializeField] private Transform _bodyTransform = null;
    private CharacterController _controller;

    private Vector3 _moveDirection = Vector3.zero;
    private Vector2 _inputVector = Vector2.zero;
    private float _initialYPosition;
    private float _dashTimer = 0f;
    private bool _isDashOnCooldown = false;
    private float _dashSpeed = 10;
    private bool _inDash = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _initialYPosition = transform.position.y;
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
        _moveDirection = new Vector3(_inputVector.x, 0, _inputVector.y);
        if (_moveDirection != Vector3.zero)
        {
            _bodyTransform.forward = _moveDirection * -1;
        }
        _moveDirection = transform.TransformDirection(_moveDirection);
        _controller.Move(_moveDirection * _movementSpeed * Time.deltaTime);
        if (transform.position.y != _initialYPosition)
        {
            Debug.Log("Changed Y.");
            transform.position = new Vector3(transform.position.x, _initialYPosition, transform.position.z);
            //transform.Translate(new Vector3(transform.position.x, _initialYPosition, transform.position.z) - transform.position);
        }
    }
    private void OnDash(InputValue inputValue)
    {
        if (inputValue.isPressed && !_isDashOnCooldown)
        {
            StartCoroutine(Dash());
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
            _controller.Move(_moveDirection * _dashDistance * Time.deltaTime);
            i += Time.deltaTime * _dashSpeed;
            yield return null;
        }
        _inDash = false;
        yield return null;
    }
}
