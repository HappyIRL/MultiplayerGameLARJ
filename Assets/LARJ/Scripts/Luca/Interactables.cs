using cakeslice;
using System;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    PickUp,
    Press,
    Hold,
}

[Serializable]
public class InteractionEvents : UnityEvent { }

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(Outline))]
public class Interactables : MonoBehaviour
{
    public InteractionType InteractionType = InteractionType.PickUp;
    [HideInInspector] public Rigidbody Rb = null;
    [HideInInspector] public Outline OutlineRef;

    public float HoldingTime = 1f;

    //Events
    public InteractionEvents PressInteractionEvent = null;
    public InteractionEvents HoldingFinishedInteractionEvent = null;
    public InteractionEvents HoldingStartedInteractionEvent = null;
    public InteractionEvents HoldingFailedInteractionEvent = null;


    private bool _referenceWasSetInOnTriggerStay = false;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        OutlineRef = GetComponent<Outline>();
        OutlineRef.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction.ObjectToInteract = this;
            playerInteraction.InteractableInteractionType = InteractionType;
            playerInteraction.CanInteract = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!_referenceWasSetInOnTriggerStay)
            {
                PlayerInteraction pi = other.GetComponent<PlayerInteraction>();

                if (pi.ObjectToInteract == null)
                {
                    pi.ObjectToInteract = this;
                    pi.InteractableInteractionType = InteractionType;
                    pi.CanInteract = true;
                    _referenceWasSetInOnTriggerStay = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerInteraction pi = other.GetComponent<PlayerInteraction>();

            OutlineRef.enabled = false;
            _referenceWasSetInOnTriggerStay = false;

            if (pi.ObjectToInteract == this)
            {
                pi.ObjectToInteract = null;
            }
            if (!pi.IsPickedUp)
            {
                pi.CanInteract = false;
            }
        }
    }   
}
