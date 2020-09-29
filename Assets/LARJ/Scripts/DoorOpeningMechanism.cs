using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] private float _distanceToOpen = 5f;
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Transform _door1 = null;
    [SerializeField] private Transform _door2 = null;

    private int _insideCounter = 0;
    private Coroutine _lastCoroutine;

    private DoorState _doorState = DoorState.Closed;
    private List<Transform> _customers = new List<Transform>();

    private void FixedUpdate()
    {
        bool customerIsNearby = false;

        Debug.Log("Customers: " + _customers.Count);
        for (int i = 0; i < _customers.Count; i++)
        {
            if (Vector3.Distance(_customers[i].position, transform.position) <= _distanceToOpen)
            {
                customerIsNearby = true;

                if (_doorState != DoorState.OpenAtInside && _doorState != DoorState.OpenAtOutside && _doorState != DoorState.DoorIsOpeningToInside && _doorState != DoorState.DoorIsOpeningToOutside)
                {
                    Debug.Log("OPEN DOOR");
                    OpenDoor(_customers[i]);
                }
            }
        }

        if (!customerIsNearby)
        {
            if (_doorState != DoorState.Closed && _doorState != DoorState.DoorIsClosingFromInside && _doorState != DoorState.DoorIsClosingFromOutside)
            {
                Debug.Log("Close Door");
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
    private void CloseDoor()
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
        _doorState = DoorState.DoorIsOpeningToInside;
        while (_door1.rotation.eulerAngles.y < 90 || _door2.rotation.eulerAngles.y > -90)
        {
            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return new WaitForFixedUpdate();
        }
        _doorState = DoorState.OpenAtInside;
    }
    private IEnumerator OpenDoorToOutsideCoroutine()
    {
        _doorState = DoorState.DoorIsOpeningToOutside; 
        while (_door1.rotation.eulerAngles.y > -90 || _door2.rotation.eulerAngles.y < 90)
        {
            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return new WaitForFixedUpdate();
        }
        _doorState = DoorState.OpenAtOutside;
    }

    private IEnumerator CloseDoorFromInsideCoroutine()
    {
        _doorState = DoorState.DoorIsClosingFromInside;
        while (_door1.rotation.eulerAngles.y > 0 || _door2.rotation.eulerAngles.y < 0)
        {
            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return new WaitForFixedUpdate();
        }
        _doorState = DoorState.Closed;
    }
    private IEnumerator CloseDoorFromOutsideCoroutine()
    {
        _doorState = DoorState.DoorIsClosingFromOutside;
        while (_door1.rotation.eulerAngles.y < 0 || _door2.rotation.eulerAngles.y > 0)
        {
            _door1.rotation = Quaternion.Euler(_door1.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.rotation.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.rotation.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.rotation.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.rotation.eulerAngles.z);

            yield return new WaitForFixedUpdate();
        }
        _doorState = DoorState.Closed;
    }
}
