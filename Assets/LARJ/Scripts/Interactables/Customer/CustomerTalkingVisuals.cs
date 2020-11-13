using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTalkingVisuals : MonoBehaviour
{
    [Header("Talking")]
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private ParticleSystem _talkingParticles = null;
    [SerializeField] private AudioClip[] _talkingSounds = null;
    private bool _isTalking = false;
    private Coroutine _talkingCoroutine;

    public void ActivateTalkingVisuals()
    {
        _audioSource.volume = 0.05f;
        _isTalking = true;
        _talkingCoroutine = StartCoroutine(PlayTalkingSoundsCoroutine());

        _talkingParticles.gameObject.SetActive(true);
        _talkingParticles.Play();
    }
    public void DeactivateTalkingVisuals()
    {
        if (_talkingCoroutine != null) StopCoroutine(_talkingCoroutine);

        _talkingParticles.Stop();
        _talkingParticles.gameObject.SetActive(false);
        _isTalking = false;

    }
    private IEnumerator PlayTalkingSoundsCoroutine()
    {
        while (_isTalking)
        {
            if (!_audioSource.isPlaying)
            {
                AudioClip clip = _talkingSounds[Random.Range(0, _talkingSounds.Length)];
                SFXManager.Instance.PlaySound(_audioSource, clip);

                yield return new WaitForSeconds(clip.length + 0.1f);
            }
        }
    }
}
