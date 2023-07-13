using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class SituationChangerComponent : ItemComponent
{
    [SerializeField]
    private SituationChangerComponentData situationChangerComponentData;

    private bool isActivated = false;

    public Light[] _light;

    public float[] _lightsIntensity;

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
        Debug.Log(JsonUtility.ToJson(situationChangerComponentData));

        this.situationChangerComponentData = situationChangerComponentData;

        isActivated = true;
    }

    private void OnCollisionEnter(Collision _other)
    {

        Debug.Log("Situation changer collision" + _other.gameObject.name);
        if (/*_other.gameObject.tag == "Player" || */(_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            //TimeStats._intensityChangerStop?.Invoke();
            TimeStats._intensityChanger?.Invoke(this.situationChangerComponentData.isOff, _light, _lightsIntensity, situationChangerComponentData.Timer, this.gameObject);
        }
    }

}