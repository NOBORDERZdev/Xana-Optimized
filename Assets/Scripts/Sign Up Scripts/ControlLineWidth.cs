using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ControlLineWidth : MonoBehaviour
{
    public RectTransform line1;
    public RectTransform line2;
    // Start is called before the first frame update
    void Start()
    {
        if (ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
        {
            line1.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600);
            line2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
