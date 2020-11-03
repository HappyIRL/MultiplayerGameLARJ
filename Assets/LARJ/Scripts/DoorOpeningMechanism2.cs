using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpeningMechanism2 : MonoBehaviour
{
    [SerializeField] private float _distanceToOpen = 2f;
    [SerializeField] private float _openingSpeed = 100f;
    [SerializeField] private Transform _door1 = null;
    [SerializeField] private Transform _door2 = null;

    private List<Transform> _customers = new List<Transform>();
    private bool _canCheck = true;

    private Vector3 _closedPositionDoor1;
    private Vector3 _closedPositionDoor2;

    private Coroutine _door1Coroutine;
    private Coroutine _door2Coroutine;

    private void Awake()
    {
        _closedPositionDoor1 = _door1.position;
        _closedPositionDoor2 = _door2.position;
    }

    private void Update()
    {
        if (_canCheck)
        {
            bool customerIsNearby = false;

            for (int i = 0; i < _customers.Count; i++)
            {
                if (Vector3.Distance(_customers[i].position, transform.position) <= _distanceToOpen)
                {
                    customerIsNearby = true;
                    OpenDoor();
                    
                    break;
                }
            }

            if (!customerIsNearby)
            {
                CloseDoor();                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Customer")
        {
            if (!_customers.Contains(other.transform))
            {
                _customers.Add(other.transform);
            }
        }
    }


    private void OpenDoor()
    {
        if (_door1Coroutine != null) StopCoroutine(_door1Coroutine);
        if (_door2Coroutine != null) StopCoroutine(_door2Coroutine);

        _door1Coroutine = StartCoroutine(MoveDoor1(_door1.position + _door1.forward * -3f));
        _door2Coroutine = StartCoroutine(MoveDoor2(_door2.position + _door2.forward * -3f));
    }
    private void CloseDoor()
    {
        if (_door1Coroutine != null) StopCoroutine(_door1Coroutine);
        if (_door2Coroutine != null) StopCoroutine(_door2Coroutine);

        _door1Coroutine = StartCoroutine(MoveDoor1(_closedPositionDoor1));
        _door2Coroutine = StartCoroutine(MoveDoor2(_closedPositionDoor2));
    }

    private IEnumerator MoveDoor1(Vector3 ToPosition)
    {
        while (Vector3.Distance(_door1.position, ToPosition) > 0.1f)
        {
            _door1.position = Vector3.MoveTowards(_door1.position, ToPosition, _openingSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator MoveDoor2(Vector3 ToPosition)
    {
        while (Vector3.Distance(_door2.position, ToPosition) > 0.1f)
        {
            _door2.position = Vector3.MoveTowards(_door2.position, ToPosition, _openingSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
