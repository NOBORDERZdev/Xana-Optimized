using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patchProfile : MonoBehaviour
{
    private void OnEnable()
    {
        if (!XanaConstantsHolder.xanaConstants.LoginasGustprofile)
        {
            if (UIHandler.Instance != null)//rik
            {
                // UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().SetProfileButton();
                
            }
        }
    }
    private void OnDisable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        GameManager.Instance.m_RenderTextureCamera.gameObject.SetActive(false);
    }
}
