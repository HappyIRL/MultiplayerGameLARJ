using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class DoorOpeningMechanism : MonoBehaviour
{
    private enum DoorState
    {
        OpenAtInside,
        OpenAtOutside,
        Closed,
        DoorIsOpeningToInside,
        DoorIsOpeningToOutside,
        DoorIsClosingFromInside,
        DoorIsClosingFromOutside
    }

    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Transform _door1 = null;
    [SerializeField] private Transform _door2 = null;

    private int _insideCounter = 0;
    private Coroutine _lastCoroutine;

    private DoorState _doorState = DoorState.Closed;

    private void OnTriggerEnter(Collider other)
    {
        _insideCounter++;

        OpenDoor(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        _insideCounter--;

        if (_insideCounter == 0)
        {
            CloseDoor(other.transform);
        }
    }

    private void OpenDoor(Transform person)
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        if (person.position.z > transform.position.z) //person is inside the building
        {
            if (_doorState == DoorState.OpenAtInside || _doorState == DoorState.DoorIsOpeningToInside || _doorState == DoorState.DoorIsClosingFromInside)
            {
                _lastCoroutine = StartCoroutine(OpenDoorToInsideCoroutine());
            }
            else
            {
                _lastCoroutine = StartCoroutine(OpenDoorToOutsideCoroutine());
            }
        }
        else //person is outside the building
        {
            if (_doorState == DoorState.OpenAtOutside || _doorState == DoorState.DoorIsOpeningToOutside || _doorState == DoorState.DoorIsClosingFromOutside)
            {
                _lastCoroutine = StartCoroutine(OpenDoorToOutsideCoroutine());
            }
            else
            {
                _lastCoroutine = StartCoroutine(OpenDoorToInsideCoroutine());
            }
        }
    }
    private void CloseDoor(Transform person)
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        if (_doorState == DoorState.OpenAtInside || _doorState == DoorState.DoorIsOpeningToInside)
        {
            _lastCoroutine = StartCoroutine(CloseDoorFromInsideCoroutine());
        }
        else if (_doorState == DoorState.OpenAtOutside || _doorState == DoorState.DoorIsOpeningToOutside)
        {
            _lastCoroutine = StartCoroutine(CloseDoorFromOutsideCoroutine());
        }
    }

    private IEnumerator OpenDoorToInsideCoroutine()
    {
        while (_door1.rotation.eulerAngles.y < 90 || _door2.rotation.eulerAngles.y > -90)
        {
            _doorState = DoorState.DoorIsOpeningToInside;

            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return null;
        }
        _doorState = DoorState.OpenAtInside;
    }
    private IEnumerator OpenDoorToOutsideCoroutine()
    {
        while (_door1.rotation.eulerAngles.y > -90 || _door2.rotation.eulerAngles.y < 90)
        {
            _doorState = DoorState.DoorIsOpeningToOutside; 

            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return null;
        }
        _doorState = DoorState.OpenAtOutside;
    }

    private IEnumerator CloseDoorFromInsideCoroutine()
    {
        while (_door1.rotation.eulerAngles.y > 0 || _door2.rotation.eulerAngles.y < 0)
        {
            _doorState = DoorState.DoorIsClosingFromInside;

            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return null;
        }
        _doorState = DoorState.Closed;
    }
    private IEnumerator CloseDoorFromOutsideCoroutine()
    {
        while (_door1.rotation.eulerAngles.y < 0 || _door2.rotation.eulerAngles.y > 0)
        {
            _doorState = DoorState.DoorIsClosingFromOutside;

            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return null;
        }
        _doorState = DoorState.Closed;
    }
}
