using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJScreenSound : MonoBehaviour
{
    [SerializeField] AudioSource VideoAudioSource;

    private void Awake()
    {
        SoundController.Instance.videoPlayerSource = VideoAudioSource;
        SoundSettings.soundManagerSettings.videoSource = VideoAudioSource;
        VideoAudioSource.volume = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            Invoke(nameof(SetLobbyBg),1f);
            //SoundSettings.soundManagerSettings.SetBgmVolume(0.05f);
        }
    }


    void SetLobbyBg(){ 
        SoundSettings.soundManagerSettings.SetBgmVolume(0.05f);
    }

}
