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

    [SerializeField] private float _distanceToOpen = 2f;
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Transform _door1 = null;
    [SerializeField] private Transform _door2 = null;

    private DoorState _doorState = DoorState.Closed;
    private List<Transform> _customers = new List<Transform>();
    private bool _canCheck = true;

    private void FixedUpdate()
    {
        if (_canCheck)
        {
            bool customerIsNearby = false;

            for (int i = 0; i < _customers.Count; i++)
            {
                if (Vector3.Distance(_customers[i].position, transform.position) <= _distanceToOpen)
                {
                    customerIsNearby = true;

                    if (_doorState != DoorState.OpenAtInside && _doorState != DoorState.OpenAtOutside)
                    {
                        OpenDoor(_customers[i]);
                    }
                    break;
                }
            }

            if (!customerIsNearby)
            {
                if (_doorState != DoorState.Closed)
                {
                    CloseDoor();
                }
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
        if (person.position.z > transform.position.z) //person is inside the building
        {
            if (_doorState == DoorState.Closed || _doorState == DoorState.DoorIsOpeningToOutside || _doorState == DoorState.DoorIsClosingFromOutside)
            {
                OpenDoorToOutside();
            }
            else
            {
                OpenDoorToInside();
            }
        }
        else //person is outside the building
        {
            if (_doorState == DoorState.Closed || _doorState == DoorState.DoorIsOpeningToInside || _doorState == DoorState.DoorIsClosingFromInside)
            {
                OpenDoorToInside();
            }
            else
            {
                OpenDoorToOutside();
            }
        }
    }
    private void CloseDoor()
    {
        if (_doorState == DoorState.OpenAtInside || _doorState == DoorState.DoorIsOpeningToInside || _doorState == DoorState.DoorIsClosingFromInside)
        {
            CloseDoorFromInside();
        }
        else if (_doorState == DoorState.OpenAtOutside || _doorState == DoorState.DoorIsOpeningToOutside || _doorState == DoorState.DoorIsClosingFromOutside)
        {
            CloseDoorFromOutside();
        }
    }

    private void OpenDoorToInside()
    {
        _doorState = DoorState.DoorIsOpeningToInside;
        if (_door1.eulerAngles.y < 90 && _door2.eulerAngles.y > -90)
        {
            _door1.rotation = Quaternion.Euler(_door1.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door1.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door2.eulerAngles.z);
        }
        else
        {
            _doorState = DoorState.OpenAtInside;
            StartCoroutine(WaitToCheckAgain());
        }
    }
    private void OpenDoorToOutside()
    {
        _doorState = DoorState.DoorIsOpeningToOutside; 
        if (_door1.eulerAngles.y > -90 && _door2.eulerAngles.y < 90)
        {
            _door1.rotation = Quaternion.Euler(_door1.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.eulerAngles.y, -90, _rotationSpeed * Time.deltaTime), _door1.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.eulerAngles.y, 90, _rotationSpeed * Time.deltaTime), _door2.eulerAngles.z);
        }
        else
        {
            _doorState = DoorState.OpenAtOutside;
            StartCoroutine(WaitToCheckAgain());
        }
    }

    private void CloseDoorFromInside()
    {
        _doorState = DoorState.DoorIsClosingFromInside;
        if (_door1.eulerAngles.y > 0 && _door2.eulerAngles.y - 360 < 0)
        {
            _door1.rotation = Quaternion.Euler(_door1.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.eulerAngles.z);
        }
        else
        {
            _doorState = DoorState.Closed;
        }
    }
    private void CloseDoorFromOutside()
    {
        _doorState = DoorState.DoorIsClosingFromOutside;
        if (_door1.eulerAngles.y < 0 && _door2.eulerAngles.y > 0)
        {
            _door1.rotation = Quaternion.Euler(_door1.eulerAngles.x, Mathf.MoveTowardsAngle(_door1.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door1.eulerAngles.z);
            _door2.rotation = Quaternion.Euler(_door2.eulerAngles.x, Mathf.MoveTowardsAngle(_door2.eulerAngles.y, 0, _rotationSpeed * Time.deltaTime), _door2.eulerAngles.z);
        }
        else
        {
            _doorState = DoorState.Closed;
        }
    }

    private IEnumerator WaitToCheckAgain()
    {
        _canCheck = false;
        yield return new WaitForSeconds(0.5f);
        _canCheck = true;
    }
}
