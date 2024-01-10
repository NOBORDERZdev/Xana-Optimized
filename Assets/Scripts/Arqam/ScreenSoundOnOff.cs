using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenSoundOnOff : MonoBehaviour
{
    public GameObject onBtn, OffBtn;
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

    public void OnBtnClicked()
    {
        ScreenSoundStatus?.Invoke(true);
        OffBtn.SetActive(true);
        onBtn.SetActive(false);
    }

    public void OffBtnClicked()
    {
        ScreenSoundStatus?.Invoke(false);
        OffBtn.SetActive(false);
        onBtn.SetActive(true);
    }

}
