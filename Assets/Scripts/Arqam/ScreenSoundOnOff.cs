using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenSoundOnOff : MonoBehaviour
{
    public GameObject onBtn, OffBtn;
    //private IScreenSoundControl screenSoundControl;
    public static Action<bool> ScreenSoundStatus;

    private void OnEnable()
    {
        if (XanaConstants.xanaConstants.isScreenSoundOn)
        {
            OffBtn.SetActive(false);
            onBtn.SetActive(true);
        }
        else
        {
            OffBtn.SetActive(true);
            onBtn.SetActive(false);
        }
    }

    //public IScreenSoundControl SetScreenSoundControl
    //{
    //   set { screenSoundControl = value; } // set interface reference here this is call by AvProDirectionalSound script
    //}

    public void OnBtnClicked()
    {
        //screenSoundControl.ToggleScreenSound(true);
        ScreenSoundStatus?.Invoke(true);
        OffBtn.SetActive(true);
        onBtn.SetActive(false);
    }

    public void OffBtnClicked()
    {
        //screenSoundControl.ToggleScreenSound(false);
        ScreenSoundStatus?.Invoke(false);
        OffBtn.SetActive(false);
        onBtn.SetActive(true);
    }

}
