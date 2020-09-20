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

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactables : MonoBehaviour
{
    public InteractionType InteractionType = InteractionType.PickUp;
    [HideInInspector] public Rigidbody Rb = null;

    public float HoldingTime = 1f;

    //Events
    public InteractionEvents PressInteractionEvent = null;
    public InteractionEvents HoldingInteractionEvent = null;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerInteraction pi = other.GetComponent<PlayerInteraction>();

            if (!pi.IsPickedUp)
            {
                pi.CanInteract = false;
            }
        }
    }   
}
