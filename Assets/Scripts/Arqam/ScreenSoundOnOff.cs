using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSoundOnOff : MonoBehaviour
{
    public GameObject onBtn, OffBtn;
    private IScreenSoundControl screenSoundControl;

    // Start is called before the first frame update
    void Start()
    {

    }

    public IScreenSoundControl SetScreenSoundControl
    {
       set { screenSoundControl = value; } // set interface reference here this is call by AvProDirectionalSound script
    }

    public void OnBtnClicked()
    {

        screenSoundControl.ToggleScreenSound(true);
        OffBtn.SetActive(true);
        onBtn.SetActive(false);
    }

    public void OffBtnClicked()
    {
        screenSoundControl.ToggleScreenSound(false);
        OffBtn.SetActive(false);
        onBtn.SetActive(true);
    }

}
