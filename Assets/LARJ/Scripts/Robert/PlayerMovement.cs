using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3;
    [SerializeField] private int _playerIndex = 0;

    private CharacterController _controller;

    private Vector3 _moveDirection = Vector3.zero;
    private Vector2 _inputVector = Vector2.zero;

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
        Move();
    }

    private void Move()
    {
        _moveDirection = new Vector3(_inputVector.x, 0, _inputVector.y);
        _moveDirection = transform.TransformDirection(_moveDirection);
        _controller.Move(_moveDirection * _movementSpeed * Time.deltaTime);
    }
}
