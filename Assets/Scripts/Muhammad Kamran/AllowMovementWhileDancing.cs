using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowMovementWhileDancing : MonoBehaviour
{
    public void MoveWhileDace()
    {
        if(ReferrencesForGameplay.instance.moveWhileDanceCheck== 0) // stop dance
        {
            PlayerPrefs.SetInt("dancebutton", 1);
            ReferrencesForGameplay.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            ReferrencesForGameplay.instance.landscapeMoveWhileDancingButton.SetActive(true);
            ReferrencesForGameplay.instance.portraitMoveWhileDancingButton.SetActive(true);
        }
        else //start dance
        {
            PlayerPrefs.SetInt("dancebutton", 0);
            ReferrencesForGameplay.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            //ReferrencesForGameplay.instance.moveWhileDanceCheck = 1;
            ReferrencesForGameplay.instance.landscapeMoveWhileDancingButton.SetActive(false);
            ReferrencesForGameplay.instance.portraitMoveWhileDancingButton.SetActive(false);
        }
    }
}
