using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : Interactable
{
    [Header("Stamp")]
    [SerializeField] private GameObject _stampPrintPrefab = null;
    public Animator Animator = null;

    [HideInInspector] public bool CanPrint = false;
    [HideInInspector] public bool StampingFinished = true;
    [HideInInspector] public Collider Collider;

    [SerializeField] private InteractableObjectID _interactableID;

    public override void Awake()
    {
        base.Awake();
        Collider = GetComponent<Collider>();
        InteractableID = _interactableID;
    }

    public override void MousePressEvent()
    {
        if (StampingFinished)
        {
            Animator.enabled = true;
            Rb.Sleep();
            Animator.Play("Wait");
            Collider.enabled = true;
            Animator.SetBool("StampingFinished", false);
            Animator.SetBool("StartStamping", true);
            CanPrint = true;
            StampingFinished = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CanPrint)
        {
            Debug.Log("Print");
            SpawnStampPrint(collision);
            CanPrint = false;
        }
    }

    private void SpawnStampPrint(Collision collision)
    {
        GameObject obj = InstantiateManager.Instance.Instantiate(_stampPrintPrefab);
        obj.transform.position = collision.GetContact(0).point;
        obj.transform.forward = collision.GetContact(0).normal;
        obj.transform.parent = collision.transform;
    }

    public void FinishStamping()
    {
        Animator.SetBool("StartStamping", false);
        Animator.SetBool("StampingFinished", true);
        CanPrint = false;
        StampingFinished = true;
        Animator.enabled = false;
    }
}
