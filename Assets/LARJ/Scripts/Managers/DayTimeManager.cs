using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeManager : MonoBehaviour
{
    [SerializeField] private DayManager _dayManager = null;

    [Header("Times"), Tooltip("Hours in 24h format (0-24)")]
    public int DayStartTime = 6;
    public int DayEndTime = 18;
    public int RealtimeLengthInMinutes = 5;

    [Header("Clocks")]
    [SerializeField] private List<Clock> _clocks = new List<Clock>();

    [Header("Lights")]
    [SerializeField] private Light _sunLight = null;

    [SerializeField] private List<Light> _indirectLights = new List<Light>();
    [SerializeField] private List<Light> _lights = new List<Light>();
    [SerializeField] private List<GameObject> _postLampsLights = new List<GameObject>();

    private float _timeForOneInGameMinuteInRealSecs;
    private bool _startClock = true;
    private float _sunXRotation;
    private Coroutine _lastIndirectLightCoroutine;
    private Coroutine _lastSunLightCoroutine;
    private Coroutine _lastSunLightIntensityCoroutine;
    private float _timer = 0;
    private float _currentSunAngle = 0;

    public int CurrentHour { get; private set; }
    public int CurrentMinutes { get; private set; }

    private void Awake()
	{
        CurrentMinutes = 0;
        CurrentHour = DayStartTime;
        _timeForOneInGameMinuteInRealSecs = 1 / (float)((DayEndTime - DayStartTime) / (float)RealtimeLengthInMinutes);

        SetTimeForAllClocks(DayStartTime);
        SetLights();
    }

	private void Start()
	{
        _sunXRotation = ((15 * CurrentHour - 90f) + 0.25f * CurrentMinutes);
        RotateSun(_sunXRotation);
    }

	void Update()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
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
                            EndDay();
                        }

                        if (CurrentHour == 24)
                        {
                            CurrentHour = 0;
                            _sunLight.transform.eulerAngles = new Vector3(-90f, 90, transform.eulerAngles.z);
                        }
                    }

                    SetTimeForAllClocks(CurrentHour, CurrentMinutes);
                    SetLights();

                    _sunXRotation = ((15 * CurrentHour - 90f) + 0.25f * CurrentMinutes);
                }

                float angle = Mathf.Lerp(_currentSunAngle, _sunXRotation, _timer/ _timeForOneInGameMinuteInRealSecs);
                RotateSun(angle - _currentSunAngle);
            }
        }
    }
    private void RotateSun(float angle)
    {
        _sunLight.transform.Rotate(Vector3.right, angle);
        _currentSunAngle += angle;
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

    public void SetLights()
    {
        if (CurrentMinutes == 0)
        {
            #region sunlight shadow strength
            if (CurrentHour >= 7 && CurrentHour < 17)
            {
                if (_lastSunLightCoroutine != null)
                {
                    StopCoroutine(_lastSunLightCoroutine);
                }
                _lastSunLightCoroutine = StartCoroutine(ChangeSunLightShadowStrengthCoroutine(0.5f));

                for (int i = 0; i < _lights.Count; i++)
                {
                    _lights[i].intensity = 0.1f;
                }
                for (int i = 0; i < _postLampsLights.Count; i++)
                {
                    _postLampsLights[i].SetActive(false);
                }

                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.2f));
            }
            #endregion

            #region midday - lateday
            if (CurrentHour >= 11 && CurrentHour < 17)
            {
                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.3f));
            }
            #endregion

            #region midday
            if (CurrentHour >= 11 && CurrentHour < 13)
            {
                if (_lastSunLightIntensityCoroutine != null)
                {
                    StopCoroutine(_lastSunLightIntensityCoroutine);
                }
                _lastSunLightIntensityCoroutine = StartCoroutine(ChangeSunLightIntensityCoroutine(0.75f));
            }
            else
            {
                if (_sunLight.intensity != 1)
                {
                    if (_lastSunLightIntensityCoroutine != null)
                    {
                        StopCoroutine(_lastSunLightIntensityCoroutine);
                    }
                    _lastSunLightIntensityCoroutine = StartCoroutine(ChangeSunLightIntensityCoroutine(1f));
                }
            }
            #endregion

            #region evening
            if (CurrentHour >= 17 || CurrentHour < 7)
            {
                if (_lastSunLightCoroutine != null)
                {
                    StopCoroutine(_lastSunLightCoroutine);
                }
                _lastSunLightCoroutine = StartCoroutine(ChangeSunLightShadowStrengthCoroutine(0.01f));

                for (int i = 0; i < _lights.Count; i++)
                {
                    _lights[i].gameObject.SetActive(true);
                    _lights[i].intensity = 0.5f;
                }
                for (int i = 0; i < _postLampsLights.Count; i++)
                {
                    _postLampsLights[i].SetActive(true);
                }

                if (_lastIndirectLightCoroutine != null)
                {
                    StopCoroutine(_lastIndirectLightCoroutine);
                }
                _lastIndirectLightCoroutine = StartCoroutine(ChangeIndirectLightIntensityCoroutine(0.1f));
            }
            #endregion
        }
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
    private IEnumerator ChangeSunLightIntensityCoroutine(float intensity)
    {
        float currentIntensity = _sunLight.intensity;

        if (currentIntensity > intensity)
        {
            while (currentIntensity > intensity)
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, intensity, 0.1f * Time.deltaTime);
                _sunLight.intensity = currentIntensity;

                yield return null;
            }
        }
        else
        {
            while (currentIntensity < intensity)
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, intensity, 0.1f * Time.deltaTime);
                _sunLight.intensity = currentIntensity;
                yield return null;
            }
        }
    }

    private void EndDay()
    {
        _dayManager.ActivateDayFinishedScoreBoard();
    }
}
