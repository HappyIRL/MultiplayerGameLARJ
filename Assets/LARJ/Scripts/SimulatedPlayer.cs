using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedPlayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    public void SetHairColor(Color color)
	{
		Material mat = new Material(_meshRenderer.material);
		mat.color = color;
		_meshRenderer.material = mat;
	}
}
