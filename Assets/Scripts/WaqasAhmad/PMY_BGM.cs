using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMY_BGM : MonoBehaviour
{
    public AudioClip bgmAudioSource;

    private void Awake()
    {
        // Set Referece for Slider to control controller
        SoundManager.Instance.MusicSource.clip = bgmAudioSource;
        SoundManager.Instance.MusicSource.Play();
    }


   
}
