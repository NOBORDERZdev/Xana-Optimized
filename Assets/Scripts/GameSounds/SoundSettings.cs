using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using RenderHeads.Media.AVProVideo;
using System.Collections;

public class SoundSettings : MonoBehaviour
{
    public static SoundSettings soundManagerSettings;
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource effectsSource;
    public AudioSource videoSource;
    public MediaPlayer liveVideoSource;
    [Header("Audio Slider")]
    public Slider totalVolumeSlider;
    public Slider bgmSlider;
    public Slider videoSlider;
    public Slider UserSlider;
    public Slider cameraSensitivitySlider;
    //PotriatSilders
    public Slider totalVolumeSliderPotrait;
    public Slider bgmSliderPotariat;
    public Slider videoSliderPotriat;
    public Slider UserSliderPotrait;
    public Slider cameraSensitivitySliderPotrait;

    public GameObject SoundManagerPotarit;
    public GameObject SoundManagerObject;
    float OldSliderMin = 0f;
    float OldSliderMax = 1f;
    float OldSliderRange;
    float NewSliderMin = 0f;
    float NewSliderMax = 0.7f;
    float NewSliderRange;
    [Space]
    public Button MuteBtnMain;
    public Button unMuteBtnMain;
    [Header("Speakers")]
    private Speaker speaker;
    private Recorder recorder;

    public delegate void BgmAudioEvent(bool _isMuted);
    public BgmAudioEvent OnBGMAudioMuted;

    void Awake()
    {
        if (soundManagerSettings == null)
        {
            soundManagerSettings = this;
        }
        if (SoundController.Instance)
        {

            bgmSource = SoundController.Instance.MusicSource;
            effectsSource = SoundController.Instance.EffectsSource;
            //videoSource = SoundController.Instance.videoPlayerSource;
            StartCoroutine(LiveVideoInstancs(5f));
        }
        //To Clamp AudioSource Volume Slider Range from 0 to 0.7 Rather Than 0 to 1 Range
        OldSliderRange = (OldSliderMax - OldSliderMin);
        NewSliderRange = (NewSliderMax - NewSliderMin);

        
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby")){
            PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.015f);
            PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, 0.015f);
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.015f);
        }
        else{
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.5f);
            PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.5f);
            PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f);
        }
        // PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.5f);
        //PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f);
        //PlayerPrefs.SetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.395f);
        if (PlayerPrefs.GetInt("DefaultSensivity") == 0)
        {
            PlayerPrefs.SetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.75f);
            PlayerPrefs.SetFloat(ConstantsGod.MIC, 0.5f); // Mic Value By Default 0.5f
            PlayerPrefs.SetInt("DefaultSensivity", 1);
        }
        cameraSensitivitySliderPotrait.maxValue = 1f;
    }
    IEnumerator LiveVideoInstancs(float value)
    {
        yield return new WaitForSeconds(value);
        //liveVideoSource = SoundController.Instance.livePlayerSource;
    }
    private void OnEnable()
    {

        Invoke("AddingDeley", 0.25f);
        ScreenOrientationManager.switchOrientation += OnOrientationChanged;
        //  Invoke("ObjectsDeley", 1f);
    }
    void OnOrientationChanged(bool IsPortrait)
    {
        //Debug.Log("orientation is changed ......"+ PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        totalVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        //bgmSlider.value = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        //videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);
        //cameraSensitivitySlider.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);

        totalVolumeSliderPotrait.SetValueWithoutNotify(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        //bgmSliderPotariat.value = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        //videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);
        //cameraSensitivitySliderPotrait.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);

        SetAllVolumes(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        //SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME));
        //SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME));
        SetCameraSensitivity(PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY));
        SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.MIC));
    }


    void AddingDeley()
    {
        //if (videoSource == null)
        //    videoSource = SoundController.Instance.videoPlayerSource;
        //YoutubeStreamController Videoplayer = GameObject.FindObjectOfType<YoutubeStreamController>();
        //YoutubePlayerLivestream Videoplayer2 = GameObject.FindObjectOfType<YoutubePlayerLivestream>();
        //if (Videoplayer != null)
        //{
        //    videoSource = Videoplayer.videoPlayerAudioSource;
        //    liveVideoSource = Videoplayer.LiveStreamPlayer.GetComponent<MediaPlayer>();
        //    //Debug.Log("VideoSource Set ");
        //}
        //else if (Videoplayer2 != null)
        //{
        //    //videoSource = Videoplayer.videoPlayerAudioSource;
        //    liveVideoSource = Videoplayer2.mPlayer;
        //    //Debug.Log("VideoSource Set ");
        //}
        //else
        //{
        //    //Debug.Log("VideoSource not Set ");
        //}
    }


    private void Start()
    {

        totalVolumeSlider.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
        bgmSlider.value = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);
        cameraSensitivitySlider.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);

        totalVolumeSliderPotrait.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
        bgmSliderPotariat.value = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME);
        videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);
        cameraSensitivitySliderPotrait.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);

        SetAllVolumes(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        //SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME));
        //SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME));
        SetCameraSensitivity(PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY));
        SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.MIC));
        //Adding Functions to Sliders through Add Listener
        videoSlider.onValueChanged.AddListener((float vol) =>
        {
            SetVideoVolume(vol);
        });
        UserSlider.onValueChanged.AddListener((float vol) =>
        {
            SetMicVolume(vol);
        });
        cameraSensitivitySlider.onValueChanged.AddListener((float sensitivity) =>
        {
            SetCameraSensitivity(sensitivity);
        });
        totalVolumeSlider.onValueChanged.AddListener((float vol) =>
        {
           
            SetBgmVolume(vol);
            SetVideoVolume(vol);
        });
        bgmSlider.onValueChanged.AddListener((float vol) =>
        {
            SetBgmVolume(vol);
        });

        videoSliderPotriat.onValueChanged.AddListener((float vol) =>
        {
            SetVideoVolume(vol);
        });
        UserSliderPotrait.onValueChanged.AddListener((float vol) =>
        {
            SetMicVolume(vol);
        });
        cameraSensitivitySliderPotrait.onValueChanged.AddListener((float sensitivity) =>
        {
            SetCameraSensitivity(sensitivity);
        });
        totalVolumeSliderPotrait.onValueChanged.AddListener((float vol) =>
        {
            
            SetBgmVolume(vol);
            SetVideoVolume(vol);
        });
        bgmSliderPotariat.onValueChanged.AddListener((float vol) =>
        {
            SetBgmVolume(vol);
        });

        //totalVolumeSlider.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.5f);
        //bgmSlider.value = PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME, 0.5f);
        //videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f);
        //cameraSensitivitySlider.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.72f);
        //SetAllVolumes(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.5f));
        //SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.BGM_VOLUME, 0.5f));
        //SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f));
        //SetCameraSensitivity(PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.72f));
        //SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.MIC, 0.5f));
        ////Adding Functions to Sliders through Add Listener
        //videoSlider.onValueChanged.AddListener((float vol) =>
        //{
        //    SetVideoVolume(vol);
        //});
        //UserSlider.onValueChanged.AddListener((float vol) =>
        //{
        //    SetMicVolume(vol);
        //});
        //cameraSensitivitySlider.onValueChanged.AddListener((float sensitivity) =>
        //{
        //    SetCameraSensitivity(sensitivity);
        //});
        //totalVolumeSlider.onValueChanged.AddListener((float vol) =>
        //{
        //    SetBgmVolume(vol);
        //    SetVideoVolume(vol);
        //});
        //bgmSlider.onValueChanged.AddListener((float vol) =>
        //{
        //    SetBgmVolume(vol);
        //});
       
    }
    public void SetUsersVolume()
    {
        if (SoundManagerPotarit.activeInHierarchy)
        {
            SetMicVolume(UserSliderPotrait.value);
        }
        else
        {
            SetMicVolume(UserSlider.value);
        }

    }
    public void SetAllVolumes(float volume)
    {
        //Debug.Log("check orientation===" + ScreenOrientationManager._instance.isPotrait);
        if (ScreenOrientationManager._instance.isPotrait)
        {
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, volume);
            SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.MIC));
            SetEffectsVolume(totalVolumeSliderPotrait.value);
           // bgmSliderPotariat.value = videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
            SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME));
        }
        else
        {
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, volume);
            SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.MIC));
            SetEffectsVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
           // bgmSlider.value = videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
            SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME));
        }
    }
    public void SetBgmVolume(float Vol)
    {
        //if (!liveVideoSource)
        //{
        //    liveVideoSource = SoundController.Instance.livePlayerSource;
        //}
        PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, Vol);
       // PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, Vol);
        if (bgmSource)
        {
            SetAudioSourceSliderVal(bgmSource, Vol);
            if (liveVideoSource)
            {
                SetAudioSourceSliderValLive(liveVideoSource, Vol);
            }
        }

        BuilderEventManager.BGMVolume?.Invoke(Vol);
        if (Vol <= 0)
        {
            OnBGMAudioMuted?.Invoke(true);
        }
        else
        {
            OnBGMAudioMuted?.Invoke(false);
        }
    }
    public void SetVideoVolume(float Vol)
    {
        //Debug.Log("check orientation===" + ScreenOrientationManager._instance.isPotrait);
        
            PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, Vol);
            // PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, Vol);
            videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);
            videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.VIDEO_VOLUME);

           // //Debug.Log("LiveVideo" + liveVideoSource);
            if (videoSource)
            {
                if (videoSource.GetComponent<MediaPlayer>())
                    SetAudioSourceSliderValLive(videoSource.GetComponent<MediaPlayer>(), Vol);
                else
                    SetAudioSourceSliderVal(videoSource, Vol);
            }
            if (liveVideoSource)
            {
                SetAudioSourceSliderValLive(liveVideoSource, Vol);
            }
     
   }

    public void SetCameraSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(ConstantsGod.CAMERA_SENSITIVITY, sensitivity);
        cameraSensitivitySliderPotrait.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);
        cameraSensitivitySlider.value = PlayerPrefs.GetFloat(ConstantsGod.CAMERA_SENSITIVITY);
  
            if (cameraSensitivitySliderPotrait.value >= sensitivity)
            {
                PlayerCameraController.instance.lookSpeed = sensitivity;
                PlayerCameraController.instance.lookSpeedd = sensitivity;
            }
            else
            {
                PlayerCameraController.instance.lookSpeed = cameraSensitivitySliderPotrait.value;
                PlayerCameraController.instance.lookSpeedd = cameraSensitivitySliderPotrait.value;
            }
        
       
        
    }
    public void SetEffectsVolume(float Vol)
    {
        //Debug.Log("Volume effect===" + Vol);
        PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, Vol);
        if (effectsSource)
        {
            SetAudioSourceSliderVal(effectsSource, Vol);
            if (liveVideoSource)
            {
                SetAudioSourceSliderValLive(liveVideoSource, Vol);
            }
        }
    }
    public void SetMicVolume(float vol) //Opponent Player Mic Volume Setting Using UserSldier&UserSliderPotrait
    {
        PlayerPrefs.SetFloat(ConstantsGod.MIC, vol);
        UserSlider.value = PlayerPrefs.GetFloat(ConstantsGod.MIC);
        UserSliderPotrait.value = PlayerPrefs.GetFloat(ConstantsGod.MIC);

        foreach (var gameobject in MutiplayerController.instance.playerobjects)
        {
            if (gameobject != null && !gameobject.GetComponent<PhotonView>().IsMine) 
            {
                gameobject.GetComponent<SpeakerRefrence>().RangeVolSpeaker.volume = UserSlider.value;
            }
        }

        //else
        //{
        //    foreach (var gameobject in MutiplayerController.instance.playerobjects)
        //    {
        //        if (!gameobject.GetComponent<PhotonView>().IsMine)
        //            gameobject.GetComponent<AudioSource>().volume = totalVolumeSlider.value;
        //    }
        //}
    }
    //Setting AudioSource Volume Slider Range between 0 and 0.7
    public void SetAudioSourceSliderVal(AudioSource _audioSrcRef, float _vol)
    {
        float newClampedSliderValue = (((_vol - OldSliderMin) * NewSliderRange) / OldSliderRange) + NewSliderMin;
        if (totalVolumeSlider.value >= _vol)
        {
            _audioSrcRef.volume = newClampedSliderValue;
        }
        else
        {
            _audioSrcRef.volume = newClampedSliderValue;
        }
    }
    public void SetAudioSourceSliderValLive(MediaPlayer _audioSrcRef, float _vol)
    {
        float newClampedSliderValue = (((_vol - OldSliderMin) * NewSliderRange) / OldSliderRange) + NewSliderMin;
        if (totalVolumeSlider.value >= _vol)
        {
            _audioSrcRef.AudioVolume = newClampedSliderValue;
        }
        else
        {
            _audioSrcRef.AudioVolume = newClampedSliderValue;
        }
    }
    public void setNewSliderValues()
    {
        SetAllVolumes(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.5f));
    }

    private void OnDisable()
    {
        soundManagerSettings = null;
        ScreenOrientationManager.switchOrientation -= OnOrientationChanged;
    }
}