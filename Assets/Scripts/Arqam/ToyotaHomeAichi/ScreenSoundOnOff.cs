using System.Collections;
using System.Collections.Generic;
using Toyota;
using UnityEngine;

public class ScreenSoundOnOff : MonoBehaviour
{
    public AR_Nft_Manager AR_nft;
    public enum VideoSoundStatus { On, Off };
    [Space(5)]
    public VideoSoundStatus status;
    [SerializeField] RenderHeads.Media.AVProVideo.MediaPlayer livePlayer;
    [SerializeField] AudioSource preRecoredPlayer;

    private void Start()
    {
        AR_nft.OnVideoEnlargeAction += MuteSound;
        AR_nft.exitClickedAction += UnMuteSound;
    }

    private void OnDisable()
    {
        AR_nft.OnVideoEnlargeAction -= MuteSound;
        AR_nft.exitClickedAction -= UnMuteSound;
    }

    private void MuteSound()
    {
        status = VideoSoundStatus.Off;
        livePlayer.AudioMuted = true;
        preRecoredPlayer.mute = true;
    }

    private void UnMuteSound(int nftIndex)
    {
        status = VideoSoundStatus.On;
        livePlayer.AudioMuted = false;
        preRecoredPlayer.mute = false;
    }

}
