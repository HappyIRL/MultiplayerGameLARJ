using Photon.Pun;
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

    private float _timeForOneInGameMinuteInRealSecs;
    private bool _startClock = true;
    private float _sunXRotation;
    private Coroutine _lastIndirectLightCoroutine;
    private Coroutine _lastSunLightCoroutine;
    private float _timer = 0;

    public int CurrentHour { get; private set; }
    public int CurrentMinutes { get; private set; }

    private void Awake()
	{
        CurrentMinutes = 0;
        SetTimeForAllClocks(DayStartTime);
        CurrentHour = DayStartTime;
        _timeForOneInGameMinuteInRealSecs = 1 / (float)((DayEndTime - DayStartTime) / (float)RealtimeLengthInMinutes);
        SetSunLight();
    }

	private void Start()
	{
		
	}

	void Update()
    {
        if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
		{
            if (_startClock)
            {
                _timer += Time.deltaTime;

                if (_timer >= _timeForOneInGameMinuteInRealSecs)
                {
                    _timer = 0;

                    CurrentMinutes++;
                    if (CurrentMinutes == 60)
                    {
                        CurrentMinutes = 0;
                        CurrentHour++;

                        if (CurrentHour == DayEndTime)
                        {
                            _startClock = false;
                        }

                        if (CurrentHour == 24)
                        {
                            CurrentHour = 0;
                            _sunLight.transform.eulerAngles = new Vector3(-90f, transform.eulerAngles.y, transform.eulerAngles.z);
                        }
                    }

                    SetTimeForAllClocks(CurrentHour, CurrentMinutes);
                    SetSunLight();
                }


                float xRotation = Mathf.Lerp(_sunLight.transform.eulerAngles.x, _sunXRotation, (Mathf.Abs(_sunLight.transform.eulerAngles.x - _sunXRotation) * Time.deltaTime) / _timeForOneInGameMinuteInRealSecs);
                _sunLight.transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }

    private void SetTimeForAllClocks(int hour)
    {
        for (int i = 0; i < _clocks.Count; i++)
        {
            _clocks[i].SetTime(hour);
        }
    }

    public void SetTimeForAllClocks(int hour, int minutes)
    {
        for (int i = 0; i < _clocks.Count; i++)
        {
            _clocks[i].SetTime(hour, minutes);
        }
    }

    public void SetSunLight()
    {
        if (CurrentMinutes == 0)
        {
            if (CurrentHour >= 7 && CurrentHour < 17)
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
        
            if (CurrentHour >= 11 && CurrentHour < 17)
            {
                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.3f));
            }

            if (CurrentHour >= 17 || CurrentHour < 7)
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
        
        _sunXRotation = ((15 * CurrentHour - 90f) + 0.25f * CurrentMinutes);
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
