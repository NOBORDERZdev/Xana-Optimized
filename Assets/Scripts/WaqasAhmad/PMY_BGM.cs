using PMY;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMY_BGM : MonoBehaviour
{
    public enum SoundType { TwoD, ThreeD }
    public SoundType soundType = SoundType.TwoD;
    [Space(5)]
    public AudioClip bgmAudioSource;

    private bool isLoopable = false;
    private float currentSpatialBlend = 0;
    private float currentMinDistance = 0;
    public bool isMusicPlaying = true;

    private void Awake()
    {
        // Set Referece for Slider to control controller
        SoundManager.Instance.MusicSource.clip = bgmAudioSource;
        SoundManager.Instance.MusicSource.Play();

        // Get Current Parameters of Music Source
        if (soundType.Equals(SoundType.ThreeD))
        {
            isLoopable = SoundManager.Instance.MusicSource.loop;
            currentSpatialBlend = SoundManager.Instance.MusicSource.spatialBlend;
            currentMinDistance = SoundManager.Instance.MusicSource.minDistance;

            //Update Music Source Parameters
            SoundManager.Instance.MusicSource.loop = true;
            SoundManager.Instance.MusicSource.gameObject.transform.position = new Vector3(0.2212251f, 0.6412843f, 30f);
            SoundManager.Instance.MusicSource.spatialBlend = 1;
            SoundManager.Instance.MusicSource.minDistance = 20;
        }
    }

    private void OnEnable()
    {
        if (soundType.Equals(SoundType.ThreeD))
            SceneManage.onExitAction += OnSceneExit;
    }

    private void OnDisable()
    {
        if (soundType.Equals(SoundType.ThreeD))
            SceneManage.onExitAction -= OnSceneExit;
        PMY_Nft_Manager.Instance.exitClickedAction -= UpdateMusicStatus;
    }

    private void Start()
    {
        PMY_Nft_Manager.Instance.exitClickedAction += UpdateMusicStatus;
    }

    private void OnSceneExit()
    {
        // Reset Parameters of Music Source
        SoundManager.Instance.MusicSource.loop = isLoopable;
        SoundManager.Instance.MusicSource.spatialBlend = currentSpatialBlend;
        SoundManager.Instance.MusicSource.minDistance = currentMinDistance;
    }

    public void OnVideoEnlargeAction()
    {
        isMusicPlaying = false;
        SoundManager.Instance.MusicSource.mute = true; 
    }

    private void UpdateMusicStatus(int nftNum)
    {
        if (isMusicPlaying) return;
        SoundManager.Instance.MusicSource.mute = false;
        isMusicPlaying = true;
    }

}
