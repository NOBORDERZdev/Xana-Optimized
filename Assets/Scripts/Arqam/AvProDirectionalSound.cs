using PMY;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AvProDirectionalSound : MonoBehaviour, IScreenSoundControl
{
    [Tooltip("Enable/Disable Screen sound toggle in screen panel")]
    public bool isShowScreenSoundOption = true;
    [Space(5)]
    public MediaPlayer mediaPlayer;
    [Space(5)]
    public float maxDistance = 10f; // Max distance for full volume
    public float minDistance = 2f; // Min distance for minimum volume
    [Space(5)]
    [Range(0.2f, 0.5f)]
    public float updateInterval = 0.5f; // Time interval for volume update

    private Transform playerCam; // Reference to your player object or camera
    private WaitForSeconds updateDelay;
    private Coroutine volumeCoroutine;
    private GameObject screenSoundBtnPort, screenSoundBtnLand;
    private bool isScreenSoundPlaying = true;

    private void OnEnable()
    {
        ChangeOrientation_waqas.switchOrientation += OrientationChanged;
        SceneManage.onExitAction += OnSceneExit;

        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += HookEvent;
    }

    private void OnDisable()
    {
        ChangeOrientation_waqas.switchOrientation -= OrientationChanged;
        SceneManage.onExitAction -= OnSceneExit;
        PMY_Nft_Manager.Instance.exitClickedAction -= UpdateScreenMusicStatus;
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction -= OnVideoEnlargeAction;
    }

    private void OrientationChanged()
    {
        ShowScreenSoundBtnInSettingPanel(); // StartCoroutine(ShowScreenSoundBtnInSettingPanel());
    }

    private void Start()
    {

        if (!XanaConstants.xanaConstants.isScreenSoundOn)
            mediaPlayer.AudioMuted = false;

        playerCam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        if (isShowScreenSoundOption)
            ShowScreenSoundBtnInSettingPanel(); // StartCoroutine(ShowScreenSoundBtnInSettingPanel());

        updateDelay = new WaitForSeconds(updateInterval);
        volumeCoroutine = StartCoroutine(AdjustScreenVolume());
    }

    private void HookEvent()
    {
        PMY_Nft_Manager.Instance.exitClickedAction += UpdateScreenMusicStatus;
        PMY_Nft_Manager.Instance.OnVideoEnlargeAction += OnVideoEnlargeAction;
    }

    private void ShowScreenSoundBtnInSettingPanel()
    {
        //yield return updateDelay;
        // set the interface reference to SoundOnOff script
        ButtonsPressController.Instance.gameObject.GetComponent<XanaFeaturesHandler>().
                screenSoundToggle.GetComponent<ScreenSoundOnOff>().SetScreenSoundControl = this;
        //UnityEngine.Debug.LogError("Errorrrr::" + ButtonsPressController.Instance.gameObject.GetComponent<XanaFeaturesHandler>().
        //        screenSoundToggle.GetComponent<ScreenSoundOnOff>().gameObject.name);

        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            if (screenSoundBtnPort is null)
                screenSoundBtnPort = ButtonsPressController.Instance.gameObject.GetComponent<XanaFeaturesHandler>().screenSoundToggle;
            screenSoundBtnPort.SetActive(true);
        }
        else
        {
            Handheld.Vibrate();
            if (screenSoundBtnLand is null)
                screenSoundBtnLand = ButtonsPressController.Instance.gameObject.GetComponent<XanaFeaturesHandler>().screenSoundToggle;
            screenSoundBtnLand.SetActive(true);
        }
    }

    IEnumerator AdjustScreenVolume()
    {
        while (true)
        {
            // Calculate the distance between player/camera and video source
            float distance = Vector3.Distance(playerCam.position, transform.position);

            // Clamp the distance within the range
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Map the distance to the volume level (adjust this mapping based on your needs)
            float mappedVolume = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
            //Debug.Log("<color=red> Distance: " + distance + "</color>");  
            //Debug.Log("<color=red> Volume: " + mappedVolume + "</color>");

            // Set the video volume using the third-party package's method
            mediaPlayer.AudioVolume = mappedVolume;

            yield return updateDelay;
        }
    }

    private void OnSceneExit()
    {
        screenSoundBtnPort?.SetActive(false);
        screenSoundBtnLand?.SetActive(false);
    }

    public void ToggleScreenSound(bool isSoundOn)
    {
        mediaPlayer.AudioMuted = isSoundOn;
        XanaConstants.xanaConstants.isScreenSoundOn = !isSoundOn;
        if (isSoundOn)
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
        else if (volumeCoroutine != null)
            StopCoroutine(volumeCoroutine);
    }

    private void OnVideoEnlargeAction()
    {
        isScreenSoundPlaying = false;
        mediaPlayer.AudioMuted = true;
    }

    private void UpdateScreenMusicStatus(int nftNum)
    {
        if (isScreenSoundPlaying) return;
        mediaPlayer.AudioMuted = false;
        isScreenSoundPlaying = true;
    }

}
