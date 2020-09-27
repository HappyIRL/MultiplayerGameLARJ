using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    [SerializeField] private int _strokesToClean = 5;
    private int _strokes = 0;

    private void OnEnable()
    {
        _strokes = _strokesToClean;
    }

    public void Clean()
    {
        _strokes--;

        if (_strokes <= 0)
        {
            //TODO: return to pool

            Destroy(gameObject);
        }
    }
}
