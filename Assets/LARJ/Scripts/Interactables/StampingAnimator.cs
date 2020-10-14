using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampingAnimator : MonoBehaviour
{
    private Stamp _stamp = null;

    private void Awake()
    {
        _stamp = GetComponentInChildren<Stamp>();
    }
    public void FinishStamping()
    {
        _stamp.FinishStamping();
    }
}
