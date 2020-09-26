using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class Shotgun : MonoBehaviour
{
    [SerializeField] private AudioClip _shotgunShootSound = null;

    private AudioSource _audioSource;
    private bool _canShoot = true;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _shotgunShootSound;
    }

    public void Shoot()
    {
        if (_canShoot)
        {
            _canShoot = false;
            _audioSource.Play();

            //Spawn bullets from pool

            StartCoroutine(WaitToShoot());
        }
    }

    public IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(2f);
        _canShoot = true;
    }

}
