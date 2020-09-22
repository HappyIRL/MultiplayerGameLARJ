using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _objectHolder = null;
    [SerializeField] private Image _holdingTimeBar = null;
    [SerializeField] private GameObject _holdingTimeBarBG = null;

    //Object to interact
    [HideInInspector] public Interactables ObjectToInteract = null;
    [HideInInspector] public InteractionType InteractableInteractionType = InteractionType.PickUp;
    [HideInInspector] public bool CanInteract = false;
    [HideInInspector] public bool IsPickedUp = false;

    private bool _holdingObject = false;
    private bool _holdingWasFinished = false;
    private float _holdingTimer = 0;

    private void Start()
    {
        _holdingTimeBarBG.SetActive(false);
    }
    private void Update()
    {
        if (_holdingObject)
        {
            _holdingTimer += Time.deltaTime;
            _holdingTimeBar.fillAmount = _holdingTimer / ObjectToInteract.HoldingTime;

            if (_holdingTimer >= ObjectToInteract.HoldingTime)
            {
                _holdingTimer = 0f;
                _holdingTimeBarBG.SetActive(false);
                _holdingObject = false;
                _holdingWasFinished = true;
                ObjectToInteract.HoldingFinishedInteractionEvent.Invoke();
                _holdingTimeBar.fillAmount = 0;
            }
        }
    }


    public void OnPress()
    {
        Debug.Log("OnPress");

        if (CanInteract)
        {
            if (ObjectToInteract == null) return;

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
        Debug.Log("OnHold");

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
        if (_holdingObject)
        {
            if (!_holdingWasFinished)
            {
                if (ObjectToInteract == null) return;

                ObjectToInteract.HoldingFailedInteractionEvent.Invoke();
            }
        }

        _holdingObject = false;
        _holdingTimer = 0f;
        _holdingTimeBarBG.SetActive(false);
    }

    private void PickUp()
    {
        if (ObjectToInteract == null) return;


        IsPickedUp = true;
        ObjectToInteract.Rb.Sleep();
        ObjectToInteract.transform.parent = _objectHolder;
        ObjectToInteract.transform.forward = transform.forward;
        ObjectToInteract.transform.position = _objectHolder.position;
    }
    private void Drop()
    {
        if (ObjectToInteract == null) return;

        IsPickedUp = false;
        ObjectToInteract.transform.parent = null;
        ObjectToInteract.Rb.WakeUp();
    }
    private void PressInteraction()
    {
        if (ObjectToInteract == null) return;
        ObjectToInteract.PressInteractionEvent.Invoke();
    }
    private void HoldingInteraction()
    {
        if (ObjectToInteract == null) return;

        _holdingObject = true;
        _holdingWasFinished = false;
        _holdingTimeBar.fillAmount = 0;
        _holdingTimeBarBG.SetActive(true);
        ObjectToInteract.HoldingStartedInteractionEvent.Invoke();
    }
}
