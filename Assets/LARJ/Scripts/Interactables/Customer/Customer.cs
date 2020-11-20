using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Tasks;
using Photon.Pun;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class Customer : Interactable, IObjectPoolNotifier, IQueueUpdateNotifier
{
    private StateMachine _stateMachine;
    NavMeshAgent _agent;
    
    [SerializeField] private List<GameObject> _customerModels = null;
    [SerializeField] private Image _patienceImage;
    [SerializeField] private Image _patienceImageBackground;
    [SerializeField] private GameObject _speechBubble = null;
    [SerializeField] private GameObject _moneyImage = null;
    [SerializeField] private GameObject _documentImage = null;
    [SerializeField] private TextMeshProUGUI _customerSpeechText = null;

    private CustomerTalkingVisuals _customerTalkingVisuals;
    private CustomerManager cm;
    private Transform _deskWaypoint;
    [HideInInspector] public Transform despawn;
    private readonly float _range = 3f;
    
    private int _queuePosition;
    private Coroutine _currentCoroutine;
    private float _timer;
    private float _timeToFinishTask;
    private bool _isStopped = false;

    public bool _isWaitingForMoney = false;
    public bool _isWaitingForDocument = false;
    public bool _wasHitByBullet = false;
    public bool WantsMoney;
    public bool HasPress = true;

    public override void Awake()
    {
        base.Awake();
        InteractableID = (InteractableObjectID)73;
        AlwaysInteractable = true;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        _timeToFinishTask = GetComponent<Task>().GetTimeToFinishTask;
        _customerTalkingVisuals = GetComponent<CustomerTalkingVisuals>();
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    public override void Start()
    {
        base.Start();
        _stateMachine = new StateMachine();
        var Entry = _stateMachine.CreateState("Entry", EntryStart);
        var InQueue = _stateMachine.CreateState("InQueue", QueueStart);
        var MoveToDesk = _stateMachine.CreateState("MoveToDesk", MoveToDeskStart, MoveToDeskUpdate);
        var AtDesk = _stateMachine.CreateState("AtDesk", AtDeskStart, AtDeskUpdate, AtDeskExit);
        var Leaving = _stateMachine.CreateState("Leaving", LeavingStart, LeavingUpdate);
        var WaitForMoney = _stateMachine.CreateState("WaitForMoney", WaitForMoneyStart, null, WaitForMoneyExit);
        var WaitForDocument = _stateMachine.CreateState("WaitForDocument", WaitForDocumnetStart, null, WaitForDocumentExit);

        _agent.enabled = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (_isWaitingForMoney)
        {
            if (collision.gameObject.tag == "Money")
            {
                Debug.LogError("COLLISION MONEY");
                if (_currentCoroutine != null)StopCoroutine(_currentCoroutine);
                if (!_isStopped)
                {
                    _stateMachine.TransitionTo("Leaving");
                    _isStopped = true;
                }
                TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
                collision.gameObject.SetActive(false);
            }
        }
        else if (_isWaitingForDocument)
        {
            if (collision.gameObject.tag == "Paper")
            {
                Debug.LogError("COLLISION Paper");

                if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

                var task = collision.gameObject.GetComponent<Task>();
                if (task.IsTaskActive) TaskManager.TaskManagerSingelton.OnTaskCancelled(task);
                if (!_isStopped)
                {
                    _stateMachine.TransitionTo("Leaving");
                    _isStopped = true;
                }
                TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
                collision.gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.tag == "Bullet")
        {
            if (!_wasHitByBullet)
            {
                if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
                    TaskManager.TaskManagerSingelton.OnTaskFailed(GetComponent<Task>());
                SetFearText();
                _wasHitByBullet = true;
            
                _stateMachine.TransitionTo("Leaving");
            }
        }
    }

    #region WaitForMoney State
    private void WaitForMoneyStart()
    {
         TaskManager.TaskManagerSingelton.StartMoneyTask(GetComponent<Task>());
        _isWaitingForMoney = true;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _speechBubble.SetActive(true);
        _moneyImage.SetActive(true);

        StartCoroutine(LeaveAfterDelay());

    }

    private void WaitForMoneyExit()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _speechBubble.SetActive(false);
        _moneyImage.SetActive(false);
    }

    #endregion
    #region WaitForDocument State
    private void WaitForDocumnetStart()
    {
        TaskManager.TaskManagerSingelton.StartDocumentTask(GetComponent<Task>());
        _isWaitingForDocument = true;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _speechBubble.SetActive(true);
        _documentImage.SetActive(true);

        StartCoroutine(LeaveAfterDelay());
        
    }

    private void WaitForDocumentExit()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _speechBubble.SetActive(false);
        _documentImage.SetActive(false);
    }

    #endregion


    #region Entry State
    private void EntryStart()
    {
        cm = CustomerManager.instance;
        _wasHitByBullet = false;

        _speechBubble.SetActive(false);
        _moneyImage.SetActive(false);
        _customerSpeechText.gameObject.SetActive(false);

        _stateMachine.TransitionTo("InQueue");
    }

    #endregion
    #region InQueue State
    private void QueueStart()
    {
        cm.EnqueueCustomer(this.gameObject);
        if (cm.DeskKvps.ContainsValue(true))
        {
            _stateMachine.TransitionTo("MoveToDesk");
            cm.DequeueCustomer();
        }

    }
    public void OnEnqueuedToQueue() // GET IN QUEUE POS
    {
        _queuePosition = cm.CustomerQueue.Count;

        _agent.destination = cm.QueueWaypoints[_queuePosition - 1];
    }

    public void OnQueueUpdated() // MOVE UP THE QUEUE
    {
        _queuePosition--;
        _agent.destination = cm.QueueWaypoints[_queuePosition - 1];
    }

    public void OnLeftQueue() // MOVE TO DESK
    {
        var desks = cm.DeskWaypoints;
        foreach (var desk in desks)
        {
            if (cm.DeskKvps[desk])
            {
                _agent.destination = desk.position;
                cm.JoinedDesk(desk);
                _deskWaypoint = desk;
                _stateMachine.TransitionTo("MoveToDesk");
                return;
            }
        }
    }

    #endregion
    private void MoveToDeskStart()
    {

    }
    private void MoveToDeskUpdate()
    {
        if (DidArriveAtDesk())
        {
            _stateMachine.TransitionTo("AtDesk");
        }
    }
    #region AtDesk State
    private void AtDeskStart()
    {
        _timer = 0;
        _currentCoroutine = StartCoroutine("LeaveAfterDelay");
        //var rot = transform.rotation.eulerAngles;
        _agent.updateRotation = false;
        //transform.Rotate(-rot);

        transform.rotation = Quaternion.LookRotation(_deskWaypoint.forward * -1,transform.up);

		if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            TaskManager.TaskManagerSingelton.StartTask(GetComponent<Task>());
	}
    private void AtDeskUpdate()
    {

    }
    private void AtDeskExit()
    {
        _agent.updateRotation = true;
    }
    IEnumerator LeaveAfterDelay()
    {
        _patienceImage.gameObject.SetActive(true);
        _patienceImageBackground.gameObject.SetActive(true);

        while (_timer < _timeToFinishTask) // PATIENCE ADDED HERE
        {
            _patienceImageBackground.fillAmount = 1 - _timer / _timeToFinishTask;
            _timer += Time.deltaTime;

            yield return null;
        }

        // Log Failed Task
        if(_stateMachine.currentState.name != "Leaving")
        {
            _patienceImage.gameObject.SetActive(false);
            _patienceImageBackground.gameObject.SetActive(false);
            CustomerLeavesAngry();
            _stateMachine.TransitionTo("Leaving");
        }
    }

    #endregion
    #region Leaving State
    private void LeavingStart()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

        _isWaitingForMoney = false;
        _isWaitingForDocument = false;
        _agent.destination = despawn.position;

        _patienceImage.gameObject.SetActive(false);
        _patienceImageBackground.gameObject.SetActive(false);

        cm.LeftDesk(_deskWaypoint);
        cm.DequeueCustomer();
    } // MOVE TO DESPAWN

    private void LeavingUpdate()
    {
        var distanceToDespawn = Vector3.Distance(transform.position, _agent.destination);
        if (distanceToDespawn <= _range)
        {
            PooledGameObjectExtensions.ReturnToPool(this.gameObject);
        }
    } // DESPAWN ON ARRIVAL

    private void CustomerLeavesAngry()
    {
        SetComplaintText();
    }

    #endregion

    private void OnEnterTalk()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
    }
    public void OnFinishedTalk()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

        if(!HasPress && !_isStopped)
		{
            _stateMachine.TransitionTo("Leaving");
		}
    }

    private void OnFailedTalk()
    {
        _currentCoroutine = StartCoroutine(LeaveAfterDelay());
    }


    private bool DidArriveAtDesk()
    {
        var distanceToDesk = Vector3.Distance(transform.position, _deskWaypoint.position);

        if (distanceToDesk <= _range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void OnEnqueuedToPool()
    {

    }
    public void OnCreatedOrDequeuedFromPool(bool created)
    {
        if (!created)
        {
            _stateMachine.TransitionTo("Entry");
        }
        _isStopped = false;
        ChooseRandomModel();
        _patienceImageBackground.gameObject.SetActive(false);
        _patienceImage.gameObject.SetActive(false);
    }


    private void ChooseRandomModel()
    {
        for (int i = 0; i < _customerModels.Count; i++) _customerModels[i].SetActive(false);
        _customerModels[UnityEngine.Random.Range(0, _customerModels.Count)].SetActive(true);
    }

    private void SetComplaintText()
    {
        _customerSpeechText.gameObject.SetActive(true);
        _speechBubble.SetActive(true);
        StartCoroutine(WaitToDeactivateSpeechBubble());

        string[] texts = { "I have no time!", "You're to slow!", "#@!*#~", "You will be fired!", "I will tell your boss!", "Faster!", "Get better!", "Good bye!", "Bye!", "You doing bad!", "I need a Coffee!",
            "Bad bank!", "I need Money!", "I'm angry!", "I'm mad!", "!?!?!?", "What's going on here?!", "I go somewhere else!", "OMG!", "Give me Money!", "My grandma can do better!" };

        _customerSpeechText.text = texts[UnityEngine.Random.Range(0,texts.Length)];
    }
    private void SetFearText()
    {
        _customerSpeechText.gameObject.SetActive(true);
        _speechBubble.SetActive(true);
        StartCoroutine(WaitToDeactivateSpeechBubble());

        string[] texts = { "OMG!", "Stop!", "Stop shooting!", "Ahhhhhh!", "Ahh", "Help!", "Medic!", "You're crazy!", "I never come back!", "Police!", "Call the police!", "I need help!", "You go to prison!",
            "Put the weapon down!", "I am hurted!", "What are you doing!?", "Let me go!"};

        _customerSpeechText.text = texts[UnityEngine.Random.Range(0, texts.Length)];
    }
    private IEnumerator WaitToDeactivateSpeechBubble()
    {
        yield return new WaitForSeconds(3f);
        _customerSpeechText.gameObject.SetActive(false);
        _speechBubble.SetActive(false);
    }
 

    #region Interactable Events
    public override void HoldingStartedEvent()
    {
        base.HoldingStartedEvent();
        OnEnterTalk();
        GetComponent<Task>().StopTaskCoolDown();
        _customerTalkingVisuals.ActivateTalkingVisuals();
    }


    public override void NetworkedHoldingStartedEvent()
    {
        base.NetworkedHoldingStartedEvent();
        OnEnterTalk();
        GetComponent<Task>().StopTaskCoolDown();
        _customerTalkingVisuals.ActivateTalkingVisuals();
    }


    public override void HoldingFailedEvent()
    {
        base.HoldingFailedEvent();
        OnFailedTalk();
        GetComponent<Task>().StartTaskCooldown();
        _customerTalkingVisuals.DeactivateTalkingVisuals();
    }

    public override void HoldingFinishedEvent()
    {
        base.HoldingFinishedEvent();
        OnFinishedTalk();
        _isStopped = true;
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
        _customerTalkingVisuals.DeactivateTalkingVisuals();
    }
    public override void OnNetworkHoldingFinishedEvent()
    {
        base.OnNetworkHoldingFinishedEvent();
        OnFinishedTalk();
        _isStopped = true;
        _customerTalkingVisuals.DeactivateTalkingVisuals();
    }

    public override void PressTheCorrectKeysFinishedEvent()
    {
        base.PressTheCorrectKeysFinishedEvent();
        OnFinishedTalk();
        _isStopped = true;
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }
    public override void PressTheCorrectKeysStartedEvent(string currentPlayerControlScheme)
    {
        base.PressTheCorrectKeysStartedEvent(currentPlayerControlScheme);
    }

	public override void StopInteractible()
	{
        if(!_isStopped)
		{
		    base.StopInteractible();
            OnFinishedTalk();
            _customerTalkingVisuals.DeactivateTalkingVisuals();
            _isStopped = true;
		}
    }

	public override void PressEvent()
    {
        base.PressEvent();

        if (!_isWaitingForDocument)
        {
            if (!_isWaitingForMoney)
            {
                _timer = 0;

                if(WantsMoney)
				{
                    _stateMachine.TransitionTo("WaitForMoney");
				}
                else
				{
                    _stateMachine.TransitionTo("WaitForDocument");
				}

            }
        }
    }

    #endregion
}
