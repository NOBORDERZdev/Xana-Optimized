using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMVolumeControlOnTrigger : MonoBehaviour
{

    private void Start()
    {
        SetBGMAudioOnTrigger(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                SetBGMAudioOnTrigger(0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                SetBGMAudioOnTrigger(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
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
