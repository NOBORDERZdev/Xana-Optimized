using Photon.Realtime;
using PMY;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AvProDirectionalSound : MonoBehaviour
{
    public GameObject parent;
    [Tooltip("Enable/Disable Screen sound toggle in screen panel")]
    public bool isShowScreenSoundOption = true;
    [Space(5)]
    public float maxDistance = 10f; // Max distance for full volume
    public float minDistance = 2f; // Min distance for minimum volume
    [Space(5)]
    [Range(0.2f, 0.5f)]
    public float updateInterval = 0.5f; // Time interval for volume update

    private Transform playerCam; // Reference to your player object or camera
    private WaitForSeconds updateDelay;
    private Coroutine volumeCoroutine;
    private bool isScreenSoundPlaying = true;
    public MediaPlayer activePlayer;
    public AudioSource[] sources;

    private void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += HookEvent;
        ChangeOrientation_waqas.switchOrientation += OrientationChanged;
    }

    private void OnDisable()
    {
        ChangeOrientation_waqas.switchOrientation -= OrientationChanged;
        PMY_Nft_Manager.Instance.exitClickedAction -= UpdateScreenMusicStatus;
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction -= OnVideoEnlargeAction;
        ScreenSoundOnOff.ScreenSoundStatus -= ToggleScreenSound;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= HookEvent;
    }

    private void OrientationChanged()
    {
        StartCoroutine(ShowScreenSoundBtnInSettingPanel());
    }

    private void Start()
    {
        updateDelay = new WaitForSeconds(updateInterval);
        playerCam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        GetActivePlayer();
    }

    private void GetActivePlayer()
    {

        if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            minDistance = 50;                          // lound the live video volume for ios
            foreach (AudioSource source in sources)
            {
                source.volume = 1f;
                //source.minDistance = 50;
            }
        }

        if (!XanaConstants.xanaConstants.isScreenSoundOn)
        {
            if(activePlayer)
            activePlayer.AudioMuted = true;
            foreach (AudioSource source in sources)
                source.mute = true;
        }


        if (activePlayer.gameObject.activeSelf)
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
    }

    private void HookEvent()
    {
        PMY_Nft_Manager.Instance.exitClickedAction += UpdateScreenMusicStatus;
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction += OnVideoEnlargeAction;
        ScreenSoundOnOff.ScreenSoundStatus += ToggleScreenSound;

        if (isShowScreenSoundOption)
            StartCoroutine(ShowScreenSoundBtnInSettingPanel());
    }

    IEnumerator ShowScreenSoundBtnInSettingPanel()
    {
        yield return updateDelay;
        ButtonsPressController.Instance.screenSoundBtn.SetActive(true);
    }

    IEnumerator AdjustScreenVolume()
    {
        while (true)
        {
            if (!activePlayer.gameObject.activeSelf)
            {
                StopCoroutine(volumeCoroutine);
                yield return null;
            }
            // Calculate the distance between player/camera and video source
            float distance = Vector3.Distance(playerCam.position, transform.position);

            // Clamp the distance within the range
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Map the distance to the volume level (adjust this mapping based on your needs)
            float mappedVolume = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);

            // Set the video volume using the third-party package's method
            activePlayer.AudioVolume = mappedVolume;
            yield return updateDelay;
        }
    }

    private void ToggleScreenSound(bool isSoundOn)
    {
        activePlayer.AudioMuted = isSoundOn;
        XanaConstants.xanaConstants.isScreenSoundOn = !isSoundOn;

        foreach (AudioSource source in sources)
            source.mute = isSoundOn;

        if (activePlayer.gameObject.activeSelf) {
            if (isSoundOn)
                volumeCoroutine = StartCoroutine(AdjustScreenVolume());
            else if (volumeCoroutine != null)
                StopCoroutine(volumeCoroutine);
        }
    }

    private void OnVideoEnlargeAction()
    {
        isScreenSoundPlaying = false;
        activePlayer.AudioMuted = true;
        foreach (AudioSource source in sources)
            source.mute = true;
    }

    private void UpdateScreenMusicStatus(int nftNum)   // when user click on exit button of nft
    {
        if (isScreenSoundPlaying) return;
        activePlayer.AudioMuted = false;
        isScreenSoundPlaying = true;
        foreach (AudioSource source in sources)
            source.mute = false;
    }

    public void EnableDisableLiveVideoPlayer()
    {
        StartCoroutine(SwitchOnOffPlayer());
    }

    private IEnumerator SwitchOnOffPlayer()
    {
        activePlayer.enabled = false;
        AudioOutput output = activePlayer.GetComponent<AudioOutput>();
        output.enabled = false;
        AudioSource ac = activePlayer.GetComponent<AudioSource>();
        ac.enabled = false;
        yield return new WaitForSeconds(0.5f);
        activePlayer.enabled = true;
        output.enabled = true;
        ac.enabled = true;
    }


}
