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
        if (!GameManager.Instance.isAllSceneLoaded)
            return;

        if (ConstantsHolder.xanaConstants.isFirstPanel)
        {
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
            }
            else {
                InventoryManager.instance.StartPanel_PresetParentPanelSummit.SetActive(true);
            }
            ConstantsHolder.xanaConstants.isFirstPanel = false;
        }
        else
        {
            GameManager.Instance.AvatarMenuBtnPressed();
            InventoryManager.instance.SubmitUserDetailAPI();
        }
    }

    public void StoreBtnController()
    {
        if (ConstantsHolder.xanaConstants != null)
        {
            if (ConstantsHolder.xanaConstants.isNFTEquiped)
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
