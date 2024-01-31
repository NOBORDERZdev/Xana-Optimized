using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileUIHandler : MonoBehaviour
{
    public static ProfileUIHandler instance;
    [Header("User Data Tabs Buttons")]
    public GameObject myProfileTopPartButton;
    public GameObject OtherProfileTopPartButton;

    [Space]
    [Header("User Post Containers")]
    public GameObject myProfileUserPostPartObj;
    public GameObject OtherProfileUserPostPartObj;

    [Space]
    [Header("User Data Tabs Immitating Buttons")]
    public GameObject myProfileImitateTopPartButton;
    public GameObject OtherProfileImitateTopPartButton;

    [Space]
    [Header("Script References")]
    public MainScrollController mainscrollControllerRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetMainScrolRefs()
    {
        if (MyProfileDataManager.Instance.gameObject.activeSelf)
        {
            mainscrollControllerRef.TopFixedObj = myProfileImitateTopPartButton;
            mainscrollControllerRef.headerObj = myProfileTopPartButton;
            mainscrollControllerRef.containerobj = myProfileUserPostPartObj.GetComponent<RectTransform>();
        }
        else
        {
            mainscrollControllerRef.TopFixedObj = OtherProfileImitateTopPartButton;
            mainscrollControllerRef.headerObj = OtherProfileTopPartButton;
            mainscrollControllerRef.containerobj = OtherProfileUserPostPartObj.GetComponent<RectTransform>();
        }
    }

    public void SwitchBetwenUserAndOtherProfileUI(bool _state)
    {
        if (_state)
        {
            myProfileTopPartButton.SetActive(_state);
            myProfileUserPostPartObj.SetActive(_state);
            myProfileImitateTopPartButton.SetActive(_state);
            OtherProfileTopPartButton.SetActive(!_state);
            OtherProfileUserPostPartObj.SetActive(!_state);
            OtherProfileImitateTopPartButton.SetActive(!_state);
        }
        else
        {
            myProfileTopPartButton.SetActive(_state);
            myProfileUserPostPartObj.SetActive(_state);
            myProfileImitateTopPartButton.SetActive(_state);
            OtherProfileTopPartButton.SetActive(!_state);
            OtherProfileUserPostPartObj.SetActive(!_state);
            OtherProfileImitateTopPartButton.SetActive(!_state);
        }
    }
}
