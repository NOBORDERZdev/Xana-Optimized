using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;
using System;

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

    // Start is called before the first frame update
    void Start()
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
    }

    Coroutine situationCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            playerObject = _other.gameObject;

            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            if (GamificationComponentData.instance.withMultiplayer)
            {
                if (!situationChangerComponentData.isOff)
                {
                    UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
                    utccounterValue.UTCTime = DateTime.UtcNow.ToString();
                    utccounterValue.CounterValue = defaultTimer;
                    BuilderEventManager.OnSituationChangerTriggerEnter?.Invoke(0);
                    var hash = new ExitGames.Client.Photon.Hashtable();
                    hash["situationChangerComponent"] = JsonUtility.ToJson(utccounterValue);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                }
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
            }
            else
                GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, _componentType);
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
        Start();
        float timeDiff = 0;
        if (playerObject != null)
        {
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("situationChangerComponent", out object situationChangerComponent) && !situationChangerComponentData.isOff)
            {
                UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
                utccounterValue = JsonUtility.FromJson<UTCTimeCounterValue>(situationChangerComponent.ToString());
                DateTime dateTimeRPC = DateTime.Parse(utccounterValue.UTCTime);
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = currentDateTime - dateTimeRPC;
                timeDiff = (diff.Minutes * 60) + diff.Seconds;

                BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);

                if (timeDiff >= 0 && timeDiff < utccounterValue.CounterValue + 1)
                    utccounterValue.CounterValue = utccounterValue.CounterValue - timeDiff;
                else
                    return;
                time = utccounterValue.CounterValue;
            }
        }

        //if (time == 0 && !situationChangerComponentData.isOff)
        //{
        //    time = situationChangerComponentData.Timer;
        //    situationCo = null;
        //}
        if (time != 0)
        {
            situationChangerComponentData.Timer = time;
            time = 0;
        }
        else
            situationChangerComponentData.Timer = defaultTimer;

        //if (situationCo == null && time > 0)
        //    situationCo = StartCoroutine(nameof(SituationChange));

        //Debug.LogError(situationChangerComponentData.Timer);
        TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);

    }
    private void StopComponent()
    {
        //playerObject = null;
        TimeStats._intensityChangerStop?.Invoke();
    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
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

}