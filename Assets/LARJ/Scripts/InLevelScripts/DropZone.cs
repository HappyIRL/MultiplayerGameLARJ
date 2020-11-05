using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DropZone : MonoBehaviour
{
    [SerializeField] private Vector2 _dropZoneSize = Vector2.zero;
    private float _height = 0f;
    private Vector2 _halfDropZoneSize;

    private void Awake()
    {
        _halfDropZoneSize = _dropZoneSize * 0.5f;
        _height = transform.position.y;
    }

    public Vector3 GetRandomPositionInsideDropZone()
    {
        float x = Random.Range(transform.position.x - _halfDropZoneSize.x, transform.position.x + _halfDropZoneSize.x);
        float z = Random.Range(transform.position.z - _halfDropZoneSize.y, transform.position.z + _halfDropZoneSize.y);

        return new Vector3(x, _height + 0.1f, z);
    }


    private void OnDrawGizmosSelected()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Vector3 colliderSize = new Vector3(_dropZoneSize.x, 0.05f, _dropZoneSize.y);
        collider.size = colliderSize;

        _height = transform.position.y;
        _halfDropZoneSize = _dropZoneSize * 0.5f;

        Vector3 origin = transform.position;
        origin.y = _height;

        Vector3 leftBottomCorner = origin - transform.right * _halfDropZoneSize.x - transform.forward * _halfDropZoneSize.y;
        Vector3 leftTopCorner = leftBottomCorner + transform.forward * _dropZoneSize.y;
        Vector3 rightTopCorner = leftTopCorner + transform.right * _dropZoneSize.x;
        Vector3 rightBottomCorner = rightTopCorner - transform.forward * _dropZoneSize.y;

        Gizmos.DrawLine(leftBottomCorner, leftTopCorner);
        Gizmos.DrawLine(leftTopCorner, rightTopCorner);
        Gizmos.DrawLine(rightTopCorner, rightBottomCorner);
        Gizmos.DrawLine(rightBottomCorner, leftBottomCorner);
    }
}

