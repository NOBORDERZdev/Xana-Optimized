using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotraitButtonsOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myOtherButton;
    public void ClickHidebtnOn()
    {
       myOtherButton.SetActive(true);
       this.gameObject.SetActive(false);
       ReferrencesForGameplay.instance.potraithiddenButtonDisable();
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(false);
        myOtherButton.SetActive(true);
        ReferrencesForGameplay.instance.potraithiddenButtonEnable();
       
    }
}
