using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBillboarding : MonoBehaviour
{
    void Update()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}
