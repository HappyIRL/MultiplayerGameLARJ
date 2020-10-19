using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), Serializable]
public class Shotgun : Interactable
{
    [Header("Shotgun")]
    [SerializeField] private float _bulletCount = 6f;
    [SerializeField] private Transform _bulletSpawnPoint = null;
    [SerializeField] private ObjectPool _shotgunBulletPool = null;
    [SerializeField] private AudioClip _shotgunShootSound = null;
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;

    private AudioSource _audioSource;
    private bool _canShoot = true;

    public override void Awake()
    {
        base.Awake();
        InteractableID = InteractableObjectID.Shotgun;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _shotgunShootSound;
    }

    private void Shoot()
    {
        if (_canShoot)
        {
            _canShoot = false;
            _audioSource.Play();

            for (int i = 0; i < _bulletCount; i++)
            {
                GameObject bullet = _shotgunBulletPool.GetObject();
                bullet.transform.position = _bulletSpawnPoint.position;
                bullet.transform.rotation = _bulletSpawnPoint.rotation;

                Vector3 rotation = bullet.transform.rotation.eulerAngles;
                rotation.x += UnityEngine.Random.Range(-10f, 10f);
                rotation.y += UnityEngine.Random.Range(-10f, 10f);
                bullet.transform.eulerAngles = rotation;

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(bullet.transform.forward * 40f, ForceMode.Impulse);
            }

            StartCoroutine(GunRecoil());
        }
    }

    private IEnumerator GunRecoil()
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


    public override void MousePressEvent()
    {
        Shoot();
    }

}
