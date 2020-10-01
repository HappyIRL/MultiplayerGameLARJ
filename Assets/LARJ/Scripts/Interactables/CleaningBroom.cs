using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class CleaningBroom : Interactable
{
    [Header("CleaningBroom")]
    [SerializeField] private AudioClip _cleaningSound = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Transform _broomBottom = null;
    [SerializeField] private LayerMask _garbageLayerMask;

    private AudioSource _audioSource;
    private bool _isCleaning = false;

    public override void Start()
    {
        base.Start();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _cleaningSound;
    }

    //called on animator
    public void CleanGarbage()
    {
        if (_isCleaning)
        {
            Collider[] colliders = Physics.OverlapSphere(_broomBottom.position, 1f, _garbageLayerMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Garbage garbage = colliders[i].GetComponent<Garbage>();
                if (garbage == null) continue;

                garbage.Clean();
            }
        }
    }

    private void StartCleaning()
    {
        _audioSource.Play();
        _animator.SetBool("IsCleaning", true);
        _isCleaning = true;
    }
    private void StopCleaning()
    {
        _audioSource.Stop();
        _animator.SetBool("IsCleaning", false);
        _isCleaning = false;
    }

    public override void HoldingStartedEvent()
    {
        
    }

    public override void HoldingFailedEvent()
    {
        
    }

    public override void HoldingFinishedEvent()
    {
        
    }

    public override void PressEvent()
    {
       
    }

    public override void MousePressEvent()
    {
        StartCleaning();
    }

    public override void MouseReleaseEvent()
    {
        StopCleaning();
    }
}
