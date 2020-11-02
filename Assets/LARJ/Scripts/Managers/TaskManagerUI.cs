using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tasks
{

    public class TaskManagerUI : MonoBehaviour
    {
        [SerializeField] private ObjectPool _taskUIPool = null;
        [SerializeField] private Transform _parentCanvas = null;
        [SerializeField] private Transform _taskSpawnPoint = null;
        [SerializeField] private Transform _firstTaskPoint = null;
        [SerializeField] private float _spaceBetweenTasks = 10f;

        [Header("Icons")]
        [SerializeField] private Sprite _telephoneIcon = null;
        [SerializeField] private Sprite _mailIcon = null;
        [SerializeField] private Sprite _customerIcon = null;
        [SerializeField] private Sprite _cleaningIcon = null;
        [SerializeField] private Sprite _moneyIcon = null;
        [SerializeField] private Sprite _printerIcon = null;
        [SerializeField] private Sprite _paperboxIcon = null;

        private List<TaskUI> _activeUITasks = new List<TaskUI>();

        public TaskUI SpawnUITask(TaskType taskType, int rewardMoney, float timeToCompleteTask)
        {
            GameObject obj = _taskUIPool.GetObject();
            TaskUI task = obj.GetComponent<TaskUI>();

            obj.transform.SetParent(_parentCanvas);
            obj.transform.position = _taskSpawnPoint.position;
            _activeUITasks.Add(task);

            string title = null;
            Sprite icon = null;

            switch (taskType)
            {
                case TaskType.PhoneCall:
                    title = "Answer Phone!";
                    icon = _telephoneIcon;
                    break;
                case TaskType.Printer:
                    title = "Print Documents!";
                    icon = _printerIcon;
                    break;
                case TaskType.Customer:
                    title = "Serve Customer!";
                    icon = _customerIcon;
                    break;
                case TaskType.Cleaning:
                    title = "Pick up Garbage!";
                    icon = _cleaningIcon;
                    break;
                case TaskType.Money:
                    title = "Bring the Customer Money";
                    icon = _moneyIcon;
                    break;
                case TaskType.Mail:
                    title = "Reply E-Mail";
                    icon = _mailIcon;
                    break;
                case TaskType.Paper:
                    title = "Bring Paper To PaperBox";
                    icon = _paperboxIcon;
                    break;
                case TaskType.NotAssigned:
                    Debug.Log("Error No Task Type was assigned");
                    break;
            }

            task.SetUIValues(title, rewardMoney, timeToCompleteTask, icon);
            AlignTaskUI();
            return task;
        }

        public void RemoveUITask(TaskUI taskUI)
        {
            if (_activeUITasks.Contains(taskUI))
            {
                //remove from list
                _activeUITasks.Remove(taskUI);

                //go out of screen (up)
                taskUI.MoveUp();

                //Allign other tasks
                AlignTaskUI();
            }
        }
        public void RemoveFirstUITask()
        {
            //go out of screen (up)
            _activeUITasks[0].MoveUp();

            //remove from list
            _activeUITasks.RemoveAt(0);

            //Allign other tasks
            AlignTaskUI();

        }
        private void AlignTaskUI()
        {
            if (_activeUITasks.Count <= 0) return;

            _activeUITasks[0].MoveTo(_firstTaskPoint.position.x);
            for (int i = 1; i < _activeUITasks.Count; i++)
            {
                float x = _firstTaskPoint.position.x;
                for (int j = i; j > 0; j--)
                {
                    x += _activeUITasks[j - 1].RectTransform.rect.width + _spaceBetweenTasks;
                }
                _activeUITasks[i].MoveTo(x);
            }
        }

    }

}