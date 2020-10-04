using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Paper : Interactable
{
    [Tooltip("Broom = 64,Telephone1 = 65,Telephone2 = 66,FireExtinguisher = 67,Paper = 68,PC = 69,Printer = 70,Shotgun = 71,WaterCooler = 72")]
    [SerializeField] private int _interactableID;

	private void Start()
	{
        interactableID = (InteractableObjectID)_interactableID;
    }
	

    public override void StartInteractible()
    {
    }

    public override void StopInteractible()
    {
    }
}
