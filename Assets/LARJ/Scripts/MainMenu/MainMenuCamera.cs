using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private float _cameraSpeed = 1f;

    [SerializeField] private Transform _leftFront = null;
    [SerializeField] private Transform _rightFront = null;
    [SerializeField] private Transform _leftBack = null;
    [SerializeField] private Transform _rightBack = null;


    private void Start()
    {
        StartMoving();
    }

    public void StartMoving()
    {
        _animator.SetBool("FadeIn", true);
        Vector3 camStartPosition, camEndPosition;

        if (Random.Range(0,2) == 0)
        {
            float height = Random.Range(6f, 12f);
            camStartPosition = new Vector3(_leftFront.position.x, height, Random.Range(_leftFront.position.z, _leftBack.position.z));
            camEndPosition = new Vector3(_rightFront.position.x, height, Random.Range(_leftFront.position.z, _leftBack.position.z));
        }
        else
        {
            float height = Random.Range(6f, 12f);
            camStartPosition = new Vector3(_rightFront.position.x, height, Random.Range(_leftFront.position.z, _leftBack.position.z));
            camEndPosition = new Vector3(_leftFront.position.x, height, Random.Range(_leftFront.position.z, _leftBack.position.z));
        }

        _camera.position = camStartPosition;
        StartCoroutine(CamMoving(camEndPosition));
    }

    private IEnumerator CamMoving(Vector3 endPoint)
    {
        if (_camera.position.x < endPoint.x)
        {
            while (_camera.position.x < endPoint.x)
            {
                _camera.position = Vector3.MoveTowards(_camera.position, endPoint, _cameraSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (_camera.position.x > endPoint.x)
            {
                _camera.position = Vector3.MoveTowards(_camera.position, endPoint, _cameraSpeed * Time.deltaTime);
                yield return null;
            }
        }

        _animator.SetBool("FadeIn", false);
    }
}
