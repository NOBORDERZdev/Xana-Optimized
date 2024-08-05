using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotraitButtonsOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myOtherButton;
    public void ClickHidebtnOn()
    {
       myOtherButton.SetActive(true);
       this.gameObject.SetActive(false);
       ReferencesForGamePlay.instance.potraithiddenButtonDisable();
    }
    public void ClickHidebtnOff()
    {
        this.gameObject.SetActive(false);
        myOtherButton.SetActive(true);
        ReferencesForGamePlay.instance.potraithiddenButtonEnable();
       
    }

    public void HideButtonsForFreeCam(bool b)
    {
        myOtherButton.GetComponent<Button>().interactable = !b;
        if (GetComponent<Button>())
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
            ReferencesForGamePlay.instance.potraithiddenButtonDisable();
            ReferencesForGamePlay.instance.JoyStick.SetActive(true);
            ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            if (ReferencesForGamePlay.instance.isHidebtn)
            {
                //ClickHidebtnOff();
                ReferencesForGamePlay.instance.potraithiddenButtonEnable();
                ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
}
