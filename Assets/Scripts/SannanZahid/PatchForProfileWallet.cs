using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForProfileWallet : MonoBehaviour
{
    private void OnDisable()
    {
        if (UIManager.Instance != null)//rik
        {
            // UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
            UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().SetProfileButton();
        }
    }
}
