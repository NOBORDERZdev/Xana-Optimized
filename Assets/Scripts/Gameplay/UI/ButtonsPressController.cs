using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Metaverse;

[System.Serializable]
public class Btn
{
    public Sprite normal;
    public Sprite pressed;
    //public Image image;
    public GameObject[] screens;


}

public class ButtonsPressController : MonoBehaviour
{
    public static ButtonsPressController Instance;

    [SerializeField] public List<Btn> btns;

    public bool Settings_pressed = false;

    bool IsHelpPanelOpen = false;




    private void Awake()
    {
        Instance = this;
    }

    public void OnEnable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnHelpButton += HelpBtnPressed;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnExitButton += OnExitClick;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnSettingButton += OnSettingClick;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnInvite += OnInviteClick;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnHelpButton -= HelpBtnPressed;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnExitButton -= OnExitClick;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnSettingButton -= OnSettingClick;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnInvite -= OnInviteClick;
    }

   

    public void SetPress(int index)
    {
        bool showScreen = !btns[index].screens[0].activeInHierarchy;

        CloseAllScreens();

        Settings_pressed = true;
        if (showScreen)
        {
            foreach (var screen in btns[index].screens)
            {
                if (!screen.activeInHierarchy)
                {
                    screen.SetActive(true);
                }
            }
        }
        else
        {
            Settings_pressed = false;
        }

    }

    void OnExitClick()
    {
        SetPress(1);
    }

    void OnSettingClick()
    {
        SetPress(0);
        if (ReferrencesForDynamicMuseum.instance.playerControllerNew.isFirstPerson)
        {
            ReferrencesForDynamicMuseum.instance.playerControllerNew.gyroButton.SetActive(true);
            ReferrencesForDynamicMuseum.instance.playerControllerNew.gyroButton_Portait.SetActive(true);
        }
    }
    void OnInviteClick()
    {
        if (SNSNotificationManager.Instance != null)
            SNSNotificationManager.Instance.ShowNotificationMsg("This features is coming soon");
    }
    public void CloseAllScreens()
    {
        foreach (var btn in btns)
        {
            foreach (var screen in btn.screens)
            {
                screen.SetActive(false);
            }
        }
    }

    public void HelpBtnPressed()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.UpdateHelpObjects(IsHelpPanelOpen);
    }


   
    public void ResetAllToClose()
    {
        CloseAllScreens();
    }

    private bool IsIpad()
    {
        if (Camera.main.aspect >= 1.3 && Camera.main.aspect <= 1.35)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
