using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeaEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _speedParticles = null;
    [SerializeField] private ParticleSystem _dashParticles = null;
    [SerializeField] private ParticleSystem _badParticles = null;

    public void PlaySpeedParticles()
    {
        _speedParticles.Play();
    }
    public void PlayDashParticles()
    {
        _dashParticles.Play();
    }
    public void PlayBadParticles()
    {
        _badParticles.Play();
    }


    public void StopSpeedParticles()
    {
        _speedParticles.Stop();
    }
    public void StopDashParticles()
    {
        _dashParticles.Stop();
    }
    public void StopBadParticles()
    {
        _badParticles.Stop();
    }
}
