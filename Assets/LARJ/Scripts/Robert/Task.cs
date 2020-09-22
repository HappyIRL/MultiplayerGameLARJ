using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public int CountDown { get; set; }
    public bool IsTaskFinished { get; set; }

    public Task FollowUpTask { get; set; }

}
