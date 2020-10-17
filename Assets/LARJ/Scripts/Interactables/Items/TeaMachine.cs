using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeaMachine : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _teaMachineSound = null;

    private void Start()
    {
        _audioSource.clip = _teaMachineSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Mug mug = other.GetComponentInChildren<Mug>();
            if (mug != null)
            {
                mug.FillMug(_audioSource);
                _audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Mug mug = other.GetComponentInChildren<Mug>();
            if (mug != null)
            {
                mug.StopFillingMug();
                _audioSource.Stop();
            }
        }
    }
}
