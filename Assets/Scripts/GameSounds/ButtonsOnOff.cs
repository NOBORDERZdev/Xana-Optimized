using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    public void ClickHidebtnOn()
    {
       otherButton.SetActive(true);
       this.gameObject.SetActive(false);
        ReferrencesForDynamicMuseum.instance.hiddenButtonDisable();
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(true);
        otherButton.SetActive(false);
        ReferrencesForDynamicMuseum.instance.hiddenButtonEnable();
       
    }

    public void HideButtonsForFreeCam(bool b)
    {

        if (b)
        {
            otherButton.GetComponent<Button>().interactable = false;
            ClickHidebtnOn();
            ReferrencesForDynamicMuseum.instance.JoyStick.SetActive(true);
            ReferrencesForDynamicMuseum.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            ClickHidebtnOff();
            otherButton.GetComponent<Button>().interactable = true;
            ReferrencesForDynamicMuseum.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;

        }
    }
}
