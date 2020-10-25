using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum InteractableUseType
{
    PickUp,
    Drop,
    Press,
    HoldStart,
    HoldFailed,
    HoldFinish,
    MousePress,
    MouseRelease
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _objectHolder = null;

    private bool _holdingButton = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;
    private PlayerInput _playerInput;

    private InteractionType InteractableInteractionType = InteractionType.PickUp;
    private bool _canInteract = false;
    private bool _isPickedUp = false;
    private bool _canUseArrowKeys = false;

    public delegate void LARJInteractableUseEvent(InteractableObjectID id, InteractableUseType type, int objectInstanceID, InteractableObjectID itemInHandID);
    public event LARJInteractableUseEvent LARJInteractableUse;
    public delegate void LARJTaskEvent(InteractableObjectID id, LARJTaskState state, int objectInstanceID);
    public event LARJTaskEvent OnNetworkTaskEvent;

    public List<Interactable> AllowedInteractibles = new List<Interactable>();

    //Object to interact
    private Interactable _objectToInteract;
    private Interactable _duplicator;

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
        TaskManager.TaskManagerSingelton.OnTask += ActivateInteractable;
    }

    private void ActivateInteractable(Interactable interactable, LARJTaskState state)
    {
        switch (state)
        {
            case LARJTaskState.TaskComplete:
                if (AllowedInteractibles.Contains(interactable))
                {
                    OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.UniqueInstanceID);
                    AllowedInteractibles.Remove(interactable);
                }
                break;
            case LARJTaskState.TaskFailed:
                if (AllowedInteractibles.Contains(interactable))
                {
                    OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.UniqueInstanceID);
                    AllowedInteractibles.Remove(interactable);
                }
                break;
            case LARJTaskState.TaskStart:
                if (!AllowedInteractibles.Contains(interactable))
                {
                    AllowedInteractibles.Add(interactable);
                    OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.UniqueInstanceID);
                }
                break;
        }
    }

    private void Update()
    {
        if (_holdingButton)
        {
            Interactable objectToHold;

            if (_isPickedUp) objectToHold = _duplicator;
            else objectToHold = _objectToInteract;

            _holdingTimer += Time.deltaTime;
            objectToHold.UpdateProgressbar(_holdingTimer);

            if (_holdingTimer >= objectToHold.HoldingTime)
            {
                objectToHold.DisableProgressbar();

                _holdingTimer = 0f;
                _holdingButton = false;
                _holdingWasFinished = true;

                if (_isPickedUp)
                {
                    objectToHold.HoldingFinishedEvent(_objectToInteract.TransformForPickUp.gameObject);
                }

                else
                {
                    objectToHold.HoldingFinishedEvent();
                }

                LARJInteractableUse?.Invoke(objectToHold.InteractableID, InteractableUseType.HoldFinish, objectToHold.UniqueInstanceID, _objectToInteract.InteractableID);
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
                if (AllowedInteractibles.Contains(interactable) /* true*/)
                {
                    ObjectToInteract = interactable;
                    InteractableInteractionType = interactable.InteractionType;
                    _canInteract = true;
                }
            }
            else if (other.tag == "Printer")
            {
                _duplicator = interactable;
                _duplicator.EnableButtonHints(_playerInput.currentControlScheme);
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
                if (AllowedInteractibles.Contains(interactable)/* true*/)
                {
                    ObjectToInteract = interactable;
                    InteractableInteractionType = interactable.InteractionType;
                    _canInteract = true;

                }
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
                _holdingButton = false;

                if (interactable == ObjectToInteract)
                {
                    ObjectToInteract = null;
                }
            }
            else if (other.tag == "Printer")
            {
                if (_duplicator != null)
                {
                    _duplicator.DisableButtonHints();
                    _duplicator = null;
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
                if (!_canUseArrowKeys)
                {
                    _objectToInteract.EnableButtonHints(_playerInput.currentControlScheme);
                }
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
                    if (_duplicator == null)
                    {
                        Drop();
                    }
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
            else if (InteractableInteractionType == InteractionType.MultiPress)
            {
                MultiPressInteraction();
            }
            else if (InteractableInteractionType == InteractionType.PressTheCorrectKeys)
            {
                PressTheCorrectKeysInteraction();
            }
        }
    }
    public void OnHold()
    {
        if (_canInteract)
        {
            if (InteractableInteractionType == InteractionType.Hold)
            {
                HoldingInteraction(_objectToInteract);
            }
            else if (_isPickedUp)
            {
                if (_duplicator != null)
                {
                    HoldingInteraction(_duplicator);
                }
            }
        }
    }
    public void OnRelease()
    {
        Interactable objectToRelease;

        if (_isPickedUp) objectToRelease = _duplicator;
        else objectToRelease = _objectToInteract;

        if (_holdingButton)
        {
            if (!_holdingWasFinished)
            {
                if (objectToRelease == null) return;

                objectToRelease.HoldingFailedEvent();
                LARJInteractableUse?.Invoke(objectToRelease.InteractableID, InteractableUseType.HoldFailed, objectToRelease.UniqueInstanceID, InteractableObjectID.None);
                objectToRelease.EnableButtonHints(_playerInput.currentControlScheme);

            }

            _holdingButton = false;
            _holdingTimer = 0f;
            objectToRelease.DisableProgressbar();
        }

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
                        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.MousePress, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);
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
                    LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.MouseRelease, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

                    if (_isPickedUp)
                    {
                        _objectToInteract.EnablePickedUpButtonHints(_playerInput.currentControlScheme);
                    }
                }
            }
        }
    }
    public void OnPressUpArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Up, _playerInput.currentControlScheme);
        }
    }
    public void OnPressLeftArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Left, _playerInput.currentControlScheme);
        }
    }
    public void OnPressDownArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Down, _playerInput.currentControlScheme);
        }
    }
    public void OnPressRightArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Right, _playerInput.currentControlScheme);
        }
    }
    #endregion

    #region Interaction Events
    private void PickUp()
    {
        if (_objectToInteract == null) return;

        _isPickedUp = true;
        _objectToInteract.PickUpObject(_objectHolder);

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.PickUp, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.EnablePickedUpButtonHints(_playerInput.currentControlScheme);
        }
    }
    private void Drop()
    {
        if (_objectToInteract == null) return;

        _isPickedUp = false;
        _objectToInteract.DropObject();

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.Drop, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.DisablePickedUpButtonHints();
        }

        _objectToInteract = null;
    }
    private void PressInteraction()
    {
        if (_objectToInteract == null) return;

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.Press, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);
        _objectToInteract.PressEvent();
    }

    private void MultiPressInteraction()
    {
        if (_objectToInteract == null) return;

        _objectToInteract.MultiPressEvent();
    }

    private void HoldingInteraction(Interactable objectToInteract)
    {
        if (_objectToInteract == null) return;

        _holdingButton = true;
        _holdingWasFinished = false;
        _objectToInteract.UpdateProgressbar(0f);
        objectToInteract.HoldingStartedEvent();
        objectToInteract.DisableButtonHints();
        LARJInteractableUse?.Invoke(objectToInteract.InteractableID, InteractableUseType.HoldStart, objectToInteract.UniqueInstanceID, InteractableObjectID.None);
    }
    private void PressTheCorrectKeysInteraction()
    {
        if (_objectToInteract == null) return;

        _objectToInteract.PressTheCorrectKeysStartedEvent(_playerInput.currentControlScheme);
        _canUseArrowKeys = true;
    }
    #endregion
}
