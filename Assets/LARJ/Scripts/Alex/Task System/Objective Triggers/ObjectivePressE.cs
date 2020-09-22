using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePressE : MonoBehaviour
{
    [SerializeField] ObjectiveTrigger objective = new ObjectiveTrigger();

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            objective.Invoke();

            this.enabled = false;
        }
    }
}
