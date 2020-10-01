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

    private bool _holdingButton = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;
    private PlayerInput _playerInput;

    private InteractionType InteractableInteractionType = InteractionType.PickUp;
    private bool _canInteract = false;
    private bool _isPickedUp = false;

    //Object to interact
    private Interactable _objectToInteract;
    public Interactable ObjectToInteract
    {
        get { return _objectToInteract; }
        set
        {
            if (_objectToInteract != value)
            {
                DeselectOldObject();
                SelectNewObject(value);
            }

        }
    }
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
                _objectToInteract.HoldingFinishedEvent();
                _holdingTimeBar.fillAmount = 0;
            }
        }
    }

    #region Trigger Detection
    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null)
        {
            if (!_isPickedUp)
            {
                ObjectToInteract = interactable;
                InteractableInteractionType = interactable.InteractionType;
                _canInteract = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null)
        {
            if (ObjectToInteract == null)
            {
                ObjectToInteract = interactable;
                InteractableInteractionType = interactable.InteractionType;
                _canInteract = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null)
        {
            if (!_isPickedUp)
            {
                interactable.DisableButtonHints();
                _canInteract = true;

                if (interactable == ObjectToInteract)
                {
                    ObjectToInteract = null;
                }
            }
        }
    }
    #endregion

    private void SelectNewObject(Interactable value)
    {
        _objectToInteract = value;

        if (_objectToInteract != null)
        {
            if (!_isPickedUp)
            {
                _objectToInteract.EnableButtonHints(_playerInput.currentControlScheme);
            }
        }
    }

    private void DeselectOldObject()
    {
        if (_objectToInteract != null)
        {
            _objectToInteract.DisableButtonHints();

            if (_objectToInteract.CanInteractWhenPickedUp)
            {
                _objectToInteract.DisablePickedUpButtonHints();
            }
        }
    }

    #region Input Events
    public void OnPress()
    {
        if (_canInteract)
        {
            if (_objectToInteract == null) return;

            if (InteractableInteractionType == InteractionType.PickUp)
            {               
                if (_isPickedUp)
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
        if (_canInteract)
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

                _objectToInteract.HoldingFailedEvent();
                _objectToInteract.EnableButtonHints(_playerInput.currentControlScheme);
            }
        }

        _holdingButton = false;
        _holdingTimer = 0f;
        _holdingTimeBarBG.SetActive(false);
    }
    public void OnPickedUpInteractionPress()
    {
        if (_canInteract)
        {
            if (InteractableInteractionType == InteractionType.PickUp)
            {
                if (_isPickedUp)
                {
                    if (_objectToInteract == null) return;

                    if (_objectToInteract.CanInteractWhenPickedUp)
                    {
                        _objectToInteract.MousePressEvent();
                        _objectToInteract.DisablePickedUpButtonHints();
                    }
                }
            }
        }
    }
    public void OnPickedUpInteractionRelease()
    {
        if (_canInteract)
        {
            if (InteractableInteractionType == InteractionType.PickUp)
            {
                if (_objectToInteract == null) return;

                if (_objectToInteract.CanInteractWhenPickedUp)
                {
                    _objectToInteract.MouseReleaseEvent();

                    if (_isPickedUp)
                    {
                        _objectToInteract.EnablePickedUpButtonHints(_playerInput.currentControlScheme);
                    }
                }
            }
        }
    }
    #endregion

    #region Interaction Events
    private void PickUp()
    {
        if (_objectToInteract == null) return;

        _isPickedUp = true;
        _objectToInteract.Rb.Sleep();
        _objectToInteract.transform.rotation = _objectHolder.rotation;
        _objectToInteract.transform.position = _objectHolder.position;
        _objectToInteract.transform.parent = _objectHolder;
        _objectToInteract.DisableButtonHints();
        _objectToInteract.DisableColliders();

        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.EnablePickedUpButtonHints(_playerInput.currentControlScheme);
        }
    }
    private void Drop()
    {
        if (_objectToInteract == null) return;
        
        _isPickedUp = false;
        _objectToInteract.transform.parent = null;
        _objectToInteract.Rb.WakeUp();
        _objectToInteract.EnableColliders();

        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.DisablePickedUpButtonHints();
        }

        _objectToInteract = null;
    }
    private void PressInteraction()
    {
        if (_objectToInteract == null) return;
        _objectToInteract.PressEvent();
    }
    private void HoldingInteraction()
    {
        if (_objectToInteract == null) return;

        _holdingButton = true;
        _holdingWasFinished = false;
        _holdingTimeBar.fillAmount = 0;
        _holdingTimeBarBG.SetActive(true);
        _objectToInteract.HoldingStartedEvent();
        _objectToInteract.DisableButtonHints();
    }
    #endregion
}
