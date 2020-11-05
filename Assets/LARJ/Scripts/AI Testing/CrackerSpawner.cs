using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackerSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool _crackerPool;
    [SerializeField] private Transform _spawnPoint;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _dustParticles = null;
    [SerializeField] private ParticleSystem _dirtParticles = null;

    [Header("Cracks")]
    [SerializeField] private List<GameObject> _crackImages = null;


    private void Start()
    {
        StartSpawningCracker();
    }

    public void StartSpawningCracker()
    {
        StartCoroutine(StartCrackerVisuals());
    }
    private IEnumerator StartCrackerVisuals()
    {
        _dustParticles.Play();
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < _crackImages.Count; i++)
        {
            SpawnCrack(i);
            yield return new WaitForSeconds(1f);
        }

        SpawnCracker();
        StopParticles();
    }
    private void SpawnCracker()
    {        
        var go = _crackerPool.GetObject();
        var cracker = go.GetComponent<Cracker>();

        cracker.crackerPool = _crackerPool;
        cracker.CrackerSpawner = this;
        go.transform.position = _spawnPoint.position;           
    }
    private void SpawnCrack(int crackIndex)
    {
        _dirtParticles.Play();
        for (int i = 0; i < _crackImages.Count; i++)
        {
            if (i == crackIndex) _crackImages[i].SetActive(true);
            else _crackImages[i].SetActive(false);
        }
    }
    private void StopParticles()
    {
        _dirtParticles.Stop();
        _dustParticles.Stop();
    }
    public void CloseHole()
    {
        StartCoroutine(CloseHoleCoroutine());
    }
    private IEnumerator CloseHoleCoroutine()
    {
        for (int i = _crackImages.Count - 1; i >= 0; i--)
        {
            SpawnCrack(i);
            yield return new WaitForSeconds(1f);
        }
        _crackImages[0].SetActive(false);
    }
}
