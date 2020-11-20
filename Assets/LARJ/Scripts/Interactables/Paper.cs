using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;

[Serializable]
public class Paper : Interactable
{
    public override void Awake()
    {
        base.Awake();
        AlwaysInteractable = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PaperBox")
        {
            TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>(), true);
            gameObject.SetActive(false);
        }
    }
	public override InteractableObjectID InteractableID { get => InteractableObjectID.Paper; protected set => base.InteractableID = value; }

}
