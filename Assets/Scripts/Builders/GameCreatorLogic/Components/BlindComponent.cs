using System.Collections;
using Models;
using System;
using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon.StructWrapping;

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
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
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
        if (PlayerCanvas.Instance.transform.parent != ArrowManager.Instance.nameCanvas.transform)
        {
            PlayerCanvas.Instance.transform.SetParent(ArrowManager.Instance.nameCanvas.transform);
            PlayerCanvas.Instance.transform.localPosition = Vector3.up * 18.5f;

        }
        PlayerCanvas.Instance.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;


        float timeDiff = 0;
        if (playerObject != null)
        {
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.LightOff);
            var hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("blindComponent", DateTime.UtcNow.ToString());
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("blindComponent",out object blindComponent))
            {
                string blindComponentstr = blindComponent.ToString();
                DateTime dateTimeRPC = Convert.ToDateTime(blindComponentstr).ToUniversalTime(); ;
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = dateTimeRPC - currentDateTime;

                timeDiff = (diff.Minutes * 60) + diff.Seconds;
                time = timeDiff;

                if (time == 0 || time > blindComponentData.time)
                    return;
            }
        }

        if (time == 0 && !blindComponentData.isOff)
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
        _componentType = Constants.ItemComponentType.BlindComponent;
    }

    #endregion
}