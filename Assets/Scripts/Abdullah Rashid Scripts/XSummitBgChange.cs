using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XSummitBgChange : MonoBehaviour
{
    public Sprite XSummit;
    public Sprite XanaSummit;
    public GameObject BackgroundImg;
    private void Awake()
    {
        if (ConstantsHolder.xanaConstants.XSummitBG)
        {

            BackgroundImg.GetComponent<Image>().sprite = XSummit;

        }
        else {

            BackgroundImg.GetComponent<Image>().sprite = XanaSummit;
        }
    }
}
