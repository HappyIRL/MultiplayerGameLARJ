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
    [SerializeField] private Image _holdingTimeBar = null;
    [SerializeField] private GameObject _holdingTimeBarBG = null;

    private bool _holdingButton = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;
    private PlayerInput _playerInput;

    private InteractionType InteractableInteractionType = InteractionType.PickUp;
    private bool _canInteract = false;
    private bool _isPickedUp = false;
    private bool _isLocal = true;

    public delegate void LARJInteractableUseEvent(InteractableObjectID id, InteractableUseType type, int objectInstanceID);
    public event LARJInteractableUseEvent LARJInteractableUse;
    public delegate void LARJTaskEvent(InteractableObjectID id, LARJTaskState state, int objectInstanceID);
    public event LARJTaskEvent OnNetworkTaskEvent;

    public List<Interactable> AllowedInteractibles = new List<Interactable>();
    private int _objectInstanceID;

    //Object to interact
    private Interactable _objectToInteract;
    private Interactable _duplicator;
    private LARJConnectToPhoton _larjConnectToPhoton;

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

        if (PhotonNetwork.IsConnected)
            _isLocal = false;
        else
            _isLocal = true;
    }
    private void Start()
    {
        _holdingTimeBarBG.SetActive(false);
        _larjConnectToPhoton = FindObjectOfType<LARJConnectToPhoton>();
    }

    private void ActivateInteractable(Interactable interactable, LARJTaskState state)
    {
            switch(state)
			{
                case LARJTaskState.TaskComplete:
                    if (AllowedInteractibles.Contains(interactable))
                    {
                        OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.ObjectInstanceID);
                        AllowedInteractibles.Remove(interactable);
                    }
                    break;
                case LARJTaskState.TaskFailed:
                    if (AllowedInteractibles.Contains(interactable))
                    {
                        OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.ObjectInstanceID);
                        AllowedInteractibles.Remove(interactable);
                    }
                    break;
                case LARJTaskState.TaskStart:
                    if (!AllowedInteractibles.Contains(interactable))
                    {
                        AllowedInteractibles.Add(interactable);
                        OnNetworkTaskEvent?.Invoke(interactable.InteractableID, state, interactable.ObjectInstanceID);
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
            _holdingTimeBar.fillAmount = _holdingTimer / objectToHold.HoldingTime;

            if (_holdingTimer >= objectToHold.HoldingTime)
            {
                _holdingTimer = 0f;
                _holdingTimeBarBG.SetActive(false);
                _holdingButton = false;
                _holdingWasFinished = true;

                if (_isPickedUp) objectToHold.HoldingFinishedEvent(_objectToInteract.gameObject);
                else objectToHold.HoldingFinishedEvent();
              
                LARJInteractableUse?.Invoke(objectToHold.InteractableID, InteractableUseType.HoldFinish, objectToHold.ObjectInstanceID);
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
                if (AllowedInteractibles.Contains(interactable))
                {
                    ObjectToInteract = interactable;
                    InteractableInteractionType = interactable.InteractionType;
                    _canInteract = true;
                }
            }
            else if (other.tag == "Printer")
            {
                _duplicator = interactable;
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
                if (AllowedInteractibles.Contains(interactable))
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
                LARJInteractableUse?.Invoke(objectToRelease.InteractableID, InteractableUseType.HoldFailed, objectToRelease.ObjectInstanceID);
                objectToRelease.EnableButtonHints(_playerInput.currentControlScheme);
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
                        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.MousePress, _objectToInteract.ObjectInstanceID);
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
                    LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.MouseRelease, _objectToInteract.ObjectInstanceID);

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
        _objectToInteract.PickUpObject(_objectHolder);

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.PickUp, _objectToInteract.ObjectInstanceID);

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

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.Drop, _objectToInteract.ObjectInstanceID);

        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.DisablePickedUpButtonHints();
        }

        _objectToInteract = null;
    }
    private void PressInteraction()
    {
        if (_objectToInteract == null) return;

        LARJInteractableUse?.Invoke(_objectToInteract.InteractableID, InteractableUseType.Press, _objectToInteract.ObjectInstanceID);
        _objectToInteract.PressEvent();
    }

    private void HoldingInteraction(Interactable objectToInteract)
    {
        if (_objectToInteract == null) return;

        _holdingButton = true;
        _holdingWasFinished = false;
        _holdingTimeBar.fillAmount = 0;
        _holdingTimeBarBG.SetActive(true);
        objectToInteract.HoldingStartedEvent();
        objectToInteract.DisableButtonHints();
        LARJInteractableUse?.Invoke(objectToInteract.InteractableID, InteractableUseType.HoldStart, objectToInteract.ObjectInstanceID);
    }
    #endregion
}
