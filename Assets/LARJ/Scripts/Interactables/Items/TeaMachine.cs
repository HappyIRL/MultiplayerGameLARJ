using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeaMachine : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _teaMachineSound = null;
    private SFXManager _sFXManager;

    private void Start()
    {
        _audioSource.clip = _teaMachineSound;
        _sFXManager = SFXManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Mug mug = other.GetComponentInChildren<Mug>();
            if (mug != null)
            {
                mug.FillMug(_audioSource);
                _sFXManager.PlaySound(_audioSource, _teaMachineSound);
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
                _sFXManager.StopAudioSource(_audioSource);
            }
        }
    }
}
