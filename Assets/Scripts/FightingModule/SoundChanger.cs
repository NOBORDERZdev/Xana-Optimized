///Attizaz, using this script for overriding audio source clip on runtime
///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChanger : MonoBehaviour
{
    public GameObject WinnerAvatar;
 
    // Start is called before the first frame update
    void Start()
    {
        FightingGameManager.instance.PlayCrowdSound();
        print("On enable changing sound to crowd cheering sound");
    }

    private void OnDisable()
    {
        FightingGameManager.instance.PlayMenuMusic();
        print("On disable changing sound to menu sound");
    }

}