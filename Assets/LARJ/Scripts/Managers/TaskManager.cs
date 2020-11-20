using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tasks
{
    public enum LARJTaskState
    {
        TaskStart = 5,
        TaskFailed = 6,
        TaskComplete = 7
    }

    public class TaskManager : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField] private AudioClip _cashSound = null;
        [SerializeField] private AudioClip _taskFailedSound = null;
        private SFXManager _sFXManager;

        [SerializeField] private int _openTasks = 0;
        [SerializeField] private Task[] _startingTasks;
        [SerializeField] private Task[] _followUpTasks;
        private float _delayBetweenTasks;
        [SerializeField] private float _variationOfTaskDelay;
        [SerializeField] private TaskManagerUI _taskManagerUI;
        public TaskManagerUI TaskManagerUI { get => _taskManagerUI; }
        [SerializeField] private Score _score;
        public Score Score { get => _score; }
        public delegate void LARJTaskEvent(Interactable interactable, LARJTaskState state);
        public event LARJTaskEvent OnTask;
        //[SerializeField] private TextMeshProUGUI[] _tasksListText;
        public static TaskManager TaskManagerSingelton;
        private float _timer = 0f;
        private float _currentDelay;

        private int _failedTasks = 0;
        private int _completedTasks = 0;
        public int FailedTaks { get => _failedTasks; }
        public int CompletedTasks { get => _completedTasks; }

        [SerializeField, Range(0, 10)] private int _difficulty = 1;
        public int Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                UpdateDalayBetweenTasks();
            }
        }

        private void Awake()
        {
            _audioSource.volume = 0.01f;
            _currentDelay = _delayBetweenTasks;
            TaskManagerSingelton = this;
        }
        private void Start()
        {
            //_sFXManager = SFXManager.Instance;
        }
        void Update()
        {
           
            _timer += Time.deltaTime;
            if (_timer >= _currentDelay)
            {
                UpdateDalayBetweenTasks();
                _timer = 0f;
                StartRandomStartingTask();
            }
        }

        private void UpdateDalayBetweenTasks()
        {
            if (_difficulty == 0) _delayBetweenTasks = 1000000;
            else _delayBetweenTasks = ((float)1/(float)_difficulty) * 30f;
            _currentDelay = UnityEngine.Random.Range(_delayBetweenTasks - _variationOfTaskDelay, _delayBetweenTasks + _variationOfTaskDelay);
        }

        private void StartRandomStartingTask()
        {
            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                Task task;
                if (!CheckIfTasksAreAvailable(_startingTasks))
                {
                    return;
                }
                do
                {
                    int i = UnityEngine.Random.Range(0, _startingTasks.Length);
                    task = _startingTasks[i];

                } while (task.IsTaskActive);
                task.IsTaskActive = true;
                TaskUI taskUI = TaskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
                task.TaskUI = taskUI;
                task.StartTask();
                OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
            }
        }

        private bool CheckIfTasksAreAvailable(Task[] Tasks)
        {
            for (int i = 0; i < Tasks.Length; i++)
            {
                if (!Tasks[i].IsTaskActive)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnTaskCompleted(Task task)
        {
            if (!task.IsTaskActive)
			{
                Debug.Log("Task not active: " + task);
                return;
			}

            task.IsTaskActive = false;
            task.StopTask();
            TaskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetRewardMoney, true);
            OnTask.Invoke(task.GetInteractable, LARJTaskState.TaskComplete);
            SFXManager.Instance.PlaySound(_audioSource, _cashSound);
            //_sFXManager.PlaySound(_audioSource, _cashSound); what is better?
            _completedTasks++;
        }


        public void OnTaskFailed(Task task)
        {
            if (!task.IsTaskActive) return;

            task.IsTaskActive = false;
            task.StopTask();
            TaskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetLostMoneyOnFail, false);
            OnTask.Invoke(task.GetInteractable, LARJTaskState.TaskFailed);
            SFXManager.Instance.PlaySound(_audioSource, _taskFailedSound);
            _failedTasks++;
        }
        public void StartTask(Task task)
        {
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
        public void StartRandomFollowUpTask()
        {
            Task task;
            if (!CheckIfTasksAreAvailable(_followUpTasks))
            {
                return;
            }
            do
            {
                int i = UnityEngine.Random.Range(0, _followUpTasks.Length);
                task = _followUpTasks[i];

            } while (task.IsTaskActive);
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
        public void StartMoneyTask(Task task)
        {
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(TaskType.Money, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
        public void StartDocumentTask(Task task)
        {
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(TaskType.Document, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
        public void OnTaskCancelled(Task task)
        {
            task.IsTaskActive = false;
            task.StopTask();
            TaskManagerUI.RemoveUITask(task.TaskUI);
            OnTask.Invoke(task.GetInteractable, LARJTaskState.TaskComplete);
        }
    }
}
