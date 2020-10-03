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
        Cleaning,
        ExtinguishFire,
        Mail,
        NotAssigned
    }
    public class Task : MonoBehaviour
    {
        [SerializeField] private int _rewardMoney;
        [SerializeField] private int _lostMoneyOnFail;
        [SerializeField] private float _timeToFinishTask;
        [SerializeField] private TaskType _taskType;
        private bool _isTaskActive = false;
        public TaskType GetTaskType { get => _taskType; } //remove setter later

        public TaskUI TaskUI { get; set; }
        public int TaskID { get; set; }

        public int GetRewardMoney { get => _rewardMoney; }
        public int GetLostMoneyOnFail { get => _lostMoneyOnFail; }
        public float GetTimeToFinishTask { get => _timeToFinishTask; }
        public bool IsTaskActive { get => _isTaskActive;  set => _isTaskActive = value; }

        public void StartTask()
        {

        }
    }
}
