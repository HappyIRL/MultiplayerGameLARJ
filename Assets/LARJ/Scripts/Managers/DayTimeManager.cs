using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeManager : MonoBehaviour
{
    [Header("Times"), Tooltip("Hours in 24h format (0-24)")]
    public int DayStartTime = 6;
    public int DayEndTime = 18;
    public int RealtimeLengthInMinutes = 5;

    [Header("Clocks")]
    [SerializeField] private List<Clock> _clocks = new List<Clock>();

    [Header("Lights")]
    [SerializeField] private Light _sunLight = null;

    [SerializeField] private List<Light> _indirectLights = new List<Light>();
    [SerializeField] private List<GameObject> _lights = new List<GameObject>();

    private int _currentHour;
    private int _currentMinutes;
    private float _timeForOneInGameMinuteInRealSecs;
    private float _timer = 0f;
    private bool _startClock = true;
    private float _sunXRotation;
    private Coroutine _lastIndirectLightCoroutine;
    private Coroutine _lastSunLightCoroutine;

    void Start()
    {
        SetTimeForAllClocks(DayStartTime);
        _currentHour = DayStartTime;
        _currentMinutes = 0;

        _timeForOneInGameMinuteInRealSecs = 1 / (float)((DayEndTime - DayStartTime) / (float)RealtimeLengthInMinutes);
        SetSunLight();
    }

    void Update()
    {        
        if (_startClock)
        {
            _timer += Time.deltaTime;            

            if (_timer >= _timeForOneInGameMinuteInRealSecs)
            {
                _timer = 0;

                _currentMinutes++;
                if (_currentMinutes == 60)
                {
                    _currentMinutes = 0;
                    _currentHour++;

                    if (_currentHour == DayEndTime)
                    {
                        _startClock = false;
                    }

                    if (_currentHour == 24)
                    {
                        _currentHour = 0;
                        _sunLight.transform.eulerAngles = new Vector3(-90f, transform.eulerAngles.y, transform.eulerAngles.z);
                    }
                }

                SetTimeForAllClocks(_currentHour, _currentMinutes);
                SetSunLight();  
            }

            
            float xRotation = Mathf.Lerp(_sunLight.transform.eulerAngles.x, _sunXRotation, (Mathf.Abs(_sunLight.transform.eulerAngles.x - _sunXRotation) * Time.deltaTime) / _timeForOneInGameMinuteInRealSecs);
            _sunLight.transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    private void SetTimeForAllClocks(int hour)
    {
        for (int i = 0; i < _clocks.Count; i++)
        {
            _clocks[i].SetTime(hour);
        }
    }

    private void SetTimeForAllClocks(int hour, int minutes)
    {
        for (int i = 0; i < _clocks.Count; i++)
        {
            _clocks[i].SetTime(hour, minutes);
        }
    }

    private void SetSunLight()
    {
        if (_currentMinutes == 0)
        {
            if (_currentHour >= 7 && _currentHour < 17)
            {
                if (_lastSunLightCoroutine != null)
                {
                    StopCoroutine(_lastSunLightCoroutine);
                }
                _lastSunLightCoroutine = StartCoroutine(ChangeSunLightShadowStrengthCoroutine(0.5f));

                for (int i = 0; i < _lights.Count; i++)
                {
                    _lights[i].SetActive(false);
                }

                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.2f));
            }
        
            if (_currentHour >= 11 && _currentHour < 17)
            {
                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.3f));
            }

            if (_currentHour >= 17 || _currentHour < 7)
            {
                if (_lastSunLightCoroutine != null)
                {
                    StopCoroutine(_lastSunLightCoroutine);
                }
                _lastSunLightCoroutine = StartCoroutine(ChangeSunLightShadowStrengthCoroutine(0.01f));

                for (int i = 0; i < _lights.Count; i++)
                {
                    _lights[i].SetActive(true);
                }

                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.1f));
            }
        }
        
        _sunXRotation = ((15 * _currentHour - 90f) + 0.25f * _currentMinutes);
    }

    private IEnumerator ChangeIndirectLightIntensityCoroutine(float intensity)
    {
        float currentIntensity = _indirectLights[0].intensity;

        if (currentIntensity > intensity)
        {
            while (currentIntensity > intensity)
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, intensity, 0.1f * Time.deltaTime);

                for (int i = 0; i < _indirectLights.Count; i++)
                {
                    _indirectLights[i].intensity = currentIntensity;
                }
                yield return null;
            }
        }
        else
        {
            while (currentIntensity < intensity)
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, intensity, 0.1f * Time.deltaTime);

                for (int i = 0; i < _indirectLights.Count; i++)
                {
                    _indirectLights[i].intensity = currentIntensity;
                }
                yield return null;
            }
        }
    }

    private IEnumerator ChangeSunLightShadowStrengthCoroutine(float strength)
    {
        float currentStrength = _sunLight.shadowStrength;

        if (currentStrength > strength)
        {
            while (currentStrength > strength)
            {
                currentStrength = Mathf.MoveTowards(currentStrength, strength, 0.1f * Time.deltaTime);
                _sunLight.shadowStrength = currentStrength;
                
                yield return null;
            }
        }
        else
        {
            while (currentStrength < strength)
            {
                currentStrength = Mathf.MoveTowards(currentStrength, strength, 0.1f * Time.deltaTime);
                _sunLight.shadowStrength = currentStrength;                
                yield return null;
            }
        }
    }
}
