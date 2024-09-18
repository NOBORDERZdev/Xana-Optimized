using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BGMVolumeControlOnTrigger : MonoBehaviour
{
    public AdvancedYoutubePlayer VideoPlayerController;
    public AudioSource PrePrecordered;
    public bool IsPlayerCollided = false;

    private void Start()
    {
        if (gameObject.GetComponent<AdvancedYoutubePlayer>())
        {
            VideoPlayerController = gameObject.GetComponent<AdvancedYoutubePlayer>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                IsPlayerCollided = true;
                SetBGMAudioOnTrigger(true);
                if (PrePrecordered && PrePrecordered.isActiveAndEnabled)
                {
                    SoundSettings.soundManagerSettings.videoSource = PrePrecordered;
                    //if(IsPlayerCollided)
                    // SoundSettings.soundManagerSettings.SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
                    SoundSettings.soundManagerSettings.SetAudioSourceSliderVal(PrePrecordered, PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));

                    PrePrecordered.mute = false;
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>())
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                IsPlayerCollided = false;
                SetBGMAudioOnTrigger(false);
                if (PrePrecordered && PrePrecordered.isActiveAndEnabled)
                {
                    SoundSettings.soundManagerSettings.videoSource = null;
                    PrePrecordered.mute = true;
                }
            }
        }
    }


    public void SetBGMAudioOnTrigger(bool _mute)
    {
        if (SoundController.Instance != null)
        {
            SoundController.Instance.EffectsSource.mute = _mute;
        }
    }
}
