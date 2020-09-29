using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerObjectiveOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ObjectiveTrigger objective = new ObjectiveTrigger();

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("I've been interacted with");
        objective.Invoke();

        this.enabled = false;
    }
}
