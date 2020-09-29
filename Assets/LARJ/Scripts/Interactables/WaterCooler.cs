using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class WaterCooler : MonoBehaviour
{
    [SerializeField] private AudioClip _waterDrippingSound = null;
    [SerializeField] private ParticleSystem _waterParticles = null;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _waterDrippingSound;
    }

    public void PlayParticles()
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
}
