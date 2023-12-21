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
        BuilderEventManager.UIToggle?.Invoke(true);
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(true);
        otherButton.SetActive(false);
        ReferrencesForDynamicMuseum.instance.hiddenButtonEnable();
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
            if (ReferrencesForDynamicMuseum.instance.JoyStick.GetComponent<CanvasGroup>().alpha > 0)
                ReferrencesForDynamicMuseum.instance.isHidebtn = true;
            else
                ReferrencesForDynamicMuseum.instance.isHidebtn = false;

           
            //ClickHidebtnOn();
            ReferrencesForDynamicMuseum.instance.hiddenButtonDisable();
            ReferrencesForDynamicMuseum.instance.JoyStick.SetActive(true);
            ReferrencesForDynamicMuseum.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            if (ReferrencesForDynamicMuseum.instance.isHidebtn) {
                //ClickHidebtnOff();
                ReferrencesForDynamicMuseum.instance.hiddenButtonEnable();
                ReferrencesForDynamicMuseum.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
}
