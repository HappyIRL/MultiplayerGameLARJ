using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManagerUI : MonoBehaviour
{
    [SerializeField] private ObjectPool _taskUIPool = null;
    [SerializeField] private Transform _parentCanvas = null;
    [SerializeField] private Transform _taskSpawnPoint = null;
    [SerializeField] private Transform _firstTaskPoint = null;
    [SerializeField] private float _spaceBetweenTasks = 10f;

    private List<TaskUI> _activeUITasks = new List<TaskUI>();

    public void SpawnUITask(string taskName, string rewardText, float timeToCompleteTask)
    {
        GameObject obj = _taskUIPool.GetObject();
        TaskUI task = obj.GetComponent<TaskUI>();

        task.TaskTitle = taskName;
        task.TaskRewardText = rewardText;
        task.TimeToComplete = timeToCompleteTask;

        obj.transform.parent = _parentCanvas;
        obj.transform.position = _taskSpawnPoint.position;
        _activeUITasks.Add(task);

        AlignTaskUI();
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
