using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patchProfile : MonoBehaviour
{
    private void OnEnable()
    {
        if (XanaConstants.xanaConstants && !XanaConstants.xanaConstants.LoginasGustprofile)
        {
            if (UIManager.Instance != null)//rik
            {
                // UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().SetProfileButton();
            }
        }
    }
}
