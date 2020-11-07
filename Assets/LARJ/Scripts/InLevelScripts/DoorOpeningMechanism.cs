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
                StartCoroutine(OpenDoorToOutside());
            }
            else
            {
                StartCoroutine(OpenDoorToInside());
            }
        }
        else //person is outside the building
        {
            if (_doorState == DoorState.Closed || _doorState == DoorState.DoorIsOpeningToInside || _doorState == DoorState.DoorIsClosingFromInside)
            {
                StartCoroutine(OpenDoorToInside());
            }
            else
            {
                StartCoroutine(OpenDoorToOutside());
            }
        }
    }
    private void CloseDoor()
    {
        if (_doorState == DoorState.OpenAtInside || _doorState == DoorState.DoorIsOpeningToInside || _doorState == DoorState.DoorIsClosingFromInside)
        {
            StartCoroutine(CloseDoorFromInside());
        }
        else if (_doorState == DoorState.OpenAtOutside || _doorState == DoorState.DoorIsOpeningToOutside || _doorState == DoorState.DoorIsClosingFromOutside)
        {
            StartCoroutine(CloseDoorFromOutside());
        }
    }

    private IEnumerator OpenDoorToInside()
    {
        _doorState = DoorState.DoorIsOpeningToInside;
        _canCheck = false;

        while (_door1.eulerAngles.y < 90)
        {
            float rotation = _rotationSpeed * Time.deltaTime;
            _door1.Rotate(new Vector3(0, 0, rotation));
            _door2.Rotate(new Vector3(0, 0, -rotation));

            yield return null;
        }

        _doorState = DoorState.OpenAtInside;
        _canCheck = true;
        StartCoroutine(WaitToCheckAgain());     
    }
    private IEnumerator OpenDoorToOutside()
    {
        _doorState = DoorState.DoorIsOpeningToOutside;
        _canCheck = false;

        while (_door2.eulerAngles.y < 90)
        {
            float rotation = _rotationSpeed * Time.deltaTime;
            _door1.Rotate(new Vector3(0, 0, -rotation));
            _door2.Rotate(new Vector3(0, 0, rotation));

            yield return null;
        }

        _doorState = DoorState.OpenAtOutside;
        _canCheck = true;
        StartCoroutine(WaitToCheckAgain());      
    }

    private IEnumerator CloseDoorFromInside()
    {
        _doorState = DoorState.DoorIsClosingFromInside;
        _canCheck = false;

        while (_door1.eulerAngles.y < 100)
        {
            float rotation = _rotationSpeed * Time.deltaTime;
            _door1.Rotate(new Vector3(0, 0, -rotation));
            _door2.Rotate(new Vector3(0, 0, rotation));

            yield return null;
        }

        _door1.eulerAngles = new Vector3(_door1.eulerAngles.x, 0, _door1.eulerAngles.z);
        _door2.eulerAngles = new Vector3(_door2.eulerAngles.x, 0, _door2.eulerAngles.z);

        _canCheck = true;
        _doorState = DoorState.Closed;     
    }
    private IEnumerator CloseDoorFromOutside()
    {
        _doorState = DoorState.DoorIsClosingFromOutside;
        _canCheck = false;

        while (_door2.eulerAngles.y < 100)
        {
            float rotation = _rotationSpeed * Time.deltaTime;
            _door1.Rotate(new Vector3(0, 0, rotation));
            _door2.Rotate(new Vector3(0, 0, -rotation));

            yield return null;
        }

        _door1.eulerAngles = new Vector3(_door1.eulerAngles.x,0,_door1.eulerAngles.z);
        _door2.eulerAngles = new Vector3(_door2.eulerAngles.x,0,_door2.eulerAngles.z);

        _canCheck = true;
        _doorState = DoorState.Closed;
    }

    private IEnumerator WaitToCheckAgain()
    {
        _canCheck = false;
        yield return new WaitForSeconds(0.5f);
        _canCheck = true;
    }
}
