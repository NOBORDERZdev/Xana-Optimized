using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForProfileWallet : MonoBehaviour
{
     public GameObject closeloader;
    private void OnDisable()
    {
        if (UIManager.Instance != null)//rik
        {
            // UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().SetProfileButton();
        }
    }

    private void Start()
    {
        if (XanaConstants.xanaConstants.isWalletLoadingbool) { 
        
        Invoke("OpenCrossbtn", 8f);
        }
    }
    public void OpenCrossbtn()
    {
       closeloader.SetActive(true);
       
    }
    public void CloseCrossbtn()
    {
        if (UserRegisterationManager.instance._IsWalletSignUp)
        {
            UserRegisterationManager.instance.NewSignUp_Panal .SetActive(true);
           
        }else
        {
            UserRegisterationManager.instance.LoginScreenNew.SetActive(true);
           
        }
        LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
 }
    
}