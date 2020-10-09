using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class WaterCooler : Interactable
{
    [Header("WaterCooler")]
    [SerializeField] private AudioClip _waterDrippingSound = null;
    [SerializeField] private ParticleSystem _waterParticles = null;
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;
    private AudioSource _audioSource;

	public override void Awake()
	{
		base.Awake();
        InteractableID = (InteractableObjectID)_interactableID;
    }

	public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _waterDrippingSound;
    }

    private void PlayParticles()
    {
        if (!_waterParticles.isEmitting)
        {
            _waterParticles.Play();
            _audioSource.Play();
            StartCoroutine(WaitForParticles());
        }
    }
    private IEnumerator WaitForParticles()
    {
        yield return new WaitForSeconds(_waterParticles.main.duration);
        _audioSource.Stop();
    }


    public override void PressEvent()
    {
        PlayParticles();
    }


    public override void StartInteractible()
    {
        enabled = true;
    }

    public override void StopInteractible()
    {
        enabled = false;
    }
}
