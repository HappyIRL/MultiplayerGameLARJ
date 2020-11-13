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
    private SFXManager _sFXManager;

	public override void Awake()
	{
		base.Awake();
        InteractableID = InteractableObjectID.WaterCooler;
        AlwaysInteractable = true;
    }

	public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _waterDrippingSound;
        _sFXManager = SFXManager.Instance;
    }

    private void PlayParticles()
    {
        if (!_waterParticles.isEmitting)
        {
            _waterParticles.Play();
            _sFXManager.PlaySound(_audioSource, _waterDrippingSound);
            StartCoroutine(WaitForParticles());
        }
    }
    private IEnumerator WaitForParticles()
    {
        yield return new WaitForSeconds(_waterParticles.main.duration);
        _sFXManager.StopAudioSource(_audioSource);
    }


    public override void PressEvent()
    {
        base.PressEvent();

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
