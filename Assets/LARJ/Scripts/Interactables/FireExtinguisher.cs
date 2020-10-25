﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class FireExtinguisher : Interactable
{
    [SerializeField] private int _interactableID;
    [Header("FireExtinguisher")]
    [SerializeField] private ParticleSystem _foamParticles = null;
    [SerializeField] private AudioClip _extinguishSound = null;

    private AudioSource _audioSource;
    private bool _isExtinguishing = false;

    public override void Awake()
    {
        base.Awake();
        InteractableID = InteractableObjectID.FireExtinguisher;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _extinguishSound;
        AlwaysInteractable = true;
    }

    private void StartExtinguishing()
    {
        _audioSource.Play();
        _foamParticles.Play();

        _isExtinguishing = true;

        StartCoroutine(ExtinguishCoroutine());
    }
    private void StopExtinguishing()
    {
        _audioSource.Stop();
        _foamParticles.Stop();

        _isExtinguishing = false;
    }

    private IEnumerator ExtinguishCoroutine()
    {
        RaycastHit hit;

        while (_isExtinguishing)
        {
            if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 5f))
            {
                if (hit.collider.tag == "Fire")
                {
                    Fire fire = hit.collider.gameObject.GetComponent<Fire>();

                    fire.TryToExtinguish(2f * Time.deltaTime);
                }
            }

            yield return null;
        }
    }


    public override void MousePressEvent()
    {
        StartExtinguishing();
    }

    public override void MouseReleaseEvent()
    {
        StopExtinguishing();
    }

    public override void StartInteractible()
    {
        throw new NotImplementedException();
    }

    public override void StopInteractible()
    {
        throw new NotImplementedException();
    }
}
