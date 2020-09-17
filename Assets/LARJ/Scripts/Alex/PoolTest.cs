using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    [SerializeField] private ObjectPool _pool;

    IEnumerator Start()
    {
        while (true)
        {
            var go = _pool.GetObject();

            go.transform.position = new Vector3(Random.Range(1,5), 0, Random.Range(1, 5));

            var delay = Random.Range(0.1f, 5f);

            yield return new WaitForSeconds(delay);

        }
    }
}
