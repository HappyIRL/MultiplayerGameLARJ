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
        [SerializeField] private int _openTasks = 0;
        [SerializeField] private Task[] _startingTasks;
        [SerializeField] private Task[] _followUpTasks;
        [SerializeField] private float _delayBetweenTasks;
        [SerializeField] private float _variationOfTaskDelay;
        [SerializeField] private TaskManagerUI _taskManagerUI;
        public TaskManagerUI TaskManagerUI { get => _taskManagerUI; }
        [SerializeField] private Score _score;
        public Score Score { get => _score; }
        public delegate void LARJTaskEvent(Interactable interactable, LARJTaskState state);
        public event LARJTaskEvent OnTask;
        //[SerializeField] private TextMeshProUGUI[] _tasksListText;
        public static TaskManager TaskManagerSingelton;

        private bool _isLocal;
        private float _timer = 0f;
        private float _currentDelay;

        private int _failedTasks = 0;
        private int _completedTasks = 0;
        public int FailedTaks { get => _failedTasks; }
        public int CompletedTasks { get => _completedTasks; }

        private void Awake()
        {
            if (PhotonNetwork.IsConnected)
                _isLocal = false;
            else
                _isLocal = true;
            _currentDelay = _delayBetweenTasks;
            TaskManagerSingelton = this;
        }
        private void Start()
        {
            //if done in start, will be faster than network can handle
            //StartRandomTask();
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
            _currentDelay = UnityEngine.Random.Range(_delayBetweenTasks - _variationOfTaskDelay, _delayBetweenTasks + _variationOfTaskDelay);
        }

        private void StartRandomStartingTask()
        {
            if (PhotonNetwork.IsMasterClient || _isLocal)
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

        ///// <summary>
        ///// Updates the TasksTExt in  the UI, taskType decides which Taskstext should be modified and positive means eigther increasing the Taskcounter or decreasing it.
        ///// </summary>
        ///// <param name="taskType"></param>
        ///// <param name="positive"></param>
        //private void UpdateTasksText(int taskType, bool positive)
        //{
        //    if (positive)
        //    {
        //        _taskListCounter[taskType]++;
        //        switch (taskType)
        //        {
        //            case 0:
        //                _tasksListText[taskType].text = _taskListCounter[taskType] + " Answer Phone Call!";
        //                break;
        //            case 1:
        //                _tasksListText[taskType].text = _taskListCounter[taskType] + " Print Documents!!";
        //                break;
        //            case 2:
        //                _tasksListText[taskType].text = _taskListCounter[taskType] + " Talk to Customer!!!";
        //                break;
        //            //Add more if we have more Tasks
        //            default:
        //                Debug.LogError("Task is missing TaskType");
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        _taskListCounter[taskType]--;
        //        if (_taskListCounter[taskType] > 0)
        //        {
        //            switch (taskType)
        //            {
        //                case 0:
        //                    _tasksListText[taskType].text = _taskListCounter[taskType] + " Answer Phone Call!";
        //                    break;
        //                case 1:
        //                    _tasksListText[taskType].text = _taskListCounter[taskType] + " Print Documents!!";
        //                    break;
        //                case 2:
        //                    _tasksListText[taskType].text = _taskListCounter[taskType] + " Talk to Customer!!!";
        //                    break;
        //                //Add more if we have more Tasks
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            if (_taskListCounter[taskType] < 0)
        //            {
        //                Debug.LogError("Try to delete a Task, that was never given.");
        //                _taskListCounter[taskType] = 0;
        //            }
        //            _tasksListText[taskType].text = "";
        //        }
        //    }

        //}

        public void OnTaskCompleted(Task task)
        {
            task.IsTaskActive = false;
            //UpdateTasksText((int)taskType, false);
            task.StopTask();
            TaskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetRewardMoney, true);
            OnTask.Invoke(task.GetInteractable, LARJTaskState.TaskComplete);

            _completedTasks++;
        }


        public void OnTaskFailed(Task task)
        {
            task.IsTaskActive = false;
            //UpdateTasksText((int)taskType, false);
            task.StopTask();
            TaskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetLostMoneyOnFail, false);
            if (_isLocal || PhotonNetwork.IsMasterClient)
                OnTask.Invoke(task.GetInteractable, LARJTaskState.TaskFailed);

            _failedTasks++;
        }
        public void StartTask(Task task)
        {
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            if (_isLocal || PhotonNetwork.IsMasterClient)
                OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
        public void StartRandomFollowUpTask()
        {
            if (PhotonNetwork.IsMasterClient || _isLocal)
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
        }
        public void StartMoneyTask(Task task)
        {
            task.IsTaskActive = true;
            TaskUI taskUI = TaskManagerUI.SpawnUITask(TaskType.Money, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            if (_isLocal || PhotonNetwork.IsMasterClient)
                OnTask?.Invoke(task.GetInteractable, LARJTaskState.TaskStart);
        }
    }
}
