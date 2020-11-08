using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBox : MonoBehaviour
{
     [SerializeField] private Outline _outline;
    private void Start()
    {
        _outline.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _outline.enabled = true;            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(_outline.enabled)
        _outline.enabled = false;
    }
}
