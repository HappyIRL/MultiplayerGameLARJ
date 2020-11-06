using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawner")] // Set Difficulty on spawners at Start based on day and players
    [SerializeField] private CrackerSpawner _crackerSpawner = null;
    [SerializeField] private FireSpawner _fireSpawner = null;
    [SerializeField] private CustomerSpawner _customerSpawner = null; //spawnrate & active desks

    [Header("Day time")] // Set day start/end time and how long the day should be
    [SerializeField] private DayTimeManager _dayTimeManager = null;
}
