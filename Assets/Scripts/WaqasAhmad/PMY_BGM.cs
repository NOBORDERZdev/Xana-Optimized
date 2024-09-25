using PMY;
using UnityEngine;

public class PMY_BGM : MonoBehaviour
{
    public enum SoundType { TwoD, ThreeD }
    public SoundType soundType = SoundType.TwoD;
    [Space(5)]
    public AudioClip bgmAudioSource;
    public AudioSource MusicSource;

    private bool isLoopable = false;
    private float currentSpatialBlend = 0;
    private float currentMinDistance = 0;
    public float actualVolume;
    private bool isMusicPlaying = true;

    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += SetBgm;
    }
    private void OnDisable()
    {
        //SceneManage.onExitAction -= OnSceneExit;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= HookEvent;
        BuilderEventManager.AfterPlayerInstantiated -= SetBgm;
        PlayerPrefs.SetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME, MusicSource.volume);

        if (PMY_Nft_Manager.Instance)
        {
            PMY_Nft_Manager.Instance.exitClickedAction -= UpdateMusicStatus;
            PMY_Nft_Manager.Instance.OnVideoEnlargeAction -= OnVideoEnlargeAction;
        }
    }

    private void SetBgm()
    {
        // Set Referece for Slider to control controller
        MusicSource.clip = bgmAudioSource;
        MusicSource.Play();
        MusicSource.loop = true;
        MusicSource.volume = PlayerPrefs.GetFloat(ConstantsGod.TOTAL_AUDIO_VOLUME);

        // Get Current Parameters of Music Source
        if (soundType.Equals(SoundType.ThreeD))
        {
            isLoopable = MusicSource.loop;
            currentSpatialBlend = MusicSource.spatialBlend;
            currentMinDistance = MusicSource.minDistance;

            //Update Music Source Parameters 
            MusicSource.gameObject.transform.localPosition = new Vector3(12.48055f, 36.14407f, 122.2209f);  //0.2212251f, 0.6412843f, 30f
            MusicSource.spatialBlend = 1;
            MusicSource.rolloffMode = AudioRolloffMode.Linear;
            MusicSource.maxDistance = 55;
            //if (Application.platform == RuntimePlatform.IPhonePlayer)
            //    MusicSource.volume = 1;
        }
        else
        {
            LoadFromFile.instance.PlayerCamera.m_Lens.NearClipPlane = 0.1f;  //0.01
        }
    }

    private void Start()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += HookEvent;
        UpdateMusicVolume();
    }

    private void UpdateMusicVolume()
    {
        MusicSource.volume = SoundManagerSettings.soundManagerSettings.totalVolumeSlider.value;
        SoundManagerSettings.soundManagerSettings.totalVolumeSlider.onValueChanged.AddListener((float vol) =>
        {
            SetBgmVolume(vol);
        });
        SoundManagerSettings.soundManagerSettings.totalVolumeSliderPotrait.onValueChanged.AddListener((float vol) =>
        {
            SetBgmVolume(vol);
        });

        //actualVolume = MusicSource.volume;

        //if (Application.platform == RuntimePlatform.Android)
        //    SoundManagerSettings.soundManagerSettings.SetBgmVolume(0.5f);

        //else if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    SoundManagerSettings.soundManagerSettings.SetBgmVolume(1f);
        //    SoundManager.Instance.MusicSource.volume = 1.0f;
        //}
    }

    private void SetBgmVolume(float vol)
    {
        MusicSource.volume = vol;
    }

    private void HookEvent()
    {
        //SceneManage.onExitAction += OnSceneExit;                               // invoke when scene is changed
        if (PMY_Nft_Manager.Instance != null)
        {
            PMY_Nft_Manager.Instance.exitClickedAction += UpdateMusicStatus;
            PMY_Nft_Manager.Instance.OnVideoEnlargeAction += OnVideoEnlargeAction;
        }
        else
        {
            Debug.LogWarning("PMY_Nft_Manager.Instance is null. Events not hooked.");
        }
    }

    //private void OnSceneExit()
    //{
    //    if (soundType.Equals(SoundType.ThreeD))
    //    {
    //        // Reset Parameters of Music Source
    //        SoundManager.Instance.MusicSource.loop = isLoopable;
    //        SoundManager.Instance.MusicSource.spatialBlend = currentSpatialBlend;
    //        SoundManager.Instance.MusicSource.minDistance = currentMinDistance;
    //    }
    //    SoundManager.Instance.MusicSource.volume = actualVolume;
    //}

    private void OnVideoEnlargeAction()
    {
        isMusicPlaying = false;
        MusicSource.mute = true;
    }

    private void UpdateMusicStatus(int nftNum)
    {
        if (isMusicPlaying) return;
        MusicSource.mute = false;
        isMusicPlaying = true;
    }

}
