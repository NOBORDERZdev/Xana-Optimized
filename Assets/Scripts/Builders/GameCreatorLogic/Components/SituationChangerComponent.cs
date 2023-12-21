using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;
using System;
using ExitGames.Client.Photon.StructWrapping;

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
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
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
    }

    #region BehaviourControl
    private void StartComponent()
    {
        float timeDiff = 0;
        if (playerObject != null)
        {
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
            var hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("situationChangerComponent", DateTime.UtcNow.ToString());
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("situationChangerComponent", out object situationChangerComponent))
            {
                string situationChangerComponentstr = situationChangerComponent.ToString();
                DateTime dateTimeRPC = Convert.ToDateTime(situationChangerComponentstr).ToUniversalTime(); ;
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = dateTimeRPC - currentDateTime;

                timeDiff = (diff.Minutes * 60) + diff.Seconds;
                time = timeDiff;

                if ((time == 0 || time > situationChangerComponentData.Timer) && !situationChangerComponentData.isOff)
                    return;
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

        TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);

    }
    private void StopComponent()
    {
        TimeStats._intensityChangerStop?.Invoke();
    }

    public override void StopBehaviour()
    {
        if(isPlaying)
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