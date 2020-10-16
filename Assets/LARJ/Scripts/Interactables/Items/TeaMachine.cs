using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeaMachine : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mug")
        {
            other.GetComponent<Mug>().FillMug();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Mug")
        {
            other.GetComponent<Mug>().StopFillingMug();
        }
    }
}
