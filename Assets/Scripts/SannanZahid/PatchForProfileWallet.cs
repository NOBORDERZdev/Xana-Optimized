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
        LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
    }

}