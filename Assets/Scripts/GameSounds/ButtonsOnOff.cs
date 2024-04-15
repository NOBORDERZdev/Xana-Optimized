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
        ReferencesForGamePlay.instance.hiddenButtonDisable();
        BuilderEventManager.UIToggle?.Invoke(true);
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(true);
        otherButton.SetActive(false);
        ReferencesForGamePlay.instance.hiddenButtonEnable();
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
            if (ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha > 0)
                ReferencesForGamePlay.instance.isHidebtn = true;
            else
                ReferencesForGamePlay.instance.isHidebtn = false;

           
            //ClickHidebtnOn();
            ReferencesForGamePlay.instance.hiddenButtonDisable();
            ReferencesForGamePlay.instance.JoyStick.SetActive(true);
            ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            if (ReferencesForGamePlay.instance.isHidebtn) {
                //ClickHidebtnOff();
                ReferencesForGamePlay.instance.hiddenButtonEnable();
                ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
}
