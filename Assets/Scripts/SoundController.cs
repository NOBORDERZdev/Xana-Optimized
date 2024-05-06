using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;

public class SoundController : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;
    public AudioSource videoPlayerSource;
    public MediaPlayer livePlayerSource;
    public AudioSource npcDialogueSource;

    public AudioMixer gameSounds;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundController Instance = null;

    public VideoPlayer videoPlayer1, videoPlayer2;
    public VideoClip videoClip;
    public AudioClip bgm_music;
    public int micSound;
    public GameObject SoundManagerPotrait;



    void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
        }
        //else if (Instance != this)
        //{
        //    Destroy(gameObject);
        //}

        //Assign MusicSource our BGM_Clip if there is any assigned from inspector

    }

    private void OnEnable()
    {
        if (Instance != this)
            Instance = this;
    }


    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        if (SoundManagerPotrait) 
        {
            if (SoundManagerPotrait.activeInHierarchy)
            {
                SoundManagerPotrait.GetComponent<SoundSettings>().enabled = true;
            } 
        }
    }

    public void Play(AudioClip clip)
    {
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    public void StopMic()
    {
        PlayerPrefs.SetInt("micSound", 0);
        micSound = PlayerPrefs.GetInt("micSound");
    }

    public void PlayMic()
    {
        PlayerPrefs.SetInt("micSound", 1);
        micSound = PlayerPrefs.GetInt("micSound");
    }

    public void PlayNPCAudio(AudioClip clip)
    {
        ChangeMusicVolume(-10f);
        npcDialogueSource.clip = clip;
        npcDialogueSource.Play();
    }

    void ChangeMusicVolume(float _volume)
    {
        Debug.Log("Valoume aa gae====" + _volume);
        gameSounds.DOSetFloat("MusicVolume", _volume, 0.5f);
    }

    public void StopNPCAudio()
    {
        ChangeMusicVolume(0f);
        npcDialogueSource.Stop();
    }

    public void PlayBGM()
    {
        if (MusicSource != null)
            MusicSource.Play();
    }


    public void PlayMusic(AudioClip clip)
    {
        if (videoPlayerSource)
            videoPlayerSource.Stop();
        MusicSource.mute = false;
        MusicSource.clip = clip;
        MusicSource.Play();
    }


    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

    public void SetBGMusic(AudioClip _bgm)
    {
        bgm_music = _bgm;
        MusicSource.clip = bgm_music;
    }

    public void VideoAudioSourceChangeAGROOM()
    {

    }


    public void VolumeMethodForVideo()
    {

    }

}