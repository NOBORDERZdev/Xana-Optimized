using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowMovementWhileDancing : MonoBehaviour
{
    public void MoveWhileDace()
    {
        if(ReferencesForGamePlay.instance.moveWhileDanceCheck== 0) // stop dance
        {
            PlayerPrefs.SetInt("dancebutton", 1);
            ReferencesForGamePlay.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            ReferencesForGamePlay.instance.landscapeMoveWhileDancingButton.SetActive(true);
            ReferencesForGamePlay.instance.portraitMoveWhileDancingButton.SetActive(true);
        }
        else //start dance
        {
            PlayerPrefs.SetInt("dancebutton", 0);
            ReferencesForGamePlay.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            //ReferencesForGamePlay.instance.moveWhileDanceCheck = 1;
            ReferencesForGamePlay.instance.landscapeMoveWhileDancingButton.SetActive(false);
            ReferencesForGamePlay.instance.portraitMoveWhileDancingButton.SetActive(false);
        }
    }
}
