using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowMovementWhileDancing : MonoBehaviour
{
    public void MoveWhileDace()
    {
        if(ReferrencesForDynamicMuseum.instance.moveWhileDanceCheck== 0) // stop dance
        {
            PlayerPrefs.SetInt("dancebutton", 1);
            ReferrencesForDynamicMuseum.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            ReferrencesForDynamicMuseum.instance.landscapeMoveWhileDancingButton.SetActive(true);
            ReferrencesForDynamicMuseum.instance.portraitMoveWhileDancingButton.SetActive(true);
        }
        else //start dance
        {
            PlayerPrefs.SetInt("dancebutton", 0);
            ReferrencesForDynamicMuseum.instance.moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton");
            //ReferrencesForDynamicMuseum.instance.moveWhileDanceCheck = 1;
            ReferrencesForDynamicMuseum.instance.landscapeMoveWhileDancingButton.SetActive(false);
            ReferrencesForDynamicMuseum.instance.portraitMoveWhileDancingButton.SetActive(false);
        }
    }
}
