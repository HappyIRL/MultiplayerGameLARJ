using System;
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

            //TODO: Spawn bullets from pool

            StartCoroutine(GunRecoil());
        }
    }

    public IEnumerator GunRecoil()
    {
        float originAngle = transform.eulerAngles.x;
        float targetAngle = originAngle - 10f;

        float upSpeed = 50f * Time.deltaTime;
        float downSpeed = 5f * Time.deltaTime;

        float angle = transform.eulerAngles.x;

        //go up
        while (angle > targetAngle)
        {
            angle = Mathf.MoveTowardsAngle(angle, targetAngle, upSpeed);
            transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z); 
            yield return null;
        }

        //go down
        while (angle < originAngle)
        {
            angle = Mathf.MoveTowardsAngle(angle, originAngle, downSpeed);
            transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);
            yield return null;
        }

        _canShoot = true;
    }

}
