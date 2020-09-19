using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform objectHolder = null;

    //Object to interact
    [HideInInspector] public Interactables ObjectToInteract = null;
    [HideInInspector] public InteractionType InteractableInteractionType = InteractionType.PickUp;
    [HideInInspector] public bool CanInteract = false;

    private bool isPickedUp = false;
    private bool holdingObject = false;
    private float holdingTimer = 0;

    private void Update()
    {
        if (holdingObject)
        {
            holdingTimer += Time.deltaTime;

            if (holdingTimer >= ObjectToInteract.HoldingTime)
            {
                holdingTimer = 0;
                holdingObject = false;
                ObjectToInteract.HoldingInteractionEvent.Invoke();
            }
        }



        //Keyboard kb = InputSystem.GetDevice<Keyboard>();
        //if (kb.wKey.wasPressedThisFrame)
        //{
        //    transform.Translate(Vector3.forward, Space.World);
        //}
        //if (kb.sKey.wasPressedThisFrame)
        //{
        //    transform.Translate(Vector3.back, Space.World);
        //}
        //if (kb.aKey.wasPressedThisFrame)
        //{
        //    transform.Translate(Vector3.left, Space.World);
        //}
        //if (kb.dKey.wasPressedThisFrame)
        //{
        //    transform.Translate(Vector3.right, Space.World);
        //}
    }


    public void OnPress()
    {
        Debug.Log("OnPress");

        if (CanInteract)
        {
            if (ObjectToInteract == null) return;

            if (InteractableInteractionType == InteractionType.PickUp)
            {               
                if (isPickedUp)
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
        holdingObject = false;
    }

    private void PickUp()
    {
        if (ObjectToInteract == null) return;

        Debug.Log("PickUp");

        isPickedUp = true;
        ObjectToInteract.Rb.Sleep();
        ObjectToInteract.transform.parent = transform;
        ObjectToInteract.transform.forward = transform.forward;
        ObjectToInteract.transform.position = objectHolder.position;
    }
    private void Drop()
    {
        if (ObjectToInteract == null) return;

        Debug.Log("Drop");
        isPickedUp = false;
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

        holdingObject = true;
    }
}
