using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class HealthbarCanvasHolder : MonoBehaviour
{
    [SerializeField] private GameObject _healthbarCanvasPrefab = null;
    public GameObject HealthbarCanvasPrefab
    {
        get => _healthbarCanvasPrefab;
    }

    private static HealthbarCanvasHolder _instance;
    public static HealthbarCanvasHolder Instance
    {
        get
        {
            return _instance;
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        Instance = this;
    }
}
