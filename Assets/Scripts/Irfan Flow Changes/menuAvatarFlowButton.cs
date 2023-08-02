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
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickMenuAvatarBtn);
    }
 
    void OnClickMenuAvatarBtn()
    {
        GameManager.Instance.AvatarMenuBtnPressed();
        StoreManager.instance.SubmitUserDetailAPI();
    }

    //private void OnEnable()
    //{
    //    StoreBtnController();
    //}
  
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
