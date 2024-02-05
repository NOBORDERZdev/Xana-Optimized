using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PMY_Metaverse : MonoBehaviour
{
    public BottomTabManager bottomTabManager;
    public Canvas[] canvas;

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
            foreach (var canvasGroup in canvas)
                canvasGroup.enabled = false;
        }
    }

    void SetDataForPMY()
    {
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

        UserRegisterationManager.instance.StartGameAction += PlayPMYManually;
    }

    private void PlayPMYManually()
    {
        bottomTabManager.OnClickHomeWorldButton();

        //Image blackScreen = LoadingHandler.Instance.Loading_WhiteScreen.GetComponent<Image>();
        //blackScreen.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(delegate
        //{
        //    blackScreen.color = new Color(0, 0, 0, 0);
        //});
    }

    private bool IsLoggedIn()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") != 1 && PlayerPrefs.GetInt("WalletLogin") != 1)
            return false;
        else
            return true;
    }

}
