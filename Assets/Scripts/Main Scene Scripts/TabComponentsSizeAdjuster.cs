using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabComponentsSizeAdjuster : MonoBehaviour
{
    public float width;
    public float height;
   


    public void Start()
    {
        if (ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector3(width, height);
        }
       
    }
}
