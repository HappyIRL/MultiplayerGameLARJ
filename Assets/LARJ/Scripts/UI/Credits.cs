using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private Transform _creditText = null;
    [SerializeField] private Transform _creditsStartPosition = null;
    [SerializeField] private Transform _creditsEndPosition = null;
    [SerializeField] private float _scrollSpeed = 5f;

    private bool _startMoving = false;

    private void Update()
    {
        if (_startMoving)
        {
            _creditText.position += Vector3.up * Time.deltaTime * _scrollSpeed;

            if (_creditText.position.y >= _creditsEndPosition.position.y) StartCredits();
        }
    }
    public void StartCredits()
    {
        _creditText.position = _creditsStartPosition.position;
        _startMoving = true;
    }
    public void StopCredits()
    {
        _startMoving = false;
    }
}
