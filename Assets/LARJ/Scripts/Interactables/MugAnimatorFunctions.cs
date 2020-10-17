using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugAnimatorFunctions : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;

    public void DisableAnimator()
    {
        _animator.enabled = false;
    }
}
