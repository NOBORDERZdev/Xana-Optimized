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
        ReferrencesForGameplay.instance.hiddenButtonDisable();
        BuilderEventManager.UIToggle?.Invoke(true);
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(true);
        otherButton.SetActive(false);
        ReferrencesForGameplay.instance.hiddenButtonEnable();
        BuilderEventManager.UIToggle?.Invoke(false);
    }

    

    public void HideButtonsForFreeCam(bool b)
    {
        otherButton.GetComponent<Button>().interactable = !b;
        if(GetComponent<Button>())
            GetComponent<Button>().interactable = !b;

        BuilderEventManager.UIToggle?.Invoke(b);

        if (b)
        {
            // Getting Btn status
            if (ReferrencesForGameplay.instance.JoyStick.GetComponent<CanvasGroup>().alpha > 0)
                ReferrencesForGameplay.instance.isHidebtn = true;
            else
                ReferrencesForGameplay.instance.isHidebtn = false;

           
            //ClickHidebtnOn();
            ReferrencesForGameplay.instance.hiddenButtonDisable();
            ReferrencesForGameplay.instance.JoyStick.SetActive(true);
            ReferrencesForGameplay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            if (ReferrencesForGameplay.instance.isHidebtn) {
                //ClickHidebtnOff();
                ReferrencesForGameplay.instance.hiddenButtonEnable();
                ReferrencesForGameplay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
}
