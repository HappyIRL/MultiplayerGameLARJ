using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool _pool;

   
}
public static class PooledGameObjectExtensions
{   
    public static void ReturnToPool(this GameObject go)
    {

        var pooledObject = go.GetComponent<PooledObject>();

        if(pooledObject == null)
        {
            //object never came from a pool
            return;
        }
        pooledObject._pool.ReturnObject(go);
    }        
}