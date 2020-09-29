using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] private ParticleSystem _foamParticles = null;
    [SerializeField] private AudioClip _extinguishSound = null;

    private AudioSource _audioSource;
    private bool _isExtinguishing = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _extinguishSound;
    }

    public void StartExtinguishing()
    {
        _audioSource.Play();
        _foamParticles.Play();

        _isExtinguishing = true;

        StartCoroutine(ExtinguishCoroutine());
    }
    public void StopExtinguishing()
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
}
