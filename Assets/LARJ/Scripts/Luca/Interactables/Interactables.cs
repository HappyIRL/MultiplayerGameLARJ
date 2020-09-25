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
    public bool CanInteractWhenPickedUp = false;

    //Button press hints
    public GameObject KeyboardButtonHintImage = null;
    public GameObject GamepadButtonHintImage = null;
    public GameObject MousePickedUpInteractionButtonHintImage = null;
    public GameObject GamepadPickedUpInteractionButtonHintImage = null;

    //Events
    public InteractionEvents HoldingFinishedInteractionEvent = null;
    public InteractionEvents HoldingStartedInteractionEvent = null;
    public InteractionEvents HoldingFailedInteractionEvent = null;
    public InteractionEvents PressInteractionEvent = null;
    public InteractionEvents MousePressInteractionEvent = null;
    public InteractionEvents MouseReleaseInteractionEvent = null;



    private bool _referenceWasSetInOnTriggerStay = false;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        OutlineRef = GetComponent<Outline>();
    }
    private void Start()
    {
        OutlineRef.enabled = false;
        DisableButtonHintImages();

        if (CanInteractWhenPickedUp)
        {
            DisablePickedUpInteractionButtonHints();
        }
    }

    private void OnDisable()
    {
        DisableButtonHintImages();

        if (CanInteractWhenPickedUp)
        {
            DisablePickedUpInteractionButtonHints();
        }
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

            if (!pi.IsPickedUp)
            {
                OutlineRef.enabled = false;
                DisableButtonHintImages();
                _referenceWasSetInOnTriggerStay = false;

                pi.CanInteract = false;
                if (pi.ObjectToInteract == this)
                {
                    pi.ObjectToInteract = null;
                }
            }
        }
    }   


    public void DisableButtonHintImages()
    {
        KeyboardButtonHintImage.SetActive(false);
        GamepadButtonHintImage.SetActive(false);
    }
    public void DisablePickedUpInteractionButtonHints()
    {
        MousePickedUpInteractionButtonHintImage.SetActive(false);
        GamepadPickedUpInteractionButtonHintImage.SetActive(false);
    }
    public void EnableButtonHintImage(string currentPlayerControlScheme)
    {
        if (currentPlayerControlScheme == "Keyboard")
        {
            KeyboardButtonHintImage.SetActive(true);
        }
        else if (currentPlayerControlScheme == "Gamepad")
        {
            GamepadButtonHintImage.SetActive(true);
        }
    }
    public void EnablePickedUpInteractionHintImage(string currentPlayerControlScheme)
    {
        if (currentPlayerControlScheme == "Keyboard")
        {
            MousePickedUpInteractionButtonHintImage.SetActive(true);
        }
        else if (currentPlayerControlScheme == "Gamepad")
        {
            GamepadPickedUpInteractionButtonHintImage.SetActive(true);
        }        
    }
}
