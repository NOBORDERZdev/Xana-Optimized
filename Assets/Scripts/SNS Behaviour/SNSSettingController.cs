﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SNSSettingController : MonoBehaviour
{
    //public static SNSSettingController Instance;

    [Header("Setting Screen Reference")]
    public GameObject settingScreen;
    public TextMeshProUGUI versionText;
    [Space]
    [Header("My Account Screen Reference")]
    public GameObject myAccountScreen;

    [Space]
    [Header("My Account Personal Information References")]
    public GameObject myAccountPersonalInfoScreen;
    [SerializeField] private GameObject personalInfoPublicIDObj;

    [SerializeField] private GameObject personalInfoEmailObj;
    [SerializeField] private GameObject personalInfoPhoneNumberObj;
    [SerializeField] private TextMeshProUGUI personalInfoEmailText;
    [SerializeField] private TextMeshProUGUI personalInfoPhoneNumberText;
    [SerializeField] private TextMeshProUGUI personalInfoPublicaddressText;

    [Header("Confirmation Panel for delete Account")]
    public GameObject deleteAccountPopup;

    [Space]
    [Header("Simultaneous Connections Items")]
    public Image btnImageOn;
    public Image btnImageOff;
    //public Sprite offBtn, onBtn;


    #region Setting Screen.......
    //this method is used to Open Setting Screen.......
    public void OnClickSettingOpen()
    {
        settingScreen.SetActive(true);
        SettingScreenSetup();
    }

    //this method is used to Setup Setting Screen.......
    public void SettingScreenSetup()
    {
        versionText.text = Application.version;
    }

    //this method is used to Close Setting Screen......
    public void OnClickSettingClose()
    {
        settingScreen.SetActive(false);
    }

    //this method is used to My Account Button click.......
    public void OnClickMyAccountButton()
    {
        OnClickSettingClose();
        myAccountScreen.SetActive(true);
    }

    //this method is used to My Account Screen Back Button Click.......
    public void OnClickMyAccountBackButton()
    {
        OnClickSettingOpen();
    }

    //this method is used to terms and policy.......
    public void OpenPrivacyPolicyHyperLink()
    {
        if (ConstantsHolder.xanaConstants != null)
        {
            Application.OpenURL(ConstantsGod.r_privacyPolicyLink);
        }
    }

    //this method is used to Tearms and condition button click.......
    public void OpenTermsAndConditionHyperLink()
    {
        if (ConstantsHolder.xanaConstants != null)
        {
            Application.OpenURL(ConstantsGod.r_termsAndConditionLink);
        }
    }
    #endregion

    #region My Account Screen.......
    //this method is used to Personal Information Button Click.......
    public void OnClickPersonalInformationButton()
    {
        Debug.Log("Personal information button click");

        if (MyProfileDataManager.Instance.myProfileData.id == 0)
        {
            FeedUIController.Instance.ShowLoader(true);
            SNS_APIManager.Instance.RequestGetUserDetails("MyAccount");//Get My Profile data    
        }
        else
        {
            SetUpDataOfPersonalInfo(MyProfileDataManager.Instance.myProfileData.email, MyProfileDataManager.Instance.myProfileData.phoneNumber);
            myAccountPersonalInfoScreen.SetActive(true);
        }
    }

    void SetUpDataOfPersonalInfo(string email, string phoneNumber)
    {
        personalInfoPhoneNumberText.text = "";
        personalInfoEmailText.text = "";

        if (!string.IsNullOrEmpty(email))
        {
            personalInfoEmailText.text = email;
            personalInfoEmailObj.SetActive(true);
        }
        else
        {
            personalInfoEmailObj.SetActive(false);
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            personalInfoPhoneNumberText.text = phoneNumber;
            personalInfoPhoneNumberObj.SetActive(true);
        }
        else
        {
            personalInfoPhoneNumberObj.SetActive(false);
        }
        // Public Address
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("publicID")))
        {
            personalInfoPublicaddressText.text = PlayerPrefs.GetString("publicID");
            personalInfoPublicIDObj.SetActive(true);
        }
        else
        {
            personalInfoPublicIDObj.SetActive(false);
        }
    }

    //this method is used to setup data of personal information screen.......
    public void SetUpPersonalInformationScreen()
    {
        FeedUIController.Instance.ShowLoader(false);
        SetUpDataOfPersonalInfo(MyProfileDataManager.Instance.myProfileData.email, MyProfileDataManager.Instance.myProfileData.phoneNumber);
        myAccountPersonalInfoScreen.SetActive(true);
    }

    //this method is used to logout button click.......
    public void OnClickLogoutButton()
    {

        UserLoginSignupManager.instance.LogoutAccount();
        UserLoginSignupManager.instance.ShowWelcomeScreen();
        //PlayerPrefs.SetInt("ShowLiveUserCounter",0);
        if (PlayerPrefs.GetInt("ShowLiveUserCounter") == 1)
        {
            SimultaneousConnectionButton();
        }
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().RemoveAllFriends();
        PlayerPrefs.SetInt("shownWelcome", 0);
        PlayerPrefs.SetString("UserNameAndPassword", "");
        GameManager.Instance.mainCharacter.GetComponent<CharacterOnScreenNameHandler>().SetNameOfPlayerAgain();
        GameManager.Instance.SpaceWorldManagerRef.worldSpaceHomeScreenRef.OnLogoutClearSpaceData();
        GlobalVeriableClass.callingScreen = "";
    }

    //this method is used to logout success.......
    public void LogoutSuccess()
    {
        GameManager.Instance.PostManager.GetComponent<UserPostFeature>().Bubble.gameObject.SetActive(false);
        ConstantsHolder.xanaConstants.userProfileLink = "";
        if (FeedUIController.Instance != null)
        {
            MyProfileDataManager.Instance.ClearAndResetAfterLogout();
            if(NftDataScript.Instance)
                NftDataScript.Instance.ResetNftData();
            if (File.Exists(Application.persistentDataPath + "/NftData.txt"))
            {
                FileInfo file_info = new FileInfo(Application.persistentDataPath + "/NftData.txt");
                file_info.Delete();
            }

            //StoreInstanceClear
            PlayerPrefs.DeleteKey("Loaded");
            if (File.Exists(Application.persistentDataPath + "/loginAsGuestClass") || File.Exists(Application.persistentDataPath + "/logIn"))
            {
                if (GameManager.Instance)
                    GameManager.Instance.mainCharacter.GetComponent<AvatarController>().InitializeAvatar();
                ClearStorePlayerPrefers();
            }

            myAccountScreen.SetActive(false);
            FeedUIController.Instance.ResetAllFeedScreen(false);
            FeedUIController.Instance.feedController.ResetFeedController();
            FeedUIController.Instance.ClearAllFeedDataAfterLogOut();
            FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().OnClickHomeButton();
            FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().CheckLoginOrNotForFooterButton();
            UserPassManager.Instance.combinedUserFeatures.Clear();
            ConstantsGod.UserPriorityRole = "free";
            if (GameManager.Instance.UiManager != null)
            {
                GameManager.Instance.UiManager._footerCan.GetComponentInChildren<HomeFooterHandler>().OnClickHomeButton();
            }
            CommonAPIManager.Instance.SetUpBottomUnReadCount(0);
            if (LoadPlayerAvatar.instance_loadplayer != null)
            {
                LoadPlayerAvatar.instance_loadplayer.EmptyAvatarContainer();
            }
        }
    }
    #endregion

    /// <summary>
    /// Clear player prefers data of store
    /// </summary>
    void ClearStorePlayerPrefers()
    {
        PlayerPrefs.DeleteKey("FaceMorphIndexOne");
        PlayerPrefs.DeleteKey("FaceMorphIndexTwo");
        PlayerPrefs.DeleteKey("AppliedShapeIndexppp");
        PlayerPrefs.DeleteKey("FaceBlendShapeApplied");
        PlayerPrefs.DeleteKey("AppliedShapeIndexppp");
        PlayerPrefs.DeleteKey("SelectedAvatarID");
    }

    public void DeleteAccountConfirmation()
    {
        deleteAccountPopup.SetActive(true);
    }
    public void DeleteAccount()
    {
        UserLoginSignupManager.instance.DeleteAccount(() =>
        {
            deleteAccountPopup.SetActive(false);
        });
    }
    public void SimultaneousConnectionButton()
    {
        int status = PlayerPrefs.GetInt("ShowLiveUserCounter");
        if (status == 0)
        {
            // Currently Btn is OFF, enable Btn Here
            btnImageOn.gameObject.SetActive(true);
            btnImageOff.gameObject.SetActive(false);
            status = 1;
        }
        else
        {
            // Currently Btn is ON, disable Btn Here
            status = 0;
            btnImageOn.gameObject.SetActive(false);
            btnImageOff.gameObject.SetActive(true);
        }
        PlayerPrefs.SetInt("ShowLiveUserCounter", status);
    }
    void CheckBtnStatus(int status)
    {
        if (status == 0)
        {
            btnImageOn.gameObject.SetActive(false);
            btnImageOff.gameObject.SetActive(true);
        }
        else
        {
            btnImageOn.gameObject.SetActive(true);
            btnImageOff.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        CheckBtnStatus(PlayerPrefs.GetInt("ShowLiveUserCounter"));
    }
}