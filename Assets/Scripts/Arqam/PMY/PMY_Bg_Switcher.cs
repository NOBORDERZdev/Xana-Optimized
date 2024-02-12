using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PMY_Bg_Switcher : MonoBehaviour
{
    public Sprite pmyBg;

    private void Start()
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
            GetComponent<Image>().sprite = pmyBg;
    }

}
