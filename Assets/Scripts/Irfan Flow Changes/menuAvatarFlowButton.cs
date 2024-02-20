using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class menuAvatarFlowButton : MonoBehaviour
{
    public static menuAvatarFlowButton _instance;

    private void Awake()
    {
        _instance = this;
    }
    void OnEnable()
    {
        MainSceneEventHandler.OpenPresetPanel += OnClickMenuAvatarBtn;
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickMenuAvatarBtn);
    }

    void OnDisable()
    {
        MainSceneEventHandler.OpenPresetPanel -= OnClickMenuAvatarBtn;
        gameObject.GetComponent<Button>().onClick.RemoveListener(OnClickMenuAvatarBtn);
    }
 
    void OnClickMenuAvatarBtn()
    {
        StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        /* comment out due to new avatar not having store for now
        GameManager.Instance.AvatarMenuBtnPressed();
        StoreManager.instance.SubmitUserDetailAPI();
        */
    }

    public void StoreBtnController()
    {
        if (XanaConstants.xanaConstants != null)
        {
            if (XanaConstants.xanaConstants.isNFTEquiped)
                OnNFTAvatarDisableStore();
            else
                OnNFTAvatarEnableStore();
        }
    }

    void OnNFTAvatarDisableStore()
    {
        this.gameObject.GetComponent<Button>().interactable = false;
    }

    void OnNFTAvatarEnableStore()
    {
        this.gameObject.GetComponent<Button>().interactable = true;
    }
}
