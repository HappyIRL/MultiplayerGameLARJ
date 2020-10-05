using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedPlayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

	[SerializeField] public GameObject _objectHolder;
	[SerializeField] public GameObject _baseCharacter;

    public void SetHairColor(Color color)
	{
		Material mat = new Material(_meshRenderer.material);
		mat.color = color;
		_meshRenderer.material = mat;
	}
}
