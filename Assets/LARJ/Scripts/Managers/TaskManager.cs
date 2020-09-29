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
        [SerializeField] private int _openTasks;
        [SerializeField] private Task[] _possibleTasks;
        [SerializeField] private float _delayBetweenTasks;
        [SerializeField] private TextMeshProUGUI[] _tasksListText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        float _timer = 0f;
        private byte[] _taskListCounter = new byte[(int)TaskType.NotAssigned];

        private void Awake()
        {
            for (int i = 0; i < _taskListCounter.Length; i++)
            {
                _taskListCounter[i] = 0;
            }
        }
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _delayBetweenTasks)
            {
                _timer = 0f;
                StartRandomTask();
            }
        }

        private void StartRandomTask()
        {
            int i = UnityEngine.Random.Range(0, _possibleTasks.Length);
            Task task = _possibleTasks[i];
            task.enabled = true;
            UpdateTasksText((int)task.GetTaskType,true);
        }


        /// <summary>
        /// Updates the TasksTExt in  the UI, taskType decides which Taskstext should be modified and positive means eigther increasing the Taskcounter or decreasing it.
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="positive"></param>
        private void UpdateTasksText(int taskType, bool positive)
        {
            if (positive)
            {
                _taskListCounter[taskType]++;
                switch (taskType)
                {
                    case 0:
                        _tasksListText[taskType].text = _taskListCounter[taskType] + " Answer Phone Call!";
                        break;
                    case 1:
                        _tasksListText[taskType].text = _taskListCounter[taskType] + " Print Documents!!";
                        break;
                    case 2:
                        _tasksListText[taskType].text = _taskListCounter[taskType] + " Talk to Customer!!!";
                        break;
                    //Add more if we have more Tasks
                    default:
                        Debug.LogError("Task is missing TaskType");
                        break;
                }
            }
            else
            {
                _taskListCounter[taskType]--;
                if (_taskListCounter[taskType] > 0)
                {
                    switch (taskType)
                    {
                        case 0:
                            _tasksListText[taskType].text = _taskListCounter[taskType] + " Answer Phone Call!";
                            break;
                        case 1:
                            _tasksListText[taskType].text = _taskListCounter[taskType] + " Print Documents!!";
                            break;
                        case 2:
                            _tasksListText[taskType].text = _taskListCounter[taskType] + " Talk to Customer!!!";
                            break;
                        //Add more if we have more Tasks
                        default:
                            break;
                    }
                }
                else
                {
                    if (_taskListCounter[taskType] < 0)
                    {
                        Debug.LogError("Try to delete a Task, that was never given.");
                        _taskListCounter[taskType] = 0;
                    }
                    _tasksListText[taskType].text = "";
                }
            }

        }

        public void OnTaskCompleted(TaskType taskType)
        {
            _completedTasks++;
            UpdateTasksText((int)taskType, false);
            UpdateScore();
        }

        private void UpdateScore()
        {
            _scoreText.text = (_completedTasks * 20 - _failedTasks * 10).ToString();
        }

        public void OnTaskFailed(TaskType taskType)
        {
            _failedTasks++;
            UpdateTasksText((int)taskType, false);
            UpdateScore();
        }
    }
}
