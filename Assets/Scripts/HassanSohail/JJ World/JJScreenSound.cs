using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJScreenSound : MonoBehaviour
{
    [SerializeField] AudioSource VideoAudioSource;

    private void Awake()
    {
        SoundManager.Instance.videoPlayerSource = VideoAudioSource;
        SoundManagerSettings.soundManagerSettings.videoSource = VideoAudioSource;
        VideoAudioSource.volume = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            Invoke(nameof(SetLobbyBg),1f);
            //SoundManagerSettings.soundManagerSettings.SetBgmVolume(0.05f);
        }
    }


    void SetLobbyBg(){ 
        SoundManagerSettings.soundManagerSettings.SetBgmVolume(0.05f);
    }

}
