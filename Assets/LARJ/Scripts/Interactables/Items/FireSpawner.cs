using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private int _difficulty = 1;
    [SerializeField] private ObjectPool _firePool = null;
    [SerializeField] private float _xSize = 10f;
    [SerializeField] private float _zSize = 10f;
    [SerializeField] private LayerMask _wallLayerMask;

    private float _spawnChance;
    private float _timer = 0f;

    public int Difficulty 
    { 
        get => _difficulty; 
        set
        {
            _difficulty = value;
            SetSpawnChance();
        }
    }

    private void Awake()
    {
        SetSpawnChance();
    }
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= 5f)
        {
            _timer = 0f;

            if (Random.value <= _spawnChance)
            {
                SpawnFireAtRandomPosition();
            }
        }
    }
    private void SetSpawnChance()
    {
        _spawnChance = 0.025f * _difficulty;
    }

    public void SpawnFireAtRandomPosition()
    {
        GameObject obj = _firePool.GetObject();
        obj.GetComponent<Fire>().FireSpawner = this;

        Vector3 pos;
        do
        {
            float x = Random.Range(transform.position.x + _xSize / 2, transform.position.x - _xSize / 2);
            float y = transform.position.y;
            float z = Random.Range(transform.position.z + _zSize / 2, transform.position.z - _zSize / 2);
            pos = new Vector3(x,y,z);

        } while (Physics.CheckSphere(pos, 1f, _wallLayerMask));

        obj.transform.position = pos;
    }
    public void SpawnFireAt(Vector3 position)
    {
        GameObject obj = _firePool.GetObject();
        obj.GetComponent<Fire>().FireSpawner = this;

        obj.transform.position = position;
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
