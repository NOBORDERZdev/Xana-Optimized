using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;
using System;
using TMPro;

public class SituationChangerComponent : ItemComponent
{
    [SerializeField]
    private SituationChangerComponentData situationChangerComponentData;
    string RuntimeItemID = "";
    internal bool isActivated = false;
    public Light[] _light;
    public float[] _lightsIntensity;
    private bool IsAgainTouchable = true;
    float time, defaultTimer;
    GameObject playerObject;

    public float timeCheck = 0f;

    bool running = false;
    private float againTouchDealy = .5f;

    bool _collideWithComponent;

    private void OnEnable()
    {
        BuilderEventManager.DisableSituationLight += ResetSituation;
    }
    private void OnDisable()
    {
        BuilderEventManager.DisableSituationLight -= ResetSituation;
    }

    private void GetLightData()
    {
        _light = FindObjectsOfType<Light>();
        _lightsIntensity = new float[_light.Length];
        for (int i = 0; i < _light.Length; i++)
        {
            _lightsIntensity[i] = _light[i].intensity;
        }
    }

    public void Init(SituationChangerComponentData situationChangerComponentData)
    {
        this.situationChangerComponentData = situationChangerComponentData;
        isActivated = true;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
        defaultTimer = this.situationChangerComponentData.Timer;
        GetLightData();
        StartCoroutine(SituationChangerSkyboxScript.instance.DownloadSituatioChangerSkyboxes());
    }

    Coroutine situationCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            //if (GamificationComponentData.instance.withMultiplayer)
            //{
            //    if (!situationChangerComponentData.isOff && !isRuninig)
            //    {
            //        UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
            //        utccounterValue.UTCTime = DateTime.UtcNow.ToString();
            //        utccounterValue.CounterValue = defaultTimer;
            //        BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);
            //        var hash = new ExitGames.Client.Photon.Hashtable();
            //        hash["situationChangerComponent"] = JsonUtility.ToJson(utccounterValue);
            //        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            //    }
            //    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
            //}
            //else
            PlayBehaviour();
        }
    }

    IEnumerator SituationChange()
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        IsAgainTouchable = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        IsAgainTouchable = true;
        playerObject = null;
    }

    #region BehaviourControl
    private void StartComponent()
    {

        if (_collideWithComponent)
            return;
        _collideWithComponent = true;
        Invoke(nameof(CollideWithComponet), 0.5f);
        GetLightData();
        if (!isRuninig)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
        }
        float timeDiff = 0;
        time = defaultTimer;
        if (playerObject != null)
        {
            ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
        }
        //else
        //{
        //    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("situationChangerComponent", out object situationChangerComponent) && !situationChangerComponentData.isOff)
        //    {
        //        UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
        //        utccounterValue = JsonUtility.FromJson<UTCTimeCounterValue>(situationChangerComponent.ToString());
        //        DateTime dateTimeRPC = DateTime.Parse(utccounterValue.UTCTime);
        //        DateTime currentDateTime = DateTime.UtcNow;
        //        TimeSpan diff = currentDateTime - dateTimeRPC;
        //        timeDiff = (diff.Minutes * 60) + diff.Seconds;

        //        BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);

        //        if (timeDiff >= 0 && timeDiff < utccounterValue.CounterValue + 1)
        //            utccounterValue.CounterValue = utccounterValue.CounterValue - timeDiff;
        //        else
        //            return;
        //        time = utccounterValue.CounterValue;
        //    }
        //}

        //if (time == 0 && !situationChangerComponentData.isOff)
        //{
        //    time = situationChangerComponentData.Timer;
        //    situationCo = null;
        //}
        //if (time != 0)
        //{
        //    situationChangerComponentData.Timer = time;
        //    time = 0;
        //}
        //else
        //    situationChangerComponentData.Timer = defaultTimer;

        //if (situationCo == null && time > 0)
        //    situationCo = StartCoroutine(nameof(SituationChange));

        //Debug.LogError(situationChangerComponentData.Timer);
        //TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);
        timeCheck = time;
        SituationStarter(this.situationChangerComponentData.isOff, _light, _lightsIntensity, timeCheck, this.gameObject);

    }

    void CollideWithComponet()
    {
        _collideWithComponent = false;
    }

    private void StopComponent()
    {
        // when time completed then component is removed from the item so we dont put here the code
        // this will work only with situation changer that is timer
        StopDimLights();
    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.SituationChangerComponent;
    }

    #endregion

    #region Timer
    private float m_TotalTime;

    public static bool _stopTimer = false, canRun = false;

    private bool isRuninig;

    private bool isShowUI;


    IEnumerator coroutine, dimmerCoroutine;

    private TimeSpan m_SecondsInToTimeFormate;

    private const string m_TimeFormat = @"mm\:ss";

    public void SituationStarter(bool _isOff, Light[] _lights, float[] _intensities, float _value, GameObject _obj)
    {
        isShowUI = true;
        if (isRuninig)
        {
            GamificationComponentData.instance.isNight ^= true;
            if (GamificationComponentData.instance.isNight)
                SetNightMode();
            else
                SetDayMode(_lights, _intensities);

            return;
        }

        dimmerCoroutine = DimLights(_isOff, _lights, _intensities, _value, _obj);
        isRuninig = true;
        canRun = true;
        StartCoroutine(dimmerCoroutine);
    }

    IEnumerator DimLights(bool _isOff, Light[] _light, float[] _lightsIntensity, float timeCheck, GameObject _obj)
    {
        if (_isOff)
        {
            GamificationComponentData.instance.isNight ^= true;
            if (GamificationComponentData.instance.isNight)
                SetNightMode();
            else
                SetDayMode(_light, _lightsIntensity);
            BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);
        }
        else
        {
            _stopTimer = true;

            GamificationComponentData.instance.isNight ^= true;
            if (GamificationComponentData.instance.isNight)
                SetNightMode();
            else
                SetDayMode(_light, _lightsIntensity);


            //If we don't want to reset the time for continuous collision them remove "this."
            while (!this.timeCheck.Equals(0) && canRun)
            {
                SetTimer(ref this.timeCheck);
                yield return null;
            }

            BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);
            GamificationComponentData.instance.isNight = false;
            isRuninig = false;
            SetDayMode(_light, _lightsIntensity);
        }
    }

    IEnumerator Timer()
    {
        canRun = false;

        while (!_stopTimer)
        {
            Debug.Log("Elapsed time");
            m_TotalTime += Time.deltaTime;
            BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke((int)m_TotalTime);

            yield return null;
        }

        // m_TimeCounterText_.text = "";
    }

    public void StopDimLights()
    {
        print("Stop Situation");
        isShowUI = false;
        BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);

        GetLightData();
        if (dimmerCoroutine != null)
        {
            //StopCoroutine(dimmerCoroutine);
            //dimmerCoroutine = null;
            //isRuninig = false;
            //canRun = false;
            //GamificationComponentData.instance.isNight = false;
            if (GamificationComponentData.instance.isBlindToogle)
                GamificationComponentData.instance.isBlindToogle = false;
            SetDayMode(_light, _lightsIntensity);

        }

    }

    private void SetTimer(ref float timeCheck)
    {
        timeCheck -= Time.deltaTime;
        if (!isShowUI) return;
        timeCheck = Mathf.Clamp(timeCheck, 0, Mathf.Infinity);
        TimeSpan sp = TimeSpan.FromSeconds(timeCheck);
        BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke((int)timeCheck);
    }
    private void SetNightMode()
    {
        SituationChangerSkyboxScript.instance.ChangeSkyBox(20);
    }
    private void SetDayMode(Light[] _light, float[] _lightsIntensity)
    {
        for (int i = 0; i < _light.Length; i++)
        {
            if (_light[i] != null)
                _light[i].intensity = _lightsIntensity[i];
        }

        SituationChangerSkyboxScript.instance.ChangeSkyBox(GamificationComponentData.instance.previousSkyID);
        isShowUI = false;
        BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);

        // the following lines are used when the timer is reset on day mode, if we don't want the timer to stop then remove the following lines 
        if (dimmerCoroutine != null)
        {
            StopCoroutine(dimmerCoroutine);
            dimmerCoroutine = null;
            isRuninig = false;
            canRun = false;
        }
    }
    private void ResetSituation()
    {
        timeCheck = situationChangerComponentData.Timer;
        SetDayMode(_light, _lightsIntensity);
        GamificationComponentData.instance.isNight = false;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }
    #endregion

}