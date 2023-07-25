using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeStats : MonoBehaviour
{
    public static Action _timeStart;
    public static Action<float, Action> _timeStop;

    public static Action<bool, Light[], float[], float, GameObject> _intensityChanger;
    public static Action _intensityChangerStop;

    private float m_TotalTime;
    public bool IsElapsedTimeActive;
    public bool IsSituationChangerActive;

    public static bool _stopTimer = false, canRun = false;
    bool isRuninig, isNight;
    int previousSkyID;
    IEnumerator coroutine, dimmerCoroutine;

    public Light[] lights;
    public float[] Intensity;

    private void OnEnable()
    {
        _timeStart += StartTimer;
        _timeStop += StopTimer;
        _intensityChanger += SituationStarter;
        _intensityChangerStop += SituationStoper;
    }
    private void OnDisable()
    {
        _timeStart -= StartTimer;
        _timeStop -= StopTimer;
        _intensityChanger -= SituationStarter;
        _intensityChangerStop -= SituationStoper;

    }

    private void Start()
    {
        _stopTimer = true;
        coroutine = Timer();
    }

    public void StartTimer()
    {
        if (!IsElapsedTimeActive)
        {
            IsElapsedTimeActive = true;
            m_TotalTime = 0;
            _stopTimer = false;
            BuilderEventManager.OnElapseTimeCounterTriggerEnter?.Invoke(m_TotalTime, true);
        }
    }


    public void StopTimer(float time, Action callBack)
    {
        _stopTimer = true;
        IsElapsedTimeActive = false;
        //m_TimeCounterText.transform.GetChild(0).gameObject.SetActive(false);
        BuilderEventManager.OnElapseTimeCounterTriggerEnter?.Invoke(time, false);
        StartCoroutine(StopTimerCallBack(() => { callBack(); }));
        StopCoroutine(coroutine);
    }

    IEnumerator StopTimerCallBack(Action callBack)
    {
        yield return new WaitForSeconds(.2f);
        callBack();
    }
    GameObject _situationChangeObject;
    public void SituationStarter(bool _isOff, Light[] _lights, float[] _intensities, float _value, GameObject _obj)
    {
        
        if (isRuninig)
        {
            if (_situationChangeObject == _obj)
            {
                isNight ^= true;
                if (isNight)
                    SetNightMode();
                else
                    SetDayMode(_lights, _intensities);

                return;
            }
            else
                SituationStoper();
        }
        lights = _lights;
        Intensity = _intensities;
        dimmerCoroutine = DimLights(_isOff, _lights, _intensities, _value, _obj);
        canRun = true;
        isRuninig = true;
        this._situationChangeObject = _obj;
        Debug.LogError("EnableSituationChangerUI  " + _value);
        if (!_isOff)
            BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(_value);
        StartCoroutine(dimmerCoroutine);
    }

    public void SituationStoper()
    {
        canRun = false;
        BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);

        if (dimmerCoroutine != null)
        { StopCoroutine(dimmerCoroutine); }
    }


    IEnumerator Timer()
    {
        while (!_stopTimer)
        {
            m_TotalTime += Time.deltaTime;
            yield return null;
        }
    }
    public void BackToNormalSituation()
    {
        if (lights.Length != 0)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = Intensity[i];
            }
        }
    }

    IEnumerator DimLights(bool _isOff, Light[] _light, float[] _lightsIntensity, float timeCheck, GameObject _obj)
    {
        if (_isOff)
        {
            isNight ^= true;
            if (isNight)
                SetNightMode();
            else
                SetDayMode(_light, _lightsIntensity);
        }
        else
        {
            _stopTimer = true;
            isNight ^= true;
            if (isNight)
                SetNightMode();
            else
                SetDayMode(_light, _lightsIntensity);

            while (!timeCheck.Equals(0) && canRun)
            {
                SetTimer(ref timeCheck);
                yield return null;

            }
            isNight = false;
            isRuninig = false;
            SituationChangerComponent scc = _obj.GetComponent<SituationChangerComponent>();
            Destroy(scc);
            SetDayMode(_light, _lightsIntensity);
        }
    }

    public void StopDimLights()
    {
        canRun = false;
    }

    private void SetTimer(ref float timeCheck)
    {
        timeCheck -= Time.deltaTime;
        timeCheck = Mathf.Clamp(timeCheck, 0, Mathf.Infinity);
    }
    private void SetNightMode()
    {
        //EventManager.ChangeSituationToNight?.Invoke(true);
        previousSkyID = SituationChangerSkyboxScript.instance.builderMapDownload.levelData.skyProperties.skyId;
        SituationChangerSkyboxScript.instance.ChangeSkyBox(20);
    }
    private void SetDayMode(Light[] _light, float[] _lightsIntensity)
    {
        //EventManager.ChangeSituationToNight?.Invoke(false);
        for (int i = 0; i < _light.Length; i++)
        {
            _light[i].intensity = _lightsIntensity[i];
        }

        SituationChangerSkyboxScript.instance.ChangeSkyBox(previousSkyID);
    }
}