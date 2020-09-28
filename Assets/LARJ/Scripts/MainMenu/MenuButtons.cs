using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuButtons : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClickSound = null;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _buttonClickSound;
    }

    public void ExitGame()
    {
        Debug.Log("Click");
        _audioSource.Play();
        Application.Quit();
    }
}
