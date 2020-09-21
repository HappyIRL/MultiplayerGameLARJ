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
    private float _holdingTimer = 0;

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
                ObjectToInteract.HoldingInteractionEvent.Invoke();
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
        Debug.Log("OnRelease");
        _holdingObject = false;
        _holdingTimer = 0f;
        _holdingTimeBarBG.SetActive(false);
    }

    private void PickUp()
    {
        if (ObjectToInteract == null) return;

        Debug.Log("PickUp");

        IsPickedUp = true;
        ObjectToInteract.Rb.Sleep();
        ObjectToInteract.transform.parent = transform;
        ObjectToInteract.transform.forward = transform.forward;
        ObjectToInteract.transform.position = _objectHolder.position;
    }
    private void Drop()
    {
        if (ObjectToInteract == null) return;

        Debug.Log("Drop");
        IsPickedUp = false;
        ObjectToInteract.Rb.WakeUp();
        ObjectToInteract.transform.parent = null;
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
        _holdingTimeBar.fillAmount = 0;
        _holdingTimeBarBG.SetActive(true);
    }
}
