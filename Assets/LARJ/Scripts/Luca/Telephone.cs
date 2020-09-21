using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer), typeof(AudioSource), typeof(Interactables))]
public class Telephone : MonoBehaviour
{
    [Header("Ringing")]
    [SerializeField] private float _ringingTimeInSecs = 20f;
    [SerializeField] private float _lightSwitchTimeInSecs = 1f;

    [Header("References")]
    [SerializeField] private Material _redScreenMaterial = null;
    [SerializeField] private Material _standardScreenMaterial = null;
    [SerializeField] private AudioClip _ringingSound = null;

    [Header("Events")]
    public UnityEvent FailedToAnswerEvent;
    public UnityEvent CompletedEvent;

    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private bool _callAnswered = false;
    private Coroutine _lastCoroutine;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _ringingSound;

        StartTelephoneRinging();
    }

    public void StartTelephoneRinging()
    {
        _audioSource.Play();
        _lastCoroutine = StartCoroutine(TelephoneRingingCoroutine());
    }

    private IEnumerator TelephoneRingingCoroutine()
    {
        float timer = 0f;
        float lightSwitchTimer = _lightSwitchTimeInSecs;
        bool lightIsNormal = true;

        while (timer <= _ringingTimeInSecs)
        {
            timer += Time.deltaTime;

            if (timer >= lightSwitchTimer)
            {
                lightSwitchTimer = timer + _lightSwitchTimeInSecs;

                if (lightIsNormal)
                {
                    ChangeMaterial(_redScreenMaterial);
                    lightIsNormal = false;
                }
                else
                {
                    ChangeMaterial(_standardScreenMaterial);
                    lightIsNormal = true;
                }
            }

            yield return null;
        }

        if (!_callAnswered)
        {
            FailedToAnswerEvent.Invoke();
        }

        EndCall();
    }

    private void EndCall()
    {
        _audioSource.Stop();
        ChangeMaterial(_standardScreenMaterial);
    }

    private void ChangeMaterial(Material material)
    {
        Material[] materials = _meshRenderer.materials;
        materials[_meshRenderer.materials.Length - 1] = material;
        _meshRenderer.materials = materials;
    }

    public void AnswerCall()
    {
        StopCoroutine(_lastCoroutine);
        EndCall();
        ChangeMaterial(_standardScreenMaterial);

        CompletedEvent.Invoke();
    }
}
