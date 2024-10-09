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
        
            Invoke("ChangeSummitBG", 0.4f); // Call ChangeBackground after 2 seconds
        
       
    }
    public void ChangeSummitBG()
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
