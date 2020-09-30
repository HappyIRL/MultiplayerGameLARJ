using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PooledObject), typeof(RectTransform))]
public class TaskUI : MonoBehaviour, IObjectPoolNotifier
{
    [HideInInspector] public ObjectPool TaskUIPool = null;
    [HideInInspector] public RectTransform RectTransform = null;

    [Header("Attributes")]
    [SerializeField] private TextMeshProUGUI _titleText = null;
    [SerializeField] private TextMeshProUGUI _rewardText = null;
    [SerializeField] private Image _timerImage = null;
    [HideInInspector] public string TaskTitle = "";
    [HideInInspector] public string TaskRewardText = "";
    [HideInInspector] public float TimeToComplete = 0f;

    private Coroutine _lastCoroutine;

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void MoveTo(float positionX)
    {
        if (_lastCoroutine != null)
        {
           StopCoroutine(_lastCoroutine);
        }

        _lastCoroutine = StartCoroutine(MoveCoroutine(positionX));
    }
    private IEnumerator MoveCoroutine(float positionX)
    {
        while (transform.position.x > positionX)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(positionX, transform.position.y, transform.position.z), 1000f * Time.deltaTime);
            yield return null;
        }
    }

    public void MoveUp()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }

        _lastCoroutine = StartCoroutine(MoveUpCoroutine());
    }
    private IEnumerator MoveUpCoroutine()
    {
        float endY = transform.position.y + 1000f;
        while (transform.position.y < endY)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, endY, transform.position.z), 1000f * Time.deltaTime);
            yield return null;
        }
        TaskUIPool.ReturnObject(gameObject);
    }


    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (created)
        {
            TaskUIPool = GetComponent<PooledObject>()._pool;
        }

        _titleText.text = TaskTitle;
        _rewardText.text = TaskRewardText;
        _timerImage.fillAmount = 1f;

        StartCoroutine(StartTaskTimerCorutine());
    }

    public void OnEnqueuedToPool()
    {
        
    }

    private IEnumerator StartTaskTimerCorutine()
    {
        float timer = 0f;

        while (timer < TimeToComplete)
        {
            timer += Time.deltaTime;
            _timerImage.fillAmount = (TimeToComplete - timer) / TimeToComplete;
            yield return null;
        }
    }
}
