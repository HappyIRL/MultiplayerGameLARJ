using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Interactable
{
	[SerializeField] private InteractableObjectID _interactableID;
	public override void Awake()
	{
		base.Awake();
		InteractableID = _interactableID;
	}
}
