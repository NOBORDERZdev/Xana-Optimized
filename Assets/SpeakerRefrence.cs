using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerRefrence : MonoBehaviour
{
    public static SpeakerRefrence instance;
    public GameObject RangeVolSpeaker;
    public GameObject NormalVolSpeaker;

    private void Awake()
    {
        instance = this;
    }

   

}
