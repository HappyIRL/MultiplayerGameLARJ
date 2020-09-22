using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] ObjectiveTrigger objective = new ObjectiveTrigger();

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            objective.Invoke();

            this.enabled = false;

        }
        
    }
}
