using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMY_BGM : MonoBehaviour
{
    public AudioClip bgmAudioSource;

    private bool isLoopable = false;
    private float currentSpatialBlend = 0;
    public float currentMinDistance = 0;

    private void Awake()
    {
        // Set Referece for Slider to control controller
        SoundManager.Instance.MusicSource.clip = bgmAudioSource;
        SoundManager.Instance.MusicSource.Play();

        // Get Current Parameters of Music Source
        isLoopable =  SoundManager.Instance.MusicSource.loop;
        currentSpatialBlend = SoundManager.Instance.MusicSource.spatialBlend;
        currentMinDistance = SoundManager.Instance.MusicSource.minDistance;

        //Update Music Source Parameters
        SoundManager.Instance.MusicSource.loop = true;
        SoundManager.Instance.MusicSource.gameObject.transform.position = new Vector3(0.2212251f, 0.6412843f, 30f);
        SoundManager.Instance.MusicSource.spatialBlend = 1;
        SoundManager.Instance.MusicSource.minDistance = 20;
    }

    private void OnDisable()
    {
        // Reset Parameters of Music Source
        SoundManager.Instance.MusicSource.loop = isLoopable;
        SoundManager.Instance.MusicSource.spatialBlend = currentSpatialBlend;
        SoundManager.Instance.MusicSource.minDistance = currentMinDistance;
    }

}
