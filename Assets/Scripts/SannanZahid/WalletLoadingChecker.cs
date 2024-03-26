using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletLoadingChecker : MonoBehaviour
{
    public GameObject closeloader;
    private void OnDisable()
    {
        if (UIHandler.Instance != null)//rik
        {
            // UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().SetProfileButton();
        }
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
            UserLoginSignupManager.instance.ShowWelcomeScreen();
        LoadingController.Instance.nftLoadingScreen.SetActive(false);
    }

}