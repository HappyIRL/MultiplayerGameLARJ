using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeOpener : MonoBehaviour
{
    [SerializeField] private Transform _door = null;
    private bool _doorIsClosed = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        if (_doorIsClosed)
        {
            StartCoroutine(OpenDoorCoroutine());
        }
    }
    private void CloseDoor()
    {
        StartCoroutine(CloseDoorCoroutine());      
    }

    private IEnumerator OpenDoorCoroutine()
    {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.up, new Vector3(1f, -1f, 0));

        while (Quaternion.Angle(_door.rotation, targetRotation) > 0.01f)
        {
            _door.rotation = Quaternion.RotateTowards(_door.rotation, targetRotation, 5f);
            yield return null;
        }

        _doorIsClosed = false;
    }

    private IEnumerator CloseDoorCoroutine()
    {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.up, new Vector3(0, 1, 0.5f));

        while (Quaternion.Angle(_door.rotation, targetRotation) > 0.01f)
        {
            _door.rotation = Quaternion.RotateTowards(_door.rotation, targetRotation, 5f);
            yield return null;
        }

        _doorIsClosed = true;
    }
}
