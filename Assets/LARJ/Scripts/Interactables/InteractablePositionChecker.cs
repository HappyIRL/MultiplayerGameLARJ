using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePositionChecker : MonoBehaviour
{
    [SerializeField] private Transform _leftWall = null;
    [SerializeField] private Transform _topWall = null;
    [SerializeField] private Transform _rightWall = null;
    [SerializeField] private Transform _bottomWall = null;

    public static InteractablePositionChecker Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CheckPosition(Transform interactable)
    {
        if (interactable.position.x < _leftWall.position.x)
        {
            Vector3 newPosition = interactable.position;
            newPosition.x = _leftWall.position.x + 1f;
            interactable.position = newPosition;
        }
        else if (interactable.position.x > _rightWall.position.x)
        {
            Vector3 newPosition = interactable.position;
            newPosition.x = _rightWall.position.x - 1f;
            interactable.position = newPosition;
        }
        else if (interactable.position.z > _topWall.position.z)
        {
            Vector3 newPosition = interactable.position;
            newPosition.z = _topWall.position.z - 1f;
            interactable.position = newPosition;
        }
        else if (interactable.position.z < _bottomWall.position.z)
        {
            Vector3 newPosition = interactable.position;
            newPosition.z = _bottomWall.position.z + 1f;
            interactable.position = newPosition;
        }
    }
}
