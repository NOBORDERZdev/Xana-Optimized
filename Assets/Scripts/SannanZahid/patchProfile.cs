using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patchProfile : MonoBehaviour
{
    GameManager gameManager;
    private void Awake()
    {
        gameManager = GameManager.Instance;
    }
    private void OnEnable()
    {
        if (!XanaConstants.xanaConstants.LoginasGustprofile)
        {
            if (gameManager.UiManager != null)//rik
            {
                // GameManager.Instance.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().SetProfileButton();
                
            }
        }
    }
    private void OnDisable()
    {
        gameManager.ActorManager.IdlePlayerAvatorForMenu(false);
        gameManager.m_RenderTextureCamera.gameObject.SetActive(false);
    }
}
