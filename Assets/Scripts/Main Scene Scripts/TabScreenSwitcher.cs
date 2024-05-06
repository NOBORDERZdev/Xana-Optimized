using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabScreenSwitcher : MonoBehaviour
{
    public GameObject tabBG;
  
    void Start()
    {
       if (ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
        {
            tabBG.SetActive(true);
        }
    }

}
