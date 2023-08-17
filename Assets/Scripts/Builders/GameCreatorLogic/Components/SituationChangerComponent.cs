using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class SituationChangerComponent : ItemComponent
{
    [SerializeField]
    private SituationChangerComponentData situationChangerComponentData;
    private bool isActivated = false;
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
    }

    Coroutine situationCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;

            if (situationChangerComponentData.Timer == 0 && !situationChangerComponentData.isOff)
                return;

            if (situationCo == null && situationChangerComponentData.Timer > 0)
                situationCo = StartCoroutine(nameof(SituationChange));

            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);
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

}