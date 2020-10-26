using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontMove : MonoBehaviour
{
    [SerializeField] private Transform _parent = null;
    [SerializeField] private Vector3 _offsetPosition = Vector3.zero;

    void LateUpdate()
    {
        transform.position = _parent.position + _offsetPosition;
    }
}
