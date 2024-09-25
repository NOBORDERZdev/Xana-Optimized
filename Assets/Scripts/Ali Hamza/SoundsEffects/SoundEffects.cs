using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public enum Sounds { JumpSound, PortalSound, ActionSound, AddForce, Collectible, CountDown, DisplayMessage, DoorOpen, InfoPopup, Invisible, LightOff, Narration, Ninja, QuizWrong, QuizCorrect, RandomNumber, SpecialItem}
    public Sounds CurrentSound;

    public AudioSource ref_audio;

    public AudioClip jumpClip;
    public AudioClip portalClip;
    public AudioClip actionClip;
    public AudioClip AddForceClip;
    public AudioClip CollectibleClip;
    public AudioClip CountDownClip;
    public AudioClip DisplayMessageClip;
    public AudioClip DoorOpenClip;
    public AudioClip InfoPopupClip;
    public AudioClip InvisibleClip;
    public AudioClip LightOffClip;
    public AudioClip NarrationClip;
    public AudioClip NinjaClip;
    public AudioClip QuizWrongClip;
    public AudioClip QuizCorrectClip;
    public AudioClip RandomNumberClip;
    public AudioClip SpecialItemClip;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void PlaySoundEffects(Sounds soundType)
    {
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            return;
        }

        if (ref_audio.isPlaying)
            return;
        switch (soundType)
        {
            case Sounds.JumpSound:
                ref_audio.PlayOneShot(jumpClip);
                break;
            case Sounds.PortalSound:
                ref_audio.PlayOneShot(portalClip);
                break;
            case Sounds.ActionSound:
                ref_audio.PlayOneShot(actionClip);
                break;
            case Sounds.AddForce:
                ref_audio.PlayOneShot(AddForceClip);
                break;
            case Sounds.Collectible:
                ref_audio.PlayOneShot(CollectibleClip);
                break;
            case Sounds.CountDown:
                ref_audio.PlayOneShot(CountDownClip);
                break;
            case Sounds.DisplayMessage:
                ref_audio.PlayOneShot(DisplayMessageClip);
                break;
            case Sounds.DoorOpen:
                ref_audio.PlayOneShot(DoorOpenClip);
                break;
            case Sounds.InfoPopup:
                ref_audio.PlayOneShot(InfoPopupClip);
                break;
            case Sounds.Invisible:
                ref_audio.PlayOneShot(InvisibleClip);
                break;
            case Sounds.LightOff:
                ref_audio.PlayOneShot(LightOffClip);
                break;
            case Sounds.Narration:
                ref_audio.PlayOneShot(NarrationClip);
                break;
            case Sounds.Ninja:
                ref_audio.PlayOneShot(NinjaClip);
                break;
            case Sounds.QuizWrong:
                ref_audio.PlayOneShot(QuizWrongClip);
                break;
            case Sounds.QuizCorrect:
                ref_audio.PlayOneShot(QuizCorrectClip);
                break;
            case Sounds.RandomNumber:
                ref_audio.PlayOneShot(RandomNumberClip);
                break;
            case Sounds.SpecialItem:
                ref_audio.PlayOneShot(SpecialItemClip);
                break;

        }
    }
}