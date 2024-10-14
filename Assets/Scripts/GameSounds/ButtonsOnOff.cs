using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    Button button;
    Button otherbutton;
    public void ClickHidebtnOn()
    {
        GamePlayUIHandler.inst.isHideButton = true;
        otherButton.SetActive(true);
        this.gameObject.SetActive(false);
        ReferencesForGamePlay.instance.hiddenButtonDisable();
        BuilderEventManager.UIToggle?.Invoke(true);
    }
    public void ClickHidebtnOff()
    {
        GamePlayUIHandler.inst.isHideButton = false;
        this.gameObject.SetActive(true);
        otherButton.SetActive(false);
        ReferencesForGamePlay.instance.hiddenButtonEnable();
        BuilderEventManager.UIToggle?.Invoke(false);

    }
    private void Start()
    {
        button = GetComponent<Button>();
        otherbutton = otherbutton?.GetComponent<Button>();
    }
 


    public void HideButtonsForFreeCam(bool b)
    {
        otherbutton.interactable = !b;
        if (button)
            button.interactable = !b;

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
            if (ReferencesForGamePlay.instance.isHidebtn)
            {
                //ClickHidebtnOff();
                ReferencesForGamePlay.instance.hiddenButtonEnable();
                ReferencesForGamePlay.instance.JoyStick.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
    public void EnableSelfieButton()
    {
        ReferencesForGamePlay.instance.hiddenBtnObjects[3].SetActive(false);
    }
    public void DisableSelfieButton()
    {
        ReferencesForGamePlay.instance.hiddenBtnObjects[3].SetActive(true);
    }
}
