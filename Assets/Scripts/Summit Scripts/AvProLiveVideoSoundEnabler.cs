using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvProLiveVideoSoundEnabler : MonoBehaviour
{
    public SPAAIHandler PlayerTriggerChecker;
    public AvProDirectionalSound DirectionalSoundController;
    public GameObject PreRecVideoScreen;
    public MeshFilter LiveVideoPlayerScreen;
    public Mesh AndroidLiveVideoMesh;
    public Mesh IOSLiveVideoMesh;

    private void OnEnable()
    {
        PlayerTriggerChecker.LiveVideoSoundEnabler += EnableLiveVideoSound;
    }

    private void OnDisable()
    {
        PlayerTriggerChecker.LiveVideoSoundEnabler -= EnableLiveVideoSound;
    }

    void EnableLiveVideoSound(bool _soundEnable)
    {
        if (_soundEnable)
        {
            DirectionalSoundController.enabled = true;
            DirectionalSoundController.ActiveDirectionalSound();
        }
        else
        {
            if (DirectionalSoundController.volumeCoroutine != null)
            {
                StopCoroutine(DirectionalSoundController.volumeCoroutine);
            }
            DirectionalSoundController.enabled = false;
        }
    }

    public void EnableVideoScreen(bool _isLiveVideo)
    {
        if (_isLiveVideo)
        {

#if UNITY_ANDROID
            LiveVideoPlayerScreen.mesh = AndroidLiveVideoMesh;
#elif UNITY_IOS
            LiveVideoPlayerScreen.mesh = IOSLiveVideoMesh;
#endif
            LiveVideoPlayerScreen.gameObject.SetActive(_isLiveVideo);
            PreRecVideoScreen.SetActive(!_isLiveVideo);
        }
        else
        {
            PreRecVideoScreen.SetActive(!_isLiveVideo);
            LiveVideoPlayerScreen.gameObject.SetActive(_isLiveVideo);
        }
    }
}
