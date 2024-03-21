using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patchProfile : MonoBehaviour
{
    private void OnEnable()
    {
        if (!XanaConstants.xanaConstants.LoginasGustprofile)
        {
            if (UIManager.Instance != null)//rik
            {
                // UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().SetProfileButton();
                
            }
        }
    }
    private void OnDisable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        GameManager.Instance.m_RenderTextureCamera.gameObject.SetActive(false);
    }
}
