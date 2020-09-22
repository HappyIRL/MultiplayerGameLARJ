using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveOnClick : MonoBehaviour
{
    [SerializeField] ObjectiveTrigger objective = new ObjectiveTrigger();

    void OnMouseDown()
    {
        objective.Invoke();

        this.enabled = false;
    }
}
