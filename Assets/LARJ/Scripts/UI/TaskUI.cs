using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace Tasks
{


    [RequireComponent(typeof(PooledObject), typeof(RectTransform))]
    public class TaskUI : MonoBehaviour, IObjectPoolNotifier
    {
        [HideInInspector] public ObjectPool TaskUIPool = null;
        [HideInInspector] public RectTransform RectTransform = null;

        [Header("Attributes")]
        [SerializeField] private TextMeshProUGUI _rewardText = null;
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private Image _timerImage = null;
        [SerializeField] private Image _taskIcon = null;
        private float _timeToComplete = 10f;
        private Coroutine _lastCoroutine;
        private Color _orange = new Color(0.96f, 0.57f, 0.03f, 1);

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

            StartCoroutine(StartTaskTimerCorutine());
        }

        public void OnEnqueuedToPool()
        {

        }

        private IEnumerator StartTaskTimerCorutine()
        {
            float timer = 0f;

            while (timer < _timeToComplete)
            {
                timer += Time.deltaTime;
                _timerImage.fillAmount = (_timeToComplete - timer) / _timeToComplete;
                //_timerImage.color = Color.white;
                if (0.5f > timer / _timeToComplete)
                {

                    _timerImage.color = Color.Lerp(_orange, Color.green, 1 - timer * 2 / _timeToComplete);
                }
                else
                {
                    _timerImage.color = Color.Lerp(_orange, Color.red, timer * 2 / _timeToComplete - 1);
                }
                yield return null;
            }
        }

        public void SetUIValues(string taskTitle, int rewardMoney, float taskTime, Sprite taskIcon)
        {
            _titleText.text = taskTitle;
            _rewardText.text = rewardMoney.ToString();
            _timeToComplete = taskTime;
            _taskIcon.sprite = taskIcon;
        }
    }
}