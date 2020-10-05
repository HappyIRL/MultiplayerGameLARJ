using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tasks
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private int _completedTasks = 0;
        [SerializeField] private int _failedTasks;
        [SerializeField] private int _openTasks = 0;
        [SerializeField] private Task[] _possibleTasks;
        [SerializeField] private float _delayBetweenTasks;
        [SerializeField] private float _variationOfTaskDelay;
        [SerializeField] private TaskManagerUI _taskManagerUI;
        [SerializeField] private Score _score;
        public delegate void LARJTaskEvent(Interactable interactable, bool active);
        public event LARJTaskEvent OnTask;
        //[SerializeField] private TextMeshProUGUI[] _tasksListText;
        public static TaskManager TaskManagerSingelton;

        private int _taskIDCounter = 0;
        private float _timer = 0f;
        private float _currentDelay;

        private void Awake()
        {
            _currentDelay = _delayBetweenTasks;
            TaskManagerSingelton = this;
        }
        private void Start()
        {
            StartRandomTask();
        }
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _currentDelay)
            {
                UpdateDalayBetweenTasks();
                _timer = 0f;
                StartRandomTask();
            }
        }

        private void UpdateDalayBetweenTasks()
        {
            _currentDelay = UnityEngine.Random.Range(_delayBetweenTasks - _variationOfTaskDelay, _delayBetweenTasks + _variationOfTaskDelay);
        }

        private void StartRandomTask()
        {
            Task task;
            if (!CheckIfTasksAreAvailable())
            {
                return;
            }
            do
            {
                int i = UnityEngine.Random.Range(0, _possibleTasks.Length);
                task = _possibleTasks[i];

            } while (task.IsTaskActive);
            task.IsTaskActive = true;
            task.TaskID = _taskIDCounter;
            _taskIDCounter++;
            TaskUI taskUI = _taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask?.Invoke(task.GetInteractable, true);

        }

        private bool CheckIfTasksAreAvailable()
        {
            for (int i = 0; i < _possibleTasks.Length; i++)
            {
                if (!_possibleTasks[i].IsTaskActive)
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
            _completedTasks++;
            task.IsTaskActive = false;
            //UpdateTasksText((int)taskType, false);
            task.StopTask();
            _taskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetRewardMoney, true);
            OnTask.Invoke(task.GetInteractable, false);
        }


        public void OnTaskFailed(Task task)
        {
            _failedTasks++;
            task.IsTaskActive = false;
            //UpdateTasksText((int)taskType, false);
            task.StopTask();
            _taskManagerUI.RemoveUITask(task.TaskUI);
            _score.UpdateScore(task.GetLostMoneyOnFail, false);
            OnTask.Invoke(task.GetInteractable, false);
        }
        public void StartTask(Task task)
        {
            task.IsTaskActive = true;
            task.TaskID = _taskIDCounter;
            _taskIDCounter++;
            TaskUI taskUI = _taskManagerUI.SpawnUITask(task.GetTaskType, task.GetRewardMoney, task.GetTimeToFinishTask);
            task.TaskUI = taskUI;
            task.StartTask();
            OnTask.Invoke(task.GetInteractable, true);
        }
        public void StartPaperTask()
        {

        }
    }
}
