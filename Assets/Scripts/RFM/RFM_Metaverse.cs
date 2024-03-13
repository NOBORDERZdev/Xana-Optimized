using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RFM_Metaverse : MonoBehaviour
{
    public BottomTabManager bottomTabManager;
    public Canvas[] canvas;

    private void OnDisable()
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.RFM)
        {
            UserRegisterationManager.instance.StartGameAction -= PlayPMYManually;
            AdditiveScenesManager.OnAllSceneLoaded -= SetDataForPMY;
        }
    }
    private void Start()
    {
        if (XanaConstants.xanaConstants.metaverseType == XanaConstants.MetaverseType.RFM)
        {
            AdditiveScenesManager.OnAllSceneLoaded += SetDataForPMY;
            foreach (var canvasGroup in canvas)
                canvasGroup.enabled = false;
        }
    }

    private void OnEnable()
    {
        UserRegisterationManager.instance.StartGameAction += PlayPMYManually;
    }

    void SetDataForPMY()
    {
        if (XanaConstants.xanaConstants.isBackFromRFM)      // when user back from PMY
        {
            XanaConstants.xanaConstants.isBackFromRFM = false;
            if (IsLoggedIn())
            {
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
                StoreManager.instance._CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }
            else
            {
                UserRegisterationManager.instance.welcomeScreen.SetActive(true);
            }
        }
        //else
        //    PlayPMYManually();  // call when user launch the app except very first time

        UserRegisterationManager.instance.StartGameAction += PlayPMYManually;
    }

    private void PlayPMYManually()
    {
        // bottomTabManager.OnClickHomeButton();

        Debug.LogError("Play RFM Manually");
        FindObjectOfType<WorldItemView>().LoadRFMDirectly();

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
