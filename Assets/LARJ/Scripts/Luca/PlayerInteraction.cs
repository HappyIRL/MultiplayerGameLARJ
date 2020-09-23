using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _objectHolder = null;
    [SerializeField] private Image _holdingTimeBar = null;
    [SerializeField] private GameObject _holdingTimeBarBG = null;

    //Object to interact
    [HideInInspector] public Interactables ObjectToInteract
    {
        get { return _objectToInteract; }
        set
        {
            //if there is another object remove outline
            if (_objectToInteract != value)
            {
                if (_objectToInteract != null)
                {
                    _objectToInteract.OutlineRef.enabled = false;
                    _objectToInteract.DisableButtonHintImages();
                }
            }

            _objectToInteract = value;

            if (_objectToInteract != null)
            {
                _objectToInteract.OutlineRef.enabled = true;
                _objectToInteract.EnableButtonHintImage(_playerInput.currentControlScheme);
            }
        }
    }
    private Interactables _objectToInteract;

    [HideInInspector] public InteractionType InteractableInteractionType = InteractionType.PickUp;
    [HideInInspector] public bool CanInteract = false;
    [HideInInspector] public bool IsPickedUp = false;

    private bool _holdingButton = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        _holdingTimeBarBG.SetActive(false);
    }
    private void Update()
    {
        if (_holdingButton)
        {
            _holdingTimer += Time.deltaTime;
            _holdingTimeBar.fillAmount = _holdingTimer / _objectToInteract.HoldingTime;

            if (_holdingTimer >= _objectToInteract.HoldingTime)
            {
                _holdingTimer = 0f;
                _holdingTimeBarBG.SetActive(false);
                _holdingButton = false;
                _holdingWasFinished = true;
                _objectToInteract.HoldingFinishedInteractionEvent.Invoke();
                _holdingTimeBar.fillAmount = 0;
            }
        }
    }


    public void OnPress()
    {
        if (CanInteract)
        {
            if (_objectToInteract == null) return;

            if (InteractableInteractionType == InteractionType.PickUp)
            {               
                if (IsPickedUp)
                {
                    Drop();
                }
                else
                {
                    PickUp();
                }               
            }
            else if (InteractableInteractionType == InteractionType.Press)
            {
                PressInteraction();                
            }
        }
    }
    public void OnHold()
    {
        if (CanInteract)
        {
            if (InteractableInteractionType == InteractionType.Hold)
            {                  
                HoldingInteraction();                             
            }
        }
    }
    public void OnRelease()
    {
        if (_holdingButton)
        {
            if (!_holdingWasFinished)
            {
                if (_objectToInteract == null) return;

                _objectToInteract.HoldingFailedInteractionEvent.Invoke();
                _objectToInteract.EnableButtonHintImage(_playerInput.currentControlScheme);
            }
        }

        _holdingButton = false;
        _holdingTimer = 0f;
        _holdingTimeBarBG.SetActive(false);
    }

    private void PickUp()
    {
        if (_objectToInteract == null) return;


        IsPickedUp = true;
        _objectToInteract.Rb.Sleep();
        _objectToInteract.transform.parent = _objectHolder;
        _objectToInteract.transform.forward = transform.forward;
        _objectToInteract.transform.position = _objectHolder.position;
        _objectToInteract.DisableButtonHintImages();
        _objectToInteract.OutlineRef.enabled = false;
    }
    private void Drop()
    {
        if (_objectToInteract == null) return;
        
        IsPickedUp = false;
        _objectToInteract.transform.parent = null;
        _objectToInteract.Rb.WakeUp();

        _objectToInteract = null;
    }
    private void PressInteraction()
    {
        if (_objectToInteract == null) return;
        _objectToInteract.PressInteractionEvent.Invoke();
    }
    private void HoldingInteraction()
    {
        if (_objectToInteract == null) return;

        _holdingButton = true;
        _holdingWasFinished = false;
        _holdingTimeBar.fillAmount = 0;
        _holdingTimeBarBG.SetActive(true);
        _objectToInteract.HoldingStartedInteractionEvent.Invoke();
        _objectToInteract.DisableButtonHintImages();
    }
}
