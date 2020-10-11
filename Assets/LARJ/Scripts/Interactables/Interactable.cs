using cakeslice;
using System;
using Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    PickUp,
    Press,
    Hold,
}
public enum InteractableObjectID
{
    Broom = 64,
    Telephone1 = 65,
    Telephone2 = 66,
    FireExtinguisher = 67,
    Paper = 68,
    PC = 69,
    Printer = 70,
    Shotgun = 71,
    WaterCooler = 72,
    Customer = 73,
    None = 100
}

[Serializable]
public class InteractionEvents : UnityEvent { }

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(Outline))]
[Serializable]
public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public InteractionType InteractionType;
    [HideInInspector] public Rigidbody Rb = null;
    [HideInInspector] public Outline OutlineRef = null;

    //Button press hints
    [HideInInspector] public GameObject KeyboardButtonHintImage = null;
    [HideInInspector] public GameObject GamepadButtonHintImage = null;
    [HideInInspector] public GameObject MousePickedUpInteractionButtonHintImage = null;
    [HideInInspector] public GameObject GamepadPickedUpInteractionButtonHintImage = null;

    private Collider[] _colliders;

    [HideInInspector] public float HoldingTime = 1f;
    [HideInInspector] public bool CanInteractWhenPickedUp = false;
    public InteractableObjectID InteractableID { get; protected set; }

    public int ObjectInstanceID { get; set; }

    public virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        OutlineRef = GetComponent<Outline>();
        _colliders = GetComponents<Collider>();
    }
    public virtual void Start()
    {
        DisableButtonHintImages();

        if (CanInteractWhenPickedUp)
        {
            DisablePickedUpInteractionButtonHints();
            Rb.Sleep();
        }
    }


    public virtual void OnDisable()
    {
        DisableButtonHintImages();

        if (CanInteractWhenPickedUp)
        {
            DisablePickedUpInteractionButtonHints();
        }
    }

    #region UI
    public void DisableButtonHints()
    {
        DisableButtonHintImages();
    }
    public void DisablePickedUpButtonHints()
    {
        DisablePickedUpInteractionButtonHints();
    }
    public void EnableButtonHints(string currentPlayerControlScheme)
    {
        EnableButtonHintImage(currentPlayerControlScheme);
    }
    public void EnablePickedUpButtonHints(string currentPlayerControlScheme)
    {
        EnablePickedUpInteractionHintImage(currentPlayerControlScheme);
    }

    private void DisableButtonHintImages()
    {
        if(KeyboardButtonHintImage != null || GamepadButtonHintImage != null)
		{
            KeyboardButtonHintImage.SetActive(false);
            GamepadButtonHintImage.SetActive(false);
		}
    }
    private void DisablePickedUpInteractionButtonHints()
    {
        MousePickedUpInteractionButtonHintImage.SetActive(false);
        GamepadPickedUpInteractionButtonHintImage.SetActive(false);
    }
    private void EnableButtonHintImage(string currentPlayerControlScheme)
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
    private void EnablePickedUpInteractionHintImage(string currentPlayerControlScheme)
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
    #endregion

    public void EnableColliders()
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = true;
            Rb.useGravity = true;
        }
    }
    public void DisableColliders()
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
            Rb.useGravity = false;
        }
    }

    public void PickUpObject(Transform parent)
    {
        Rb.Sleep();
        transform.rotation = parent.rotation;
        transform.position = parent.position;
        transform.parent = parent;
        DisableButtonHints();
        DisableColliders();
    }
    public void DropObject()
    {
        transform.parent = null;
        Rb.WakeUp();
        EnableColliders();
    }

    #region Events
    public virtual void HoldingStartedEvent() { }
    public virtual void HoldingFailedEvent() { }
    public virtual void HoldingFinishedEvent() { }
    public virtual void HoldingFinishedEvent(GameObject pickUpObject) { }
    public virtual void PressEvent() { }
    public virtual void MousePressEvent() { }
    public virtual void MouseReleaseEvent() { }
    public virtual void StartInteractible() { }
    public virtual void StopInteractible() { }
    public virtual void OnNetworkFinishedEvent() { }
    #endregion
}
