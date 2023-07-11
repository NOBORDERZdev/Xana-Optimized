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
        
    }


}
