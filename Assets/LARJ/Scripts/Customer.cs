using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Tasks;
using Photon.Pun;
using UnityEngine.UI;
using System.Threading;

public class Customer : Interactable, IObjectPoolNotifier, IQueueUpdateNotifier
{
    private StateMachine _stateMachine;
    NavMeshAgent _agent;

    private Transform _deskWaypoint;

    [HideInInspector] public Transform despawn;

    private readonly float _range = 3f;
    private int _queuePosition;
    private bool _isWaiting;
    private Coroutine _currentCoroutine;

    private float _timeToFinishTask;
    private float _timer;
    public bool _isWaitingForMoney = false;

    private CustomerManager cm;
    [SerializeField] private List<GameObject> _customerModels = null;
    [SerializeField] private Image _patienceImage;
    [SerializeField] private Image _patienceImageBackground;

    public override void Awake()
    {
        base.Awake();
        InteractableID = (InteractableObjectID)73;
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        _timeToFinishTask = GetComponent<Task>().GetTimeToFinishTask;
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
        var WaitForMoney = _stateMachine.CreateState("WaitForMoney", WaitForMoneyStart, WaitForMoneyExit);

        _agent.enabled = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (_isWaitingForMoney)
        {
            if (collision.gameObject.tag == "Money")
            {
                Destroy(collision.gameObject);
                _stateMachine.TransitionTo("Leaving");
                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                }
                TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
            }
        }
    }

    #region WaitForMoney State
    private void WaitForMoneyStart()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _isWaitingForMoney = true;
        StartCoroutine(LeaveAfterDelay());
        TaskManager.TaskManagerSingelton.StartMoneyTask(GetComponent<Task>());
    }

    private void WaitForMoneyExit()
    {
    }

    #endregion


    #region Entry State
    private void EntryStart()
    {
        cm = CustomerManager.instance;

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
        var rot = transform.rotation.eulerAngles;
        _agent.updateRotation = false;
        transform.Rotate(-rot);
        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            TaskManager.TaskManagerSingelton.StartTask(GetComponent<Task>());
    }
    private void AtDeskUpdate()
    {

    }
    private void AtDeskExit()
    {
        _agent.updateRotation = true;
        cm.LeftDesk(_deskWaypoint);
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
        _patienceImage.gameObject.SetActive(false);
        _patienceImageBackground.gameObject.SetActive(false);
        _stateMachine.TransitionTo("Leaving");
    }

    #endregion
    #region Leaving State
    private void LeavingStart()
    {
        _isWaitingForMoney = false;
        _agent.destination = despawn.position;
        _patienceImage.gameObject.SetActive(false);
        _patienceImageBackground.gameObject.SetActive(false);
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


    #endregion

    private void OnEnterTalk()
    {
    }
    private void OnFinishedTalk()
    {
        // Log Successful Talk
        _stateMachine.TransitionTo("Leaving");
    } // SUCCESSFUL TALK

    private void OnFailedTalk()
    {
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

        ChooseRandomModel();
    }


    private void ChooseRandomModel()
    {
        for (int i = 0; i < _customerModels.Count; i++) _customerModels[i].SetActive(false);
        _customerModels[UnityEngine.Random.Range(0, _customerModels.Count)].SetActive(true);
    }

    #region Interactable Events
    public override void HoldingStartedEvent()
    {
        base.HoldingStartedEvent();
        OnEnterTalk();
    }

    public override void HoldingFailedEvent()
    {
        base.HoldingFailedEvent();
        OnFailedTalk();
    }

    public override void HoldingFinishedEvent()
    {
        base.HoldingFinishedEvent();
        OnFinishedTalk();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }
    public override void OnNetworkHoldingFinishedEvent()
    {
        OnFinishedTalk();
    }

    public override void PressTheCorrectKeysFinishedEvent()
    {
        base.PressTheCorrectKeysFinishedEvent();

        OnFinishedTalk();
        TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
    }
    public override void PressTheCorrectKeysStartedEvent(string currentPlayerControlScheme)
    {
        base.PressTheCorrectKeysStartedEvent(currentPlayerControlScheme);
    }
    public override void PressEvent()
    {
        base.PressEvent();
        if (!_isWaitingForMoney)
        {
            _isWaitingForMoney = true;
            _timer = 0;
            _stateMachine.TransitionTo("WaitForMoney");
            TaskManager.TaskManagerSingelton.OnTaskCompleted(GetComponent<Task>());
        }
    }

    #endregion
}
