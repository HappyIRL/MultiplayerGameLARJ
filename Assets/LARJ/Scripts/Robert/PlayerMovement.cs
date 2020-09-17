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
    [SerializeField] private float _dashDistance = 2f;

    private CharacterController _controller;

    private Vector3 _moveDirection = Vector3.zero;
    private Vector2 _inputVector = Vector2.zero;
    private float _dashTimer = 0f;
    private bool _isDashOnCooldown = false;
    private float _dashSpeed = 10;
    private bool _inDash = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
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
        _moveDirection = transform.TransformDirection(_moveDirection);
        _controller.Move(_moveDirection * _movementSpeed * Time.deltaTime);
    }
    private void OnDash(InputValue inputValue)
    {
        if (inputValue.isPressed && !_isDashOnCooldown)
        {
            StartCoroutine(Dash());
            //Dash();
        }
    }
    IEnumerator Dash()
    {

        float i = 0;
        _inDash = true;
        while (i <= 1)
        {

            _controller.Move(_moveDirection * _dashDistance * Time.deltaTime);
            i += Time.deltaTime * _dashSpeed;
            yield return null;
        }
        _inDash = false;
        yield return null;
    }
    private void Daxsh()
    {
        _dashTimer = 0f;
        _isDashOnCooldown = true;
        _controller.Move(_moveDirection * _dashDistance);
    }
}
