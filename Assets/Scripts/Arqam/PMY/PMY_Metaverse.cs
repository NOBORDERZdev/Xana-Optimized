using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PMY_Metaverse : MonoBehaviour
{
    public BottomTabManager bottomTabManager;
    //private void OnEnable()
    //{
    //    if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
    //        AdditiveScenesManager.OnAllSceneLoaded += SetDataForPMY;
    //}
    private void OnDisable()
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
        {
            UserRegisterationManager.instance.StartGameAction -= PlayPMYManually;
            AdditiveScenesManager.OnAllSceneLoaded -= SetDataForPMY;
        }
    }
    private void Start()
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.PMY)
        {
            AdditiveScenesManager.OnAllSceneLoaded += SetDataForPMY;
            UIManager.Instance._SplashScreen.SetActive(true);
        }
    }

    void SetDataForPMY()
    {
        //if (PlayerPrefs.GetInt("IsProcessComplete") == 1)
        //{
        if (XanaConstants.xanaConstants.isBackFromPMY)      // when user back from PMY
        {
            XanaConstants.xanaConstants.isBackFromPMY = false;
            if (IsLoggedIn())
            {
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                StoreManager.instance._CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }
            else
                UserRegisterationManager.instance.welcomeScreen.SetActive(true);
        }
        else
            PlayPMYManually();  // call when user launch the app except very first time
        //}
        //else 
        if (IsLoggedIn())
            PlayPMYManually();

        UserRegisterationManager.instance.StartGameAction += PlayPMYManually;
    }

    private void PlayPMYManually()
    {
        //StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        //UIManager.Instance._SplashScreen.SetActive(true);
        bottomTabManager.OnClickHomeWorldButton();
    }

    private bool IsLoggedIn()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") != 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
            return false;
        else
            return true;
    }

}
