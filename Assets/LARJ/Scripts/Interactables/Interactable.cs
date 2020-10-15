using cakeslice;
using System;
using System.Collections;
using Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public enum InteractionType
{
    PickUp,
    Press,
    Hold,
    MultiPress,
    PressTheCorrectKeys
}
public enum CorrectKeysInteraction
{
    Up,
    Left,
    Down,
    Right
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
    [HideInInspector] public Transform TransformForPickUp = null;

    #region Button hints
    //Button press hints
    [HideInInspector] public GameObject KeyboardReleaseButtonHintImage = null;
    [HideInInspector] public GameObject KeyboardPressedButtonHintImage = null;
    [HideInInspector] public GameObject GamepadReleaseButtonHintImage = null;
    [HideInInspector] public GameObject GamepadPressedButtonHintImage = null;
    [HideInInspector] public GameObject HoldingHintImage = null;
    [HideInInspector] public GameObject MousePickedUpInteractionButtonHintImage = null;
    [HideInInspector] public GameObject GamepadPickedUpInteractionButtonHintImage = null;

    //arrows
    [HideInInspector] public GameObject KeyboardUpArrowHintImage = null;
    [HideInInspector] public GameObject KeyboardLeftArrowHintImage = null;
    [HideInInspector] public GameObject KeyboardDownArrowHintImage = null;
    [HideInInspector] public GameObject KeyboardRightArrowHintImage = null;
    [HideInInspector] public GameObject GamepadUpArrowHintImage = null;
    [HideInInspector] public GameObject GamepadLeftArrowHintImage = null;
    [HideInInspector] public GameObject GamepadDownArrowHintImage = null;
    [HideInInspector] public GameObject GamepadRightArrowHintImage = null;
    #endregion
    [HideInInspector] public Image Progressbar = null;
    [HideInInspector] public Image ProgressbarBackground = null;

    private Collider[] _colliders;

    [HideInInspector] public float HoldingTime = 1f;
    [HideInInspector] public int PressCountToFinishTask = 10;
    [HideInInspector] public int CorrectKeysPressedCountToFinishTask = 10;
    [HideInInspector] public bool CanInteractWhenPickedUp = false;

    private int _currentCorrectKeysPressedCount = 0; 
    private Coroutine _lastCoroutine;
    private CorrectKeysInteraction _currentCorrectKey;

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
        if (KeyboardReleaseButtonHintImage != null) KeyboardReleaseButtonHintImage.SetActive(false);
        if (KeyboardPressedButtonHintImage != null) KeyboardPressedButtonHintImage.SetActive(false);
        if (GamepadReleaseButtonHintImage != null) GamepadReleaseButtonHintImage.SetActive(false);
        if (GamepadPressedButtonHintImage != null) GamepadPressedButtonHintImage.SetActive(false);
        if (HoldingHintImage != null) HoldingHintImage.SetActive(false);

        if(_lastCoroutine != null) StopCoroutine(_lastCoroutine);		
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
            KeyboardPressedButtonHintImage.SetActive(true);
            EnableAdditionalButtonHintsForKeyboard();
        }
        else if (currentPlayerControlScheme == "Gamepad")
        {
            KeyboardPressedButtonHintImage.SetActive(true);
            EnableAdditionalButtonHintsForGamepad();
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
    private void EnableAdditionalButtonHintsForKeyboard()
    {
        switch (InteractionType)
        {
            case InteractionType.PickUp:
                break;
            case InteractionType.Press:
                break;
            case InteractionType.Hold:
                HoldingHintImage.SetActive(true);
                break;
            case InteractionType.MultiPress:
                _lastCoroutine = StartCoroutine(MultiPressButtonSwitchCoroutine(KeyboardPressedButtonHintImage, KeyboardReleaseButtonHintImage));
                break;
        }
    }
    private void EnableAdditionalButtonHintsForGamepad()
    {
        switch (InteractionType)
        {
            case InteractionType.PickUp:
                break;
            case InteractionType.Press:
                break;
            case InteractionType.Hold:
                HoldingHintImage.SetActive(true);
                break;
            case InteractionType.MultiPress:
                _lastCoroutine = StartCoroutine(MultiPressButtonSwitchCoroutine(KeyboardPressedButtonHintImage, KeyboardReleaseButtonHintImage));
                break;
        }
    }
    private IEnumerator MultiPressButtonSwitchCoroutine(GameObject pressImage, GameObject releaseImage)
    {
        while (true)
        {
            pressImage.SetActive(true);
            releaseImage.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            pressImage.SetActive(false);
            releaseImage.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void SetCurrentCorrectKeyUI(string currentPlayerControlScheme)
    {
        DeactivateArrowUI();

        if (currentPlayerControlScheme == "Keyboard")
        {
            switch (_currentCorrectKey)
            {
                case CorrectKeysInteraction.Up:
                    KeyboardUpArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Left:
                    KeyboardLeftArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Down:
                    KeyboardDownArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Right:
                    KeyboardRightArrowHintImage.SetActive(true);
                    break;
            }
        }
        else if (currentPlayerControlScheme == "Gamepad")
        {
            switch (_currentCorrectKey)
            {
                case CorrectKeysInteraction.Up:
                    GamepadUpArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Left:
                    GamepadLeftArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Down:
                    GamepadDownArrowHintImage.SetActive(true);
                    break;
                case CorrectKeysInteraction.Right:
                    GamepadRightArrowHintImage.SetActive(true);
                    break;
            }
        }
    }
    private void DeactivateArrowUI()
    {
        KeyboardUpArrowHintImage.SetActive(false);
        KeyboardLeftArrowHintImage.SetActive(false);
        KeyboardDownArrowHintImage.SetActive(false);
        KeyboardRightArrowHintImage.SetActive(false);
        GamepadUpArrowHintImage.SetActive(false);
        GamepadLeftArrowHintImage.SetActive(false);
        GamepadDownArrowHintImage.SetActive(false);
        GamepadRightArrowHintImage.SetActive(false);
    }
    public void UpdateProgressbar(float holdingTime)
    {
        if (Progressbar == null) return;

        Progressbar.gameObject.SetActive(true);
        ProgressbarBackground.gameObject.SetActive(true);

        Progressbar.fillAmount = holdingTime / HoldingTime;
    }
    public void DisableProgressbar()
    {
        Progressbar.gameObject.SetActive(false);
        ProgressbarBackground.gameObject.SetActive(false);
        Progressbar.fillAmount = 0;
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
        Rb.useGravity = false;
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
    }

    public void PickUpObject(Transform parent)
    {
        Rb.Sleep();
        DisableColliders();
        TransformForPickUp.parent = parent;
        TransformForPickUp.localPosition = Vector3.zero;
        TransformForPickUp.localRotation = Quaternion.identity;
        DisableButtonHints();
    }
    public void DropObject()
    {
        TransformForPickUp.parent = null;
        Rb.WakeUp();
        EnableColliders();
    }
    public void PressCorrectKeyInteraction(CorrectKeysInteraction pressedKey, string currentPlayerControlScheme)
    {
        if (pressedKey == _currentCorrectKey)
        {
            _currentCorrectKeysPressedCount++;

            if (_currentCorrectKeysPressedCount >= CorrectKeysPressedCountToFinishTask)
            {
                DeactivateArrowUI();
                PressTheCorrectKeysFinishedEvent();
            }
            else
            {
                SetRandomCorrectKeyInteraction(currentPlayerControlScheme);
            }
        }
        else
        {
            PressTheCorrectKeysFailedEvent();
        }
    }
    private void SetRandomCorrectKeyInteraction(string currentPlayerControlScheme)
    {
        int rnd = UnityEngine.Random.Range(0, 4);
        switch (rnd)
        {
            case 0:
                _currentCorrectKey = CorrectKeysInteraction.Up;
                break;
            case 1:
                _currentCorrectKey = CorrectKeysInteraction.Left;
                break;
            case 2:
                _currentCorrectKey = CorrectKeysInteraction.Down;
                break;
            case 3:
                _currentCorrectKey = CorrectKeysInteraction.Right;
                break;
        }
        SetCurrentCorrectKeyUI(currentPlayerControlScheme);
    }

    #region Events
    public virtual void HoldingStartedEvent() { }
    public virtual void HoldingFailedEvent() { }
    public virtual void HoldingFinishedEvent() { }
    public virtual void HoldingFinishedEvent(GameObject pickUpObject) { }
    public virtual void PressEvent() { }
    public virtual void MultiPressEvent() { }

    /// <summary>
    /// Call base to set first random correct key!
    /// </summary>
    /// <param name="currentPlayerControlScheme"></param>
    public virtual void PressTheCorrectKeysStartedEvent(string currentPlayerControlScheme)
    {
        _currentCorrectKeysPressedCount = 0;
        DisableButtonHintImages();
        SetRandomCorrectKeyInteraction(currentPlayerControlScheme);
    }
    public virtual void PressTheCorrectKeysFailedEvent() { }
    public virtual void PressTheCorrectKeysFinishedEvent() { }
    public virtual void MousePressEvent() { }
    public virtual void MouseReleaseEvent() { }
    public virtual void StartInteractible() { }
    public virtual void StopInteractible() { }
    public virtual void OnNetworkFinishedEvent() { }
    #endregion
}
