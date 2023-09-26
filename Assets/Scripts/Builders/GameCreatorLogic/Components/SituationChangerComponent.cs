using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class SituationChangerComponent : ItemComponent
{
    [SerializeField]
    private SituationChangerComponentData situationChangerComponentData;
    string RuntimeItemID = "";
    internal bool isActivated = false;
    public Light[] _light;
    public float[] _lightsIntensity;
    private bool IsAgainTouchable = true;

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
    }

    Coroutine situationCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            if (!isActivated)
                return;
            GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
        }
    }

    IEnumerator SituationChange()
    {
        while (situationChangerComponentData.Timer > 0)
        {
            situationChangerComponentData.Timer--;
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
        if (situationChangerComponentData.Timer == 0 && !situationChangerComponentData.isOff)
            return;

        if (situationCo == null && situationChangerComponentData.Timer > 0)
            situationCo = StartCoroutine(nameof(SituationChange));

        TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);

    }
    private void StopComponent()
    {
        TimeStats._intensityChangerStop?.Invoke();
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

}