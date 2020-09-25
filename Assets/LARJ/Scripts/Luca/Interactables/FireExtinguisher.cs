using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] private ParticleSystem _foamParticles = null;
    [SerializeField] private AudioClip _extinguishSound = null;

    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _extinguishSound;
    }

    public void StartExtinguishing()
    {
        _audioSource.Play();
        _foamParticles.Play();
    }
    public void StopExtinguishing()
    {
        _audioSource.Stop();
        _foamParticles.Stop();
    }
}
