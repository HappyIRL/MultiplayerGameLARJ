using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BulletImpact : MonoBehaviour
{
    private CharacterController _characterController;
    private Coroutine _forceCoroutine;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void AddForce(Vector3 direction, float force)
    {
        direction = direction.normalized;
        Vector3 impact = direction * force;
        if (_forceCoroutine != null) StopCoroutine(_forceCoroutine);
        _forceCoroutine = StartCoroutine(AddForceCoroutine(impact));
    }
    private IEnumerator AddForceCoroutine(Vector3 impact)
    {
        while (impact.magnitude > 0.2f)
        {
            _characterController.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
            yield return null;
        }
    }
}
