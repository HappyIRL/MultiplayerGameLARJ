using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        private Interactable _interactable;
        private Coroutine _cooldownCoroutine;
        public TaskType GetTaskType { get => _taskType; } //remove setter later

        public TaskUI TaskUI { get; set; }
        public int TaskID { get; set; }

        public int GetRewardMoney { get => _rewardMoney; }
        public int GetLostMoneyOnFail { get => _lostMoneyOnFail; }
        public float GetTimeToFinishTask { get => _timeToFinishTask; }
        public bool IsTaskActive { get => _isTaskActive; set => _isTaskActive = value; }
        public Interactable GetInteractable { get => _interactable; }
        
        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }
        public void StartTask()
        {
            _interactable.StartInteractible();
            _cooldownCoroutine = StartCoroutine(StartTaskCooldown());
        }
        IEnumerator StartTaskCooldown()
        {
            int timer = 0;
            while (timer < _timeToFinishTask)
            {
                timer++;
                yield return new WaitForSeconds(1);
            }
            TaskManager.TaskManagerSingelton.OnTaskFailed(this);
            yield return null;
        }

        private void StopTaskCoolDown()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }
        }
        public void StopTask()
        {
            StopTaskCoolDown();
            _interactable.StopInteractible();

        }
    }
}
