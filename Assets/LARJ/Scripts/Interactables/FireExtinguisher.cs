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
    [SerializeField] private LayerMask _fireLayer;

    private AudioSource _audioSource;
    private bool _isExtinguishing = false;
    private SFXManager _sFXManager;

    public override void Awake()
    {
        base.Awake();
        InteractableID = InteractableObjectID.FireExtinguisher;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _extinguishSound;
        AlwaysInteractable = true;
    }
    public override void Start()
    {
        base.Start();
        _sFXManager = SFXManager.Instance;
    }

    private void StartExtinguishing()
    {
        _sFXManager.PlaySound(_audioSource, _extinguishSound);
        _foamParticles.Play();

        _isExtinguishing = true;

        StartCoroutine(ExtinguishCoroutine());
    }
    private void StopExtinguishing()
    {
        _sFXManager.StopAudioSource(_audioSource);
        _foamParticles.Stop();

        _isExtinguishing = false;
    }

    private IEnumerator ExtinguishCoroutine()
    {
        while (_isExtinguishing)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, 6f, _fireLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.tag == "Fire")
                {
                    Fire fire = hits[i].collider.gameObject.GetComponent<Fire>();

                    fire.TryToExtinguish(2f * Time.deltaTime);
                }
            }
           
            yield return null;
        }
    }


    public override void MousePressEvent()
    {
        base.MousePressEvent();

        StartExtinguishing();
    }

    public override void MouseReleaseEvent()
    {
        base.MouseReleaseEvent();

        StopExtinguishing();
    }
}
