using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XSummitBgChange : MonoBehaviour
{
    public Sprite XSummitBackgroung;
    public Sprite XanaSummitBackgroung;
    public GameObject BackGroundSprite;

    void Start()
    {
        
        ChangeBG();
    }
    public void ChangeBG()
    {
        if (ConstantsHolder.xanaConstants.XSummitBg)
        {
            BackGroundSprite.GetComponent<Image>().sprite = XSummitBackgroung;

        }
        else
        {

            BackGroundSprite.GetComponent<Image>().sprite = XanaSummitBackgroung;
        }
       
    }

}
