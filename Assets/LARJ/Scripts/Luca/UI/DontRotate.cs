using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontRotate : MonoBehaviour
{
    [SerializeField] private float _yOffset;
    [SerializeField] private Transform _parent;

    private void LateUpdate()
    {
        transform.position = _parent.position + Vector3.up * _yOffset;
    }
}
