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
        if (PlayerTriggerChecker)
        {
            PlayerTriggerChecker.LiveVideoSoundEnabler += EnableLiveVideoSound;
        }
    }

    private void OnDisable()
    {
        if (PlayerTriggerChecker)
        {
            PlayerTriggerChecker.LiveVideoSoundEnabler -= EnableLiveVideoSound;
        }
        
    }

    public void EnableLiveVideoSound(bool _soundEnable)
    {
        DirectionalSoundController.playerCam = ReferencesForGamePlay.instance.m_34player.transform;
        if (_soundEnable)
        {
            DirectionalSoundController.enabled = true;
            DirectionalSoundController.ActiveDirectionalSound();
            DirectionalSoundController.activePlayer.AudioMuted = false;
        }
        else
        {
            if (DirectionalSoundController.volumeCoroutine != null)
            {
                StopCoroutine(DirectionalSoundController.volumeCoroutine);
                DirectionalSoundController.volumeCoroutine = null;
            }
            DirectionalSoundController.enabled = false;
            DirectionalSoundController.activePlayer.AudioMuted = true;
        }
    }
}
