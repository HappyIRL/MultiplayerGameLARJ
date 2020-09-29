using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{
    public enum TaskType
    {
        PhoneCall,
        Printer,
        Customer,
        NotAssigned
    }
    public class Task : MonoBehaviour
    {
        public TaskType GetTaskType { get; set; } //remove setter later

        public int CountDown { get; set; }
        public bool IsTaskFinished { get; set; }

        public Task FollowUpTask { get; set; }

    }
}
