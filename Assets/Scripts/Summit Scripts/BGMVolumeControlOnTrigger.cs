using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMVolumeControlOnTrigger : MonoBehaviour
{
    public float bgmMinVolume;
    public float bgmMaxVolume;

    private void Start()
    {
        SetBGMAudioOnTrigger(bgmMaxVolume);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                SetBGMAudioOnTrigger(bgmMinVolume);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                SetBGMAudioOnTrigger(bgmMaxVolume);
            }
            
        }
    }

    void SetBGMAudioOnTrigger(float _volume)
    {
        try
        {
            if (SoundController.Instance != null)
            {
                SoundController.Instance.EffectsSource.volume = _volume;
            }
        }
        catch (Exception e)
        {

        }
    }
}
