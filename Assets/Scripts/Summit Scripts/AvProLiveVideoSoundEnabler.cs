using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvProLiveVideoSoundEnabler : MonoBehaviour
{
    public SPAAIHandler PlayerTriggerChecker;
    public AvProDirectionalSound DirectionalSoundController;

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
}
