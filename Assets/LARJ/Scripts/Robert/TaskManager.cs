using System;
using System.Collections;
using System.Collections.Generic;
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
        float _timer = 0f;

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _delayBetweenTasks)
            {
                _timer = 0f;
                EnableTask();
            }
        }

        private void EnableTask()
        {
            int i = UnityEngine.Random.Range(0, _possibleTasks.Length);
            _possibleTasks[i].enabled = true;
        }

        public void OnTaskCompleted()
        {
            _completedTasks++;
        }
        public void OnTaskFailed()
        {
            _failedTasks++;
        }
    }
}
