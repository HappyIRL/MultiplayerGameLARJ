using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Interactables))]
public class Printer : MonoBehaviour
{
    [Header("Paper")]
    [SerializeField] private GameObject _paperPrefab = null;
    [SerializeField] private Transform _paperSpawnPoint = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip _printerInSound = null;
    [SerializeField] private AudioClip _printerInProgressSound = null;
    [SerializeField] private AudioClip _printerOutSound = null;

    private AudioSource _audioSource;
    private Coroutine _lastCoroutine;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void StartPrinting()
    {
        _lastCoroutine = StartCoroutine(StartPrintingSoundCoroutine());
    }
    private IEnumerator StartPrintingSoundCoroutine()
    {
        _audioSource.loop = false;
        PlaySound(_printerInSound);
        yield return new WaitForSeconds(_printerInSound.length);
        PlaySound(_printerInProgressSound);
        _audioSource.loop = true;
    }

    public void FinishPrinting()
    {
        StopCoroutine(_lastCoroutine);
        PlaySound(_printerOutSound);
        _audioSource.loop = false;

        Instantiate(_paperPrefab, _paperSpawnPoint.position, _paperSpawnPoint.rotation);
    }
    public void CancelPrinting()
    {
        StopCoroutine(_lastCoroutine);
        _audioSource.Stop();
        _audioSource.loop = false;
    }

}
