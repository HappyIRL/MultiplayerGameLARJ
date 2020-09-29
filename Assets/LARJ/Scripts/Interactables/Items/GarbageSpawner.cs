using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool _garbagePool = null;
    [SerializeField] private float _xSize = 10f;
    [SerializeField] private float _zSize = 10f;
    [SerializeField] private LayerMask _wallLayerMask;

    public void SpawnGarbageAtRandomPosition()
    {
        GameObject obj = _garbagePool.GetObject();

        Vector3 pos;
        do
        {
            float x = Random.Range(transform.position.x + _xSize / 2, transform.position.x - _xSize / 2);
            float y = transform.position.y;
            float z = Random.Range(transform.position.z + _zSize / 2, transform.position.z - _zSize / 2);
            pos = new Vector3(x, y, z);

        } while (Physics.CheckSphere(pos, 1f, _wallLayerMask));

        obj.transform.position = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        //left-bottom to left-top
        Vector3 from = transform.position + Vector3.left * (_xSize / 2) + Vector3.back * (_zSize / 2);
        Vector3 to = transform.position + Vector3.left * (_xSize / 2) + Vector3.forward * (_zSize / 2);
        Gizmos.DrawLine(from, to);

        //left-top to right-top
        from = to;
        to += Vector3.right * _xSize;
        Gizmos.DrawLine(from, to);

        //right-top to right-bottom
        from = to;
        to += Vector3.back * _zSize;
        Gizmos.DrawLine(from, to);

        //right-bottom to left-bottom
        from = to;
        to += Vector3.left * _xSize;
        Gizmos.DrawLine(from, to);
    }
}
