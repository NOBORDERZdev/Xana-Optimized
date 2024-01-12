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

    GameObject playerObject;

    void Start()
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
    }

    Coroutine blindComponentCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            playerObject = _other.gameObject;
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            if (GamificationComponentData.instance.withMultiplayer)
            {
                if (!blindToggle)
                {
                    UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
                    utccounterValue.UTCTime = DateTime.UtcNow.ToString();
                    utccounterValue.CounterValue = blindComponentCo == null ? blindComponentData.time : time;
                    BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);
                    if (blindComponentCo != null)
                    {
                        StopCoroutine(blindComponentCo);
                        blindComponentCo = null;
                    }
                    var hash = new ExitGames.Client.Photon.Hashtable();
                    hash["blindComponent"] = JsonUtility.ToJson(utccounterValue);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                }
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
            }
            else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, _componentType);
        }
    }

    IEnumerator BlindComponentStart()
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
        }
        blindComponentCo = null;
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
        if (TimeStats.playerCanvas.transform.parent != GamificationComponentData.instance.nameCanvas.transform)
        {
            TimeStats.playerCanvas.transform.SetParent(GamificationComponentData.instance.nameCanvas.transform);
            TimeStats.playerCanvas.transform.localPosition = Vector3.up * 18.5f;

        }
        TimeStats.playerCanvas.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;


        float timeDiff = 0;

        if (playerObject != null)
        {
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("blindComponent", out object blindComponentObj) && !blindToggle)
            {
                UTCTimeCounterValue utccounterValue = new UTCTimeCounterValue();
                utccounterValue = JsonUtility.FromJson<UTCTimeCounterValue>(blindComponentObj.ToString());
                DateTime dateTimeRPC = DateTime.Parse(utccounterValue.UTCTime);
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = currentDateTime - dateTimeRPC;
                timeDiff = (diff.Minutes * 60) + diff.Seconds;

                if (blindComponentCo != null)
                {
                    StopCoroutine(blindComponentCo);
                    blindComponentCo = null;
                }

                BuilderEventManager.OnBlindComponentTriggerEnter?.Invoke(0);

                if (timeDiff >= 0 && timeDiff < utccounterValue.CounterValue + 1)
                    utccounterValue.CounterValue = utccounterValue.CounterValue - timeDiff;
                else
                    return;
                time = utccounterValue.CounterValue;
                //if (time == 0 || time > blindComponentData.time)
                //    return;
            }
        }

        if (time == 0 && !blindToggle)
        {
            time = blindComponentData.time;
            blindComponentCo = null;
        }

        if (blindComponentCo == null && time > 0)
            blindComponentCo = StartCoroutine(nameof(BlindComponentStart));
        TimeStats._blindComponentStart?.Invoke(blindToggle, _light, _lightsIntensity, time, blindComponentData.radius, this.gameObject, skyBoxID);

    }
    private void StopComponent()
    {
        TimeStats._blindComponentStop?.Invoke();
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
        _componentType = Constants.ItemComponentType.BlindComponent;
    }

    #endregion
}