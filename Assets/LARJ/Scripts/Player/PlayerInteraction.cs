using System;
using Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractableUseType
{
    PickUp,
    Drop,
    Press,
    HoldStart,
    HoldFailed,
    HoldFinish,
    MousePress,
    MouseRelease,
    PressTheCorrectKeysFinished,
    PressTheCorrectKeysFailed

}

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _objectHolder = null;
    [SerializeField] private DayManager _dayManager = null;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource = null; 
    [SerializeField] private AudioClip _dropSound = null;
    private SFXManager _sFXManager;

    private bool _holdingButton = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;
    private PlayerInput _playerInput;

    private InteractionType InteractableInteractionType = InteractionType.PickUp;
    private bool _canInteract = false;
    private bool _isPickedUp = false;
    private bool _canUseArrowKeys = false;

    public delegate void LARJInteractableUseEvent(InteractableUseType type, int objectInstanceID, InteractableObjectID itemInHandID);
    public event LARJInteractableUseEvent LARJInteractableUse;

    public event Action<LARJTaskState, int, bool> OnNetworkTaskEvent;

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
    }

    private void Start()
    {
        _sFXManager = SFXManager.Instance;
        TaskManager.TaskManagerSingelton.OnTask += ActivateInteractable;
    }

    private void ActivateInteractable(Interactable interactable, LARJTaskState state, bool stopTask)
    {
        switch (state)
        {
            case LARJTaskState.TaskComplete:
                if (AllowedInteractables.Instance.Interactables.Contains(interactable))
                {
                    if (!interactable.AlwaysInteractable)
                    {
                        AllowedInteractables.Instance.Interactables.Remove(interactable);
                        DisableInteraction(interactable);
                    }
                    OnNetworkTaskEvent?.Invoke(state, interactable.UniqueInstanceID, stopTask);
                }
                break;
            case LARJTaskState.TaskFailed:
                if (AllowedInteractables.Instance.Interactables.Contains(interactable))
                {
                    if (!interactable.AlwaysInteractable)
                    {
                        AllowedInteractables.Instance.Interactables.Remove(interactable);
                        DisableInteraction(interactable);
                    }
                    OnNetworkTaskEvent?.Invoke(state, interactable.UniqueInstanceID, stopTask);
                }
                break;
            case LARJTaskState.TaskStart:
                if (!AllowedInteractables.Instance.Interactables.Contains(interactable))
                {
                    AllowedInteractables.Instance.AddInteractable(interactable);
                }
                OnNetworkTaskEvent?.Invoke(state, interactable.UniqueInstanceID, stopTask);
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
                _holdingTimer = 0f;
                _holdingButton = false;
                _holdingWasFinished = true;

                LARJInteractableUse?.Invoke(InteractableUseType.HoldFinish, objectToHold.UniqueInstanceID, _objectToInteract.InteractableID);

                if (_isPickedUp)
                {
                    objectToHold.HoldingFinishedEvent(_objectToInteract.TransformForPickUp.gameObject);
                }

                else
                {
                    objectToHold.HoldingFinishedEvent();
                }
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
                if (AllowedInteractables.Instance.Interactables.Contains(interactable))
                {
                    ObjectToInteract = interactable;               
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
                if (AllowedInteractables.Instance.Interactables.Contains(interactable))
                {
                    ObjectToInteract = interactable;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null)
        {
            DisableInteraction(interactable);

            if (other.tag == "Printer")
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

    private void DisableInteraction(Interactable interactable)
	{
        if (!_isPickedUp)
        {
            interactable.DisableButtonHints();

            if (interactable == ObjectToInteract)
            {
                if (interactable.InteractionType == InteractionType.Hold)
                {
                    if (!_holdingWasFinished)
                    {
                        interactable.HoldingFailedEvent();
                    }
                    _holdingButton = false;
                }
                ObjectToInteract = null;
            }
        }
        else if (_duplicator != null)
        {
            _duplicator.HoldingFailedEvent();
            _holdingButton = false;
            _duplicator = null;
        }
    }

    private void SelectNewObject(Interactable value)
    {
        _objectToInteract = value;

        if (_objectToInteract != null)
        {
            InteractableInteractionType = value.InteractionType;
            _canInteract = true;

            if (!_isPickedUp)
                _objectToInteract.EnableButtonHints(_playerInput.currentControlScheme);

            if (InteractableInteractionType != InteractionType.PressTheCorrectKeys) _canUseArrowKeys = false;

            _objectToInteract.EnableOutline();
        }
    }

    private void DeselectOldObject()
    {
        if (_objectToInteract != null)
        {
            _objectToInteract.DisableButtonHints();
            _objectToInteract.DisableProgressbar();
            _objectToInteract.DeactivateArrowUI();
            _objectToInteract.DisableOutline();

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
        _holdingTimer = 0f;

        Interactable objectToRelease;

        if (_isPickedUp) objectToRelease = _duplicator;
        else objectToRelease = _objectToInteract;

        if (_holdingButton)
        {
            if (!_holdingWasFinished)
            {
                if (objectToRelease == null) return;

                objectToRelease.HoldingFailedEvent();
                LARJInteractableUse?.Invoke(InteractableUseType.HoldFailed, objectToRelease.UniqueInstanceID, InteractableObjectID.None);
                objectToRelease.EnableButtonHints(_playerInput.currentControlScheme);
            }
            _holdingButton = false;
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
                        LARJInteractableUse?.Invoke(InteractableUseType.MousePress, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);
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
                    LARJInteractableUse?.Invoke(InteractableUseType.MouseRelease, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

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

            CheckIfKeyInteractionFinished(_objectToInteract, _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Up, _playerInput.currentControlScheme));
        }
    }
    public void OnPressLeftArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            CheckIfKeyInteractionFinished(_objectToInteract, _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Left, _playerInput.currentControlScheme));
        }
    }
    public void OnPressDownArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            CheckIfKeyInteractionFinished(_objectToInteract, _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Down, _playerInput.currentControlScheme));
        }
    }
    public void OnPressRightArrow()
    {
        if (_canUseArrowKeys)
        {
            if (_objectToInteract == null) return;

            CheckIfKeyInteractionFinished(_objectToInteract, _objectToInteract.PressCorrectKeyInteraction(CorrectKeysInteraction.Right, _playerInput.currentControlScheme));
        }
    }

    private void CheckIfKeyInteractionFinished(Interactable objectToInteract, bool? x)
	{
        if (x == null) return;

        if(x.Value)
		{
            LARJInteractableUse?.Invoke(InteractableUseType.PressTheCorrectKeysFinished, objectToInteract.UniqueInstanceID, InteractableObjectID.None);
        }
        else
		{
            LARJInteractableUse?.Invoke(InteractableUseType.PressTheCorrectKeysFailed, objectToInteract.UniqueInstanceID, InteractableObjectID.None);
        }
	}

    public void OnPressESC()
    {
        _dayManager.PressESCInteraction();
    }
    #endregion

    #region Interaction Events
    private void PickUp()
    {
        if (_objectToInteract == null) return;

        _isPickedUp = true;
        _objectToInteract.PickUpObject(_objectHolder, gameObject);

        LARJInteractableUse?.Invoke(InteractableUseType.PickUp, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

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

        _audioSource.volume = 0.05f;
        _sFXManager.PlaySound(_audioSource, _dropSound);


        if (_objectToInteract.CanInteractWhenPickedUp)
        {
            _objectToInteract.DisablePickedUpButtonHints();
        }
        LARJInteractableUse?.Invoke(InteractableUseType.Drop, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);

        _objectToInteract = null;
    }
    private void PressInteraction()
    {
        if (_objectToInteract == null) return;

        LARJInteractableUse?.Invoke(InteractableUseType.Press, _objectToInteract.UniqueInstanceID, InteractableObjectID.None);
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

        objectToInteract.HoldingStartedEvent();

        LARJInteractableUse?.Invoke(InteractableUseType.HoldStart, objectToInteract.UniqueInstanceID, InteractableObjectID.None);
    }
    private void PressTheCorrectKeysInteraction()
    {
        if (_objectToInteract == null) return;

        _objectToInteract.PressTheCorrectKeysStartedEvent(_playerInput.currentControlScheme);
        _canUseArrowKeys = true;
    }
    #endregion
}