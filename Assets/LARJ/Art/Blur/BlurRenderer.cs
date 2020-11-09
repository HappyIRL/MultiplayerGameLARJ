using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurRenderer : MonoBehaviour
{
    [SerializeField] private Camera _blurCamera = null;
    [SerializeField] private Material _blurMaterial = null;

    void Start()
    {
        if (_blurCamera.targetTexture != null)
        {
            _blurCamera.targetTexture.Release();
        }
        _blurCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);
        _blurMaterial.SetTexture("_RenTex", _blurCamera.targetTexture);
    }

}
