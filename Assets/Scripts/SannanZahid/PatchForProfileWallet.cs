using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForProfileWallet : MonoBehaviour
{
    public GameObject closeloader;
    GameManager gameManager;
    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnDisable()
    {
        if (gameManager.UiManager != null)//rik
        {
            // gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<HomeFooterHandler>().SetProfileButton();
        }
        closeloader.SetActive(false);
    }

    private void Start()
    {
        if (ConstantsHolder.xanaConstants.isWalletLoadingbool)
        {
            Invoke("OpenCrossbtn", 8f);
        }
    }
    public void OpenCrossbtn()
    {
        closeloader.SetActive(true);

    }
    public void CloseCrossbtn()
    {
        if (!ConstantsHolder.loggedIn)
        {
           if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                UserLoginSignupManager.instance.ShowWelcomeScreen();
                LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
                closeloader.SetActive(false);
            }
            else
            {
                UserLoginSignupManager.instance.ShowWelcomeScreen();
                LoadingHandler.Instance.LoadingScreenSummit.SetActive(false);
                closeloader.SetActive(false);
            }
        }
    }

}