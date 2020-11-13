using System.Collections;
using TMPro;
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
        private Coroutine _timerCoroutine;
        private Color _orange = new Color(0.96f, 0.57f, 0.03f, 1);
        float _timer = 0f;

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
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(positionX, transform.position.y, transform.position.z), Screen.width * Time.deltaTime);
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

            _timer = 0f;
            _timerCoroutine = StartCoroutine(StartTaskTimerCoroutine());
        }

        public void OnEnqueuedToPool()
        {

        }

        private IEnumerator StartTaskTimerCoroutine()
        {
            while (_timer < _timeToComplete)
            {
                _timer += Time.deltaTime;
                _timerImage.fillAmount = (_timeToComplete - _timer) / _timeToComplete;
                if (0.5f > _timer / _timeToComplete)
                {

                    _timerImage.color = Color.Lerp(_orange, Color.green, 1 - _timer * 2 / _timeToComplete);
                }
                else
                {
                    _timerImage.color = Color.Lerp(_orange, Color.red, _timer * 2 / _timeToComplete - 1);
                }
                yield return null;
            }
        }

        public void StopTaskUITimer()
        {
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        }
        public void StartTaskUITimer()
        {
            _timerCoroutine = StartCoroutine(StartTaskTimerCoroutine());
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