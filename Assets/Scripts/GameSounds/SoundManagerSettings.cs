using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using RenderHeads.Media.AVProVideo;
using System.Collections;
public class SoundManagerSettings : MonoBehaviour
{
    public static SoundManagerSettings soundManagerSettings;
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
    void Awake()
    {
        if (soundManagerSettings == null)
        {
            soundManagerSettings = this;
        }
        if (SoundManager.Instance)
        {

            bgmSource = SoundManager.Instance.MusicSource;
            effectsSource = SoundManager.Instance.EffectsSource;
            videoSource = SoundManager.Instance.videoPlayerSource;
            StartCoroutine(LiveVideoInstancs(5f));
        }
        //To Clamp AudioSource Volume Slider Range from 0 to 0.7 Rather Than 0 to 1 Range
        OldSliderRange = (OldSliderMax - OldSliderMin);
        NewSliderRange = (NewSliderMax - NewSliderMin);

        
        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby")){
            PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.015f);
            PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME,  0.015f);
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.015f);
        }
        else{
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, 0.5f);
            PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.5f);
            PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f);
        }
       // PlayerPrefs.SetFloat(ConstantsGod.BGM_VOLUME, 0.5f);
        //PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, 0.5f);
        PlayerPrefs.SetFloat(ConstantsGod.CAMERA_SENSITIVITY, 0.72f);
    }
    IEnumerator LiveVideoInstancs(float value)
    {
        yield return new WaitForSeconds(value);
        liveVideoSource = SoundManager.Instance.livePlayerSource;
    }
    private void OnEnable()
    {

        Invoke("AddingDeley", 0.25f);
        ChangeOrientation_waqas.switchOrientation += OnOrientationChanged;
        //  Invoke("ObjectsDeley", 1f);
    }
    void OnOrientationChanged()
    {
        Debug.LogError("orientation is changed ......"+ PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
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
        if (videoSource == null)
            videoSource = SoundManager.Instance.videoPlayerSource;
        YoutubeStreamController Videoplayer = GameObject.FindObjectOfType<YoutubeStreamController>();
        if (Videoplayer != null)
        {
            videoSource = Videoplayer.videoPlayerAudioSource;
            liveVideoSource = Videoplayer.LiveStreamPlayer.GetComponent<MediaPlayer>();
            Debug.Log("VideoSource Set ");
        }
        else
        {
            Debug.Log("VideoSource not Set ");
        }
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
        Debug.Log("check orientation===" + ChangeOrientation_waqas._instance.isPotrait);
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, volume);
            SetMicVolume(totalVolumeSliderPotrait.value);
            SetEffectsVolume(totalVolumeSliderPotrait.value);
            bgmSliderPotariat.value = videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
            SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        }
        else
        {
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, volume);
            SetMicVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            SetEffectsVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            bgmSlider.value = videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
            SetBgmVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
            SetVideoVolume(PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        }
    }
    public void SetBgmVolume(float Vol)
    {
        if (!liveVideoSource)
        {
            liveVideoSource = SoundManager.Instance.livePlayerSource;
        }
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
    }
    public void SetVideoVolume(float Vol)
    {
        Debug.Log("check orientation===" + ChangeOrientation_waqas._instance.isPotrait);
       
            PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, Vol);
           // PlayerPrefs.SetFloat(ConstantsGod.VIDEO_VOLUME, Vol);
            videoSliderPotriat.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);
            videoSlider.value = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);

            Debug.Log("LiveVideo" + liveVideoSource);
            if (videoSource)
            {
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
                CameraLook.instance.lookSpeed = sensitivity;
                CameraLook.instance.lookSpeedd = sensitivity;
            }
            else
            {
                CameraLook.instance.lookSpeed = cameraSensitivitySliderPotrait.value;
                CameraLook.instance.lookSpeedd = cameraSensitivitySliderPotrait.value;
            }
        
       
        
    }
    public void SetEffectsVolume(float Vol)
    {
        Debug.Log("Volume effect===" + Vol);
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
    public void SetMicVolume(float vol)
    {
        //PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, vol);
        Debug.Log("Volume SetMicVolume===" + vol);
        Debug.Log("Volume SetMicVolume 1 ===" + PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME));
        //if(totalVolumeSlider.value >= vol)
        //{
        //foreach (var gameobject in Launcher.instance.playerobjects)
        //{
        //    if (!gameobject.GetComponent<PhotonView>().IsMine)
        //    {
        //        gameobject.GetComponent<AudioSource>().volume = vol;
        //    }
        //}
        //}
        //else
        //{
        //        foreach (var gameobject in Launcher.instance.playerobjects)
        //        {
        //            if (!gameobject.GetComponent<PhotonView>().IsMine)
        //            gameobject.GetComponent<AudioSource>().volume = totalVolumeSlider.value;
        //        }
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
        ChangeOrientation_waqas.switchOrientation -= OnOrientationChanged;
    }
}