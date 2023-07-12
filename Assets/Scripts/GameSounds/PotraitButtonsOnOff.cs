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
       ReferrencesForDynamicMuseum.instance.potraithiddenButtonDisable();
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(false);
        myOtherButton.SetActive(true);
        ReferrencesForDynamicMuseum.instance.potraithiddenButtonEnable();
       
    }
}
