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


        RaycastHit[] hits = Physics.SphereCastAll(_cleaningPoint.position, 1f, _cleaningPoint.forward, 5f, _playerLayer);
        {
            for (int i = 0; i < hits.Length; i++)
            {
                var obj = hits[i].collider.GetComponent<PlayerMovement>();
                if (obj != null)
                {
                    obj.RemoveAllEffects();
                }           
            }
        }
    }

    public override void MousePressEvent()
    {
        base.MousePressEvent();

        Clean();
    }
}
