using System.Collections;
using Models;
using System;
using Photon.Pun;
using UnityEngine;

public class BlindComponent : ItemComponent
{
    private BlindComponentData blindComponentData;
    private bool blindToggle;
    public Light[] _light;
    public float[] _lightsIntensity;

    public int previousSkyID;
    public int skyBoxID = 21;
    private float againTouchDealy = .5f;
    private bool IsAgainTouchable = true;

    float time;
    string RuntimeItemID = "";
    bool isRunning = false;
    GameObject playerObject;
    Coroutine dimLightsCoroutine;

    bool _collideWithComponent;
    void GetLightsData()
    {
        _light = FindObjectsOfType<Light>();
        _lightsIntensity = new float[_light.Length];
        for (int i = 0; i < _light.Length; i++)
        {
            _lightsIntensity[i] = _light[i].intensity;
        }
    }

    public void Init(BlindComponentData _blindComponentData)
    {
        this.blindComponentData = _blindComponentData;
        blindToggle = _blindComponentData.isOff;
        RuntimeItemID = this.gameObject.GetComponent<XanaItem>().itemData.RuntimeItemID;
        StartCoroutine(SituationChangerSkyboxScript.instance.DownloadBlindComponentSkyboxes());
        GetLightsData();
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            playerObject = _other.gameObject;
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            //if (GamificationComponentData.instance.withMultiplayer)
            //{
            //    if (!blindToggle && !isRunning)
            //    {
            //        UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
            //        utccounterValue.UTCTime = DateTime.UtcNow.ToString();
            //        utccounterValue.CounterValue = blindComponentData.time + 1;
            //        var hash = new ExitGames.Client.Photon.Hashtable();
            //        hash["blindComponent"] = JsonUtility.ToJson(utccounterValue);
            //        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            //    }
            //    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
            //}
            //else
            StartComponent();
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

    IEnumerator DimLights(bool _isOff, Light[] _light, float[] _lightsIntensity, float timeCheck, float _Radius, int _skyBoxID = 20)
    {
        while (!timeCheck.Equals(0))
        {
            timeCheck -= Time.deltaTime;
            timeCheck = Mathf.Clamp(timeCheck, 0, Mathf.Infinity);
            BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke((int)timeCheck);
            yield return null;
        }

        Stop();

    }




    public void Play()
    {
        if (isRunning) return;
        isRunning = true;
        if (dimLightsCoroutine != null)
        {
            StopCoroutine(dimLightsCoroutine);
            dimLightsCoroutine = null;
        }

        dimLightsCoroutine = StartCoroutine(DimLights(blindToggle, _light, _lightsIntensity, time, blindComponentData.radius, skyBoxID));
    }
    public void Stop()
    {
        if (dimLightsCoroutine != null)
        {
            StopCoroutine(dimLightsCoroutine);
            dimLightsCoroutine = null;
        }

        for (int i = 0; i < _light.Length; i++)
        {
            _light[i].intensity = _lightsIntensity[i];
        }
        BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);
        SituationChangerSkyboxScript.instance.ChangeSkyBox(GamificationComponentData.instance.previousSkyID);
        TimeStats.playerCanvas.ToggleBlindLight(false, 300);
        GamificationComponentData.instance.isBlindToogle = false;
        isRunning = false;

    }

    private void ToggleStatus(bool _Status, float _Radius, int _skyBoxID)
    {
        if (_Status)
        {
            SituationChangerSkyboxScript.instance.ChangeSkyBox(_skyBoxID);
            TimeStats.playerCanvas.ToggleBlindLight(true, _Radius);
            return;
        }

        for (int i = 0; i < _light.Length; i++)
        {
            if (_light[i] != null)
                _light[i].intensity = _lightsIntensity[i];
        }

        BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);
        SituationChangerSkyboxScript.instance.ChangeSkyBox(GamificationComponentData.instance.previousSkyID);
        TimeStats.playerCanvas.ToggleBlindLight(false, 300);
    }

    #region BehaviourControl
    private void StartComponent()
    {
        if (_collideWithComponent)
            return;
        _collideWithComponent = true;
        Invoke(nameof(CollideWithComponet), 0.5f);
        GetLightsData();
        if (TimeStats.playerCanvas.transform.parent != GamificationComponentData.instance.nameCanvas.transform)
        {
            TimeStats.playerCanvas.transform.SetParent(GamificationComponentData.instance.nameCanvas.transform);
            TimeStats.playerCanvas.transform.localPosition = Vector3.up * 18.5f;

        }
        TimeStats.playerCanvas.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;


        float timeDiff = 0;
        time = blindComponentData.time + 1;
        if (playerObject != null)
        {
            ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
        }
        //else
        //{
        //    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("blindComponent", out object blindComponentObj) && !blindToggle && GamificationComponentData.instance.withMultiplayer)
        //    {
        //        UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
        //        utccounterValue = JsonUtility.FromJson<UTCTimeCounterValue>(blindComponentObj.ToString());
        //        DateTime dateTimeRPC = DateTime.Parse(utccounterValue.UTCTime);
        //        DateTime currentDateTime = DateTime.UtcNow;
        //        TimeSpan diff = currentDateTime - dateTimeRPC;
        //        timeDiff = (diff.Minutes * 60) + diff.Seconds;
        //        if (timeDiff >= 0 && timeDiff < utccounterValue.CounterValue + 1)
        //            utccounterValue.CounterValue = utccounterValue.CounterValue - timeDiff;
        //        else
        //            return;
        //        time = utccounterValue.CounterValue;
        //    }
        //}

        //if (time == 0 && !blindToggle)
        //{
        //    time = blindComponentData.time;
        //    blindComponentCo = null;
        //}

        //if (blindComponentCo == null && time > 0)
        //    blindComponentCo = StartCoroutine(nameof(BlindComponentStart));
        //TimeStats._blindComponentStart?.Invoke(blindToggle, _light, _lightsIntensity, time, blindComponentData.radius, this.gameObject, skyBoxID);


        //New code
        if (!isRunning)
        {
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
        }

        if (blindToggle)// when toggle is off then no timer
        {
            // set dark mode
            if (!GamificationComponentData.instance.isBlindToogle)
            {
                GamificationComponentData.instance.isBlindToogle = true;
                ToggleStatus(true, blindComponentData.radius, skyBoxID);
                return;
            }
            else // set light mode
            {
                GamificationComponentData.instance.isBlindToogle = false;
                ToggleStatus(false, blindComponentData.radius, skyBoxID);
                return;
            }
        }
        else
        {
            // set dark mode
            if (!GamificationComponentData.instance.isBlindToogle)
            {
                GamificationComponentData.instance.isBlindToogle = true;
                ToggleStatus(true, blindComponentData.radius, skyBoxID);
                Play();
                return;
            }
            else // set light mode
            {
                GamificationComponentData.instance.isBlindToogle = false;
                ToggleStatus(false, blindComponentData.radius, skyBoxID);
                if (!isRunning) Play();
                return;
            }
        }

    }

    void CollideWithComponet()
    {
        _collideWithComponent = false;
    }

    private void StopComponent()
    {
        //TimeStats._blindComponentStop?.Invoke();
        //isRunning = false;
        GetLightsData();
        ToggleStatus(false, blindComponentData.radius, GamificationComponentData.instance.previousSkyID);
        BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);
        isRunning = false;
        if (dimLightsCoroutine != null)
        {
            StopCoroutine(dimLightsCoroutine);
            dimLightsCoroutine = null;
        }
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
        _componentType = Constants.ItemComponentType.BlindComponent;
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