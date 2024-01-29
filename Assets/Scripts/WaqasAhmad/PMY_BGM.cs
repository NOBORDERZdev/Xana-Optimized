using PMY;
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
    public float actualVolume;
    private bool isMusicPlaying = true;

    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += SetBgm;
    }
    private void OnDisable()
    {
        SceneManage.onExitAction -= OnSceneExit;
        PMY_Nft_Manager.Instance.exitClickedAction -= UpdateMusicStatus;
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction -= OnVideoEnlargeAction;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= HookEvent;
        BuilderEventManager.AfterPlayerInstantiated -= SetBgm;
    }

    private void SetBgm()
    {
        Debug.LogError("SetBgm");
        // Set Referece for Slider to control controller
        SoundManager.Instance.MusicSource.clip = bgmAudioSource;
        SoundManager.Instance.MusicSource.Play();
        SoundManager.Instance.MusicSource.loop = true;

        // Get Current Parameters of Music Source
        if (soundType.Equals(SoundType.ThreeD))
        {
            isLoopable = SoundManager.Instance.MusicSource.loop;
            currentSpatialBlend = SoundManager.Instance.MusicSource.spatialBlend;
            currentMinDistance = SoundManager.Instance.MusicSource.minDistance;

            //Update Music Source Parameters
            SoundManager.Instance.MusicSource.gameObject.transform.position = new Vector3(0.2212251f, 0.6412843f, 30f);
            SoundManager.Instance.MusicSource.spatialBlend = 1;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
               SoundManager.Instance.MusicSource.minDistance = 20;
            else
                SoundManager.Instance.MusicSource.minDistance = 10;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            SoundManager.Instance.MusicSource.outputAudioMixerGroup = null;
    }

    private void Start()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += HookEvent;
        UpdateMusicVolume();
    }

    private void UpdateMusicVolume()
    {
        Debug.LogError("Start");
        actualVolume = SoundManager.Instance.MusicSource.volume;
        if (Application.platform == RuntimePlatform.Android)
            SoundManagerSettings.soundManagerSettings.SetBgmVolume(0.5f);
        //SoundManager.Instance.MusicSource.volume = 0.5f;
        else if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            SoundManagerSettings.soundManagerSettings.SetBgmVolume(1f);
            SoundManager.Instance.MusicSource.volume = 1.0f;
        }
    }

    private void HookEvent()
    {
        SceneManage.onExitAction += OnSceneExit;                               // invoke when scene is changed
        PMY_Nft_Manager.Instance.exitClickedAction += UpdateMusicStatus;       // invoke when nft video is closed
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction += OnVideoEnlargeAction; // invoke when nft video is enlarged
    }

    private void OnSceneExit()
    {
        if (soundType.Equals(SoundType.ThreeD))
        {
            // Reset Parameters of Music Source
            SoundManager.Instance.MusicSource.loop = isLoopable;
            SoundManager.Instance.MusicSource.spatialBlend = currentSpatialBlend;
            SoundManager.Instance.MusicSource.minDistance = currentMinDistance;
        }
        SoundManager.Instance.MusicSource.volume = actualVolume;
    }

    private void OnVideoEnlargeAction()
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
