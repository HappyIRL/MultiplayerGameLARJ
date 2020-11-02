using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;

[Serializable]
public class Paper : Interactable
{
    [SerializeField] private InteractableObjectID _interactableID;

    public override void Awake()
    {
        base.Awake();
        InteractableID = _interactableID;
        AlwaysInteractable = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag =="PaperBox")
        {
            Debug.Log("In Paper Box");
            TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
            Destroy(gameObject);
        }
    }

}
