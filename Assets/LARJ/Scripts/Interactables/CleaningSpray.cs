using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CleaningSpray : Interactable
{
    [Header("CleaningSpray")]
    [SerializeField] private ParticleSystem _sprayParticles = null;
    [SerializeField] private Transform _cleaningPoint = null;
    [SerializeField] private AudioClip _spraySound = null;
    [SerializeField] private LayerMask _cleaningLayer;

    private AudioSource _audioSource;

    public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _spraySound;
    }

    private void Clean()
    {
        _sprayParticles.Play();
        _audioSource.Play();

        RaycastHit hit;
        if (Physics.SphereCast(_cleaningPoint.position, 1f, _cleaningPoint.forward,out hit, 5f, _cleaningLayer))
        {
            Destroy(hit.collider.gameObject);
        }
    }

    public override void MousePressEvent()
    {
        Clean();
    }
}
