using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI TimeText = null;

    public void SetTime(int hours)
    {
        TimeText.text = $"{hours.ToString("D2")}:00";
    }

    public void SetTime(int hours, int minutes)
    {
        TimeText.text = $"{hours.ToString("D2")}:{minutes.ToString("D2")}";
    }
}
