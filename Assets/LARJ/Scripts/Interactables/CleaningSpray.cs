using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CleaningSpray : Interactable
{
    [Header("CleaningSpray")]
    [SerializeField] private ParticleSystem _sprayParticles = null;
    [SerializeField] private Transform _cleaningPoint = null;
    [SerializeField] private AudioClip _spraySound = null;
    [SerializeField] private LayerMask _playerLayer;

    private AudioSource _audioSource;
    private SFXManager _sFXManager;

    public override void Awake()
    {
        base.Awake();
        InteractableID = InteractableObjectID.CleaningSpray;
        AlwaysInteractable = true;
    }

    public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _spraySound;
        _sFXManager = SFXManager.Instance;
    }

    private void Clean()
    {
        _sprayParticles.Play();
        _sFXManager.PlaySound(_audioSource, _spraySound);

        RaycastHit hit;
        if (Physics.SphereCast(_cleaningPoint.position, 1f, _cleaningPoint.forward, out hit, 5f, _playerLayer))
        {
            var obj = hit.collider.GetComponent<PlayerMovement>();
            if (obj != null)
            {
                obj.RemoveAllEffects();
            }
        }
    }

    public override void MousePressEvent()
    {
        base.MousePressEvent();

        Clean();
    }
}
