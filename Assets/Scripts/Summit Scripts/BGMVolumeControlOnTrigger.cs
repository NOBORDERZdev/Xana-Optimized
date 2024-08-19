using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMVolumeControlOnTrigger : MonoBehaviour
{
    public AdvancedYoutubePlayer VideoPlayerController;
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
                if (VideoPlayerController.VideoPlayer.isPlaying)
                {
                    SetBGMAudioOnTrigger(true);
                }
                else if (VideoPlayerController.AVProVideoPlayer.Control != null && VideoPlayerController.AVProVideoPlayer.Control.IsPlaying())
                {
                    SetBGMAudioOnTrigger(true);
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
