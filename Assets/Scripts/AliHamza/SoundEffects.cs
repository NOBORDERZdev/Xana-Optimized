using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public enum Sounds { JumpSound, PortalSound, ActionSound}
    public Sounds CurrentSound;
    public AudioSource ref_audio;
    public AudioClip jumpClip;
    public AudioClip portalClip;
    public AudioClip actionClip;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void PlaySoundEffects(Sounds soundType)
    {
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
        }
    }
}