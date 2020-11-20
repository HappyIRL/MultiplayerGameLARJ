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
        Money,
        Mail,
        Paper,
        Document,
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
        private float _timer = 0;

        public TaskType GetTaskType { get => _taskType; set => _taskType = value; }

        public TaskUI TaskUI { get; set; }

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
            _timer = 0;
            _interactable.StartInteractible();
            StartTaskCooldown();
        }

        public void StartTaskCooldown()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }
            _cooldownCoroutine = StartCoroutine(StartTaskCooldownCoroutine());
        }

        IEnumerator StartTaskCooldownCoroutine()
        {         
            while (_timer < _timeToFinishTask)
            {
                _timer += Time.deltaTime;
                yield return null;
            }
            TaskManager.TaskManagerSingelton.OnTaskFailed(this);
            yield return null;
        }

        public void StopTaskCoolDown()
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
