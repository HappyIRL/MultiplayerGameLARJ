using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomPositionSetter : MonoBehaviour
{
    [Serializable]
    private struct ChangableObject
    {
        public Transform Object;
        public List<Transform> SpawnPositions;
    }

    [SerializeField] private List<ChangableObject> _changableObjects = null;

    void Awake()
    {
        for (int i = 0; i < _changableObjects.Count; i++)
        {
            _changableObjects[i].Object.position = _changableObjects[i].SpawnPositions[UnityEngine.Random.Range(0, _changableObjects[i].SpawnPositions.Count)].position;
        }
    }
}
