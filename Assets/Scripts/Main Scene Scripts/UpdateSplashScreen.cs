using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public class UpdateSplashScreen : MonoBehaviour
{
    //public Image LoadingImg;
    public GameObject tabBG;
    private bool Once;
    private float time;
    public float TotalTimer;
    public TextMeshProUGUI versionText;
    public static string version = "";

    // Start is called before the first frame update
    void Start()
    {
        //Once = false;
        //time = 0;
        versionText.text = "Ver." + Application.version;
        //if (ConstantsHolder.xanaConstants.screenType==ConstantsHolder.ScreenType.TabScreen)
        //{
        //    tabBG.SetActive(true);
        //}
    }

    //public void CallWelcome()
    //{
    //    UserLoginSignupManager.instance.ShowWelcomeScreen();
    //}

}
