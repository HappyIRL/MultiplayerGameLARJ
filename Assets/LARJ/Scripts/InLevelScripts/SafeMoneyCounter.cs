using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeMoneyCounter : MonoBehaviour
{
    [SerializeField] private Transform _MoneyCheckPoint = null;

    private int _moneyCount = 0;
    private Queue<GameObject> _invisibleMoney = new Queue<GameObject>();

    private float _timeToCheckMoney = 1f;
    private float _timer = 0f;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _timeToCheckMoney)
        {
            _timer = 0;
            CheckMoney();
        }
    }

    private void CheckMoney()
    {
        Collider[] colliders = Physics.OverlapSphere(_MoneyCheckPoint.position, 1f, LayerMask.GetMask("Money"));
        _moneyCount = colliders.Length;

        if (_moneyCount == 0) return;
        if (_moneyCount == 2) return;

        if (_moneyCount == 1)
        {
            if(_invisibleMoney.Count > 0)
            {
                GameObject money = _invisibleMoney.Dequeue();
                money.SetActive(true);
            }
        }
        if (_moneyCount > 2)
        {           
            for (int i = 2; i < _moneyCount; i++)
            {
                _invisibleMoney.Enqueue(colliders[i].gameObject);
                colliders[i].gameObject.SetActive(false);
            }
        }
    }   

    public void StealAllMoney()
    {
        _invisibleMoney.Clear();

        Collider[] colliders = Physics.OverlapSphere(_MoneyCheckPoint.position, 1f, LayerMask.GetMask("Money"));
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.SetActive(false);
        }

        _moneyCount = 0;
    }
}
