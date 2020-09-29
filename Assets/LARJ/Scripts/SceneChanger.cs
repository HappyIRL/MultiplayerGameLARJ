using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//This script changes to the next scene with a fading animation.
public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;

    private int _sceneToLoad;
 

    public void FadeToScene(int sceneIndex)
    {
        _sceneToLoad = sceneIndex;
        _animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(_sceneToLoad);
    }
}
