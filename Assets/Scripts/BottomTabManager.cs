﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BottomTabManager : MonoBehaviour
{
    public List<Image> allButtonIcon = new List<Image>();
    public List<Sprite> allButtonUnSelected = new List<Sprite>();
    public List<Sprite> allButtonSelected = new List<Sprite>();
    public List<Text> AllTitleText = new List<Text>();
    public Color sellectedColor = new Color();
    public Color unSellectedColor = new Color();
    public Color intractableFalseColor = new Color();
    public Color DisableButtonColor = new Color();
    public Color ActiveButtonColor = new Color();

    //public int GameManager.Instance.defaultSelection = 0;
    public bool WaitToLoadAvatarData = false;
    public CanvasGroup canvasGroup;

    public Image PostButton;
    public GameObject chatMessageUnReadCountObj;
    public TextMeshProUGUI chatMessageUnReadCountText;

    private void Awake()
    {
        if (GameManager.Instance.defaultSelection == 3)
        {
            if (GlobalVeriableClass.callingScreen == "Profile")
            {
                GameManager.Instance.defaultSelection = 4;
            }
            else
            {
                GlobalVeriableClass.callingScreen = "Feed";
            }
        }
    }
    void Start()
    {
        if (UIManager.Instance != null)
        {
            GameManager.Instance.defaultSelection = 0;
        }
      //---->>>Sannan  OnSelectedClick(GameManager.Instance.defaultSelection);

        if (UIManager.Instance != null && GameManager.Instance.defaultSelection == 0)
        {
            CheckLoginOrNotForFooterButton();
        }
    }

    public void OnSelectedClick(int index)
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
           // allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = false;
            //PostButton.transform.GetComponent<Button>().interactable = false;
            //  allButtonIcon[4].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        else
            return;


        for (int i = 0; i < allButtonIcon.Count; i++)
        {
            if (i== 2 || i==3)
            {
                break;
            }
                if (i == index)
                {
                    allButtonIcon[i].sprite = allButtonSelected[i];
                    GameManager.Instance.defaultSelection = index;
                    if (i == 2)
                    {
                        allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = ActiveButtonColor;
                    }
                }
                else
                {
                    allButtonIcon[i].sprite = allButtonUnSelected[i];
                    if (i == 2)
                    {
                        allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                    }
                }
        }
        PostButton.transform.GetComponent<Button>().interactable = true;

    }
    public void CheckLoginOrNotForFooterButton()
    {
        ////---->>>Sannan   if (UIManager.Instance != null)
        //{
        //    UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
        //}
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            /// Disabling old Screens
            //allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = false;
            //allButtonIcon[2].transform.GetComponent<Image>().color = DisableButtonColor;
            //allButtonIcon[3].transform.GetComponent<Image>().color = DisableButtonColor;
            //allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = false;
            /// Disabling
            /// 
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[4].transform.GetComponent<Image>().color = DisableButtonColor;
            //PostButton.transform.GetComponent<Button>().interactable = false;


            //  allButtonIcon[4].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        else
        {
            /// Disabling old Screens
            //allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = true;
            //allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = true;
            /// Disabling
            /// 
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = true;
           // PostButton.transform.GetComponent<Button>().interactable = true;

            /// Disabling old Screens
            //allButtonIcon[2].transform.GetComponent<Image>().color = ActiveButtonColor;
            //allButtonIcon[3].transform.GetComponent<Image>().color = ActiveButtonColor;
            /// Disabling
            /// 
            allButtonIcon[4].transform.GetComponent<Image>().color = ActiveButtonColor;
            // allButtonIcon[4].transform.GetChild(0).GetComponent<Image>().color = Color.black;
        }
        if (CommonAPIManager.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
        }
    }

    public void HomeSceneFooterSNSButtonIntrectableTrueFalse()
    {
        for (int i = 2; i < allButtonIcon.Count; i++)
        {
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                allButtonIcon[i].color = new Color(intractableFalseColor.r, intractableFalseColor.g, intractableFalseColor.b, 0.5f);
                if (i == 2)
                {
                    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = DisableButtonColor /*Color.gray*/;
                }
                AllTitleText[i].color = intractableFalseColor;

                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                if(i==2 || i==3){
                    break;
                }
                allButtonIcon[i].color = ActiveButtonColor;
                //if (i == 2)
                //{
                //    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                //}
                AllTitleText[i].color = unSellectedColor;
                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = true;
                
            }
        }

        if (CommonAPIManager.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIManager.Instance.RequestGetAllChatUnReadMessagesCount();
        }
    }

    Coroutine waitToLoadAvatarDataCo;
    IEnumerator waitToAvatarDataLoad()
    {
        yield return new WaitForSeconds(2f);
        WaitToLoadAvatarData = true;
    }
    public void OnClickHomeButton()
    {
        if (/*GameManager.Instance.defaultSelection != 0*/ true)
        {
            GameManager.Instance.defaultSelection=0;
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
          //  GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);
            if (UIManager.Instance != null)
            {
                CheckLoginOrNotForFooterButton();
                UIManager.Instance.HomeWorldScreen.SetActive(false);
                UIManager.Instance.HomePage.SetActive(true);
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                UIManager.Instance.Canvas.SetActive(true);

                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha= 1;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
                    if (FeedUIController.Instance)
                    {
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 0;
                     FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                }
            }
    }
    public void OnClickHomeButtonIdleAvatar()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        OnClickHomeButton();
    }
    public void OnClickHomeWorldButton()
    {
        GlobalVeriableClass.callingScreen = "";
        Debug.Log("Home button onclick");
        if (GameManager.Instance.defaultSelection != 1)
        {
            GameManager.Instance.defaultSelection=1;
          //  GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            //OnSelectedClick(0);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
            ////---->>>Sannan   if (UIManager.Instance != null)
            //   {
            //     UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 0;
            //     UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            // }
             if (UIManager.Instance != null)
             {
                CheckLoginOrNotForFooterButton();
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha=1; // hiding home footer
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable=true;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts=true;
                UIManager.Instance.Canvas.SetActive(true);
                UIManager.Instance.HomeWorldScreen.SetActive(true);
                UIManager.Instance.HomePage.SetActive(false);
                UIManager.Instance.SwitchToScreen(0);
                if (FeedUIController.Instance)
                {
                     FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 0;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
               

             }
            WorldManager.instance.ChangeWorld(APIURL.Hot);
            WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
        }
    }

    public void OnClickNewWorldButton()
    {
        //if (!UIManager.Instance.WorldPage.activeSelf)
        {
            Debug.Log("World button onclick");
            if (GameManager.Instance.defaultSelection != 1)
            {
               // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
                OnSelectedClick(1);
                if (FindObjectOfType<AdditiveScenesManager>() != null)
                {
                    FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                    FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                }
                if (UIManager.Instance != null)
                {
                    GameManager.Instance.defaultSelection = 1;
                    UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(1);
                }
               // UIManager.Instance.Canvas.SetActive(true);
                UIManager.Instance.SwitchToScreen(1);
                WorldManager.instance.ChangeWorld(APIURL.Hot);
                WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
            }
        }
    }


    public void OnClickAvatarButton()
    {
        if (GameManager.Instance.defaultSelection != 0)
        {
           // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            OnSelectedClick(1);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
            if (UIManager.Instance != null)
            {
                GameManager.Instance.defaultSelection = 0;
                UIManager.Instance._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            }

           // UIManager.Instance.Canvas.SetActive(true);
        }
        GameManager.Instance.BottomAvatarBtnPressed();
    }
    //this method is used to Explore button click.......
    public void OnClickWorldButton()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("WorldButton"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }


        GlobalVeriableClass.callingScreen = "";

        if (GameManager.Instance.defaultSelection != 1)
        {
           // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            OnSelectedClick(1);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                if (MessageController.Instance != null)
                {
                    MessageController.Instance.isChatDetailsScreenDeactive = true;
                }
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                GameManager.Instance.defaultSelection = 1;
                MessageController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(1);
            }
            else
            {
                Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
            }

            if (UIManager.Instance.Canvas.activeSelf)
            {
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha=1; // hiding home footer
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable=true;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts=true;
                UIManager.Instance.Canvas.SetActive(true);
               // UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }
    public void OnclickEventButton()
    {
        Debug.Log("OnclickEventButton");
        PremiumUsersDetails.Instance.OpenComingSoonPopUp();
    }

    //this method is used to create button click.......
    public void OnClickCreateButton()
    {
        Debug.Log("Create button onclick");

        if (GameManager.Instance.defaultSelection != 5)
        {
           
            //OnSelectedClick(5);
            GameManager.Instance.defaultSelection = 5;
            GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
            if (XanaConstants.xanaConstants.r_MainSceneAvatar != null)
            {
                Destroy(XanaConstants.xanaConstants.r_MainSceneAvatar);
                XanaConstants.xanaConstants.r_MainSceneAvatar = null;
            }
            GameObject MainSceneAvatar = Instantiate(GameManager.Instance.mainCharacter);
            Transform rootRotationObj = MainSceneAvatar.transform.Find("mixamorig:Hips");
            if (rootRotationObj != null)
            {
                Transform hadeObj = rootRotationObj.GetChild(2).GetChild(0).GetChild(0).GetChild(1).transform;
                hadeObj.localRotation = Quaternion.Euler(Vector3.zero);
                hadeObj.transform.GetChild(0).transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            DontDestroyOnLoad(MainSceneAvatar);
            MainSceneAvatar.SetActive(false);
            //MainSceneAvatar.transform.parent.transform.eulerAngles= new Vector3(0,180,0);
            Initiate.Fade("ARModuleRoomScene", Color.black, 1.0f, true);
            XanaConstants.xanaConstants.r_MainSceneAvatar = MainSceneAvatar;

        }
    }

    //this method is used to feed button click.......
    public void OnClickFeedButton()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("sns_feed"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        if (GameManager.Instance.defaultSelection != 3)
        {
           // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            // LoaderShow(true);
            OnSelectedClick(3);
            GameManager.Instance.defaultSelection = 3;
           // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
            GlobalVeriableClass.callingScreen = "Feed";
           // GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                GameManager.Instance.defaultSelection = 3;
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(3);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            {
                //FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
                FeedUIController.Instance.ResetAllFeedScreen(true);
                MyProfileDataManager.Instance.MyProfileSceenShow(false);//false my profile screen
            }
            else
            {
                APIManager.Instance.RequestGetUserDetails("myProfile");
            }

            if (FeedUIController.Instance != null)
            {
                FeedUIController.Instance.SetAddFriendScreen(false);
                FeedUIController.Instance.feedUiScreen.SetActive(true);
                if (FeedUIController.Instance.feedUiScreen.activeSelf)
                {
                    FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
                }
            }
            UIManager.Instance.HomeWorldScreen.SetActive(false);
            if (UIManager.Instance.Canvas.activeSelf)
            {
               // UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }

    public void OnClickAddFriends(){
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("AdFriends"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
       // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        if (PlayerPrefs.GetInt("IsLoggedIn") != 1 && PlayerPrefs.GetInt("WalletLogin") != 1) // Show login page for not sign in
        {
             UserRegisterationManager.instance.OpenUIPanal(17);
            return;
        }
        if (GameManager.Instance.defaultSelection != 5){ 
            GameManager.Instance.defaultSelection =5;
            GlobalVeriableClass.callingScreen = "Feed";
            
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
          //  GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
            FeedUIController.Instance.SetAddFriendScreen(true);
            APIManager.Instance.SetHotFriend();
            FeedUIController.Instance.findFriendInputFieldAdvanced.Text = "";
            FeedUIController.Instance.findFriendScreen.gameObject.SetActive(false);
            FeedUIController.Instance.feedUiScreen.GetComponent<FeedScreenOff>().OffFeedScreen();
            FeedUIController.Instance.OnClickHotFrnd();
            FeedUIController.Instance.ResetAllFeedScreen(true);
            MyProfileDataManager.Instance.MyProfileSceenShow(false);
            //Invoke(nameof(InvokeDisableFeed),1f);
            //if (MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            //{
            //    //FeedUIController.Instance.FadeInOutScreenShow();//show fade in out.......
            //    FeedUIController.Instance.ResetAllFeedScreen(true);
            //    MyProfileDataManager.Instance.MyProfileSceenShow(false);//false my profile screen
            //}
            //else
            //{
            //    APIManager.Instance.RequestGetUserDetails("myProfile");
            //}

            //if (FeedUIController.Instance != null)
            //{
            //    if (FeedUIController.Instance.feedUiScreen.activeSelf)
            //    {
            //        FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
            //    }
            //}
            UIManager.Instance.HomeWorldScreen.SetActive(false);
            UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha= 0;
            UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable = false;
            UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
            
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 1;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (UIManager.Instance.Canvas.activeSelf)
            {
                // UIManager.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }

    //void InvokeDisableFeed(){ 
    //    FeedUIController.Instance.feedUiScreen.SetActive(false);
    //}

    //this method is used to Profile button click.......
    public void OnClickProfileButton()
    {
        if (/*GameManager.Instance.defaultSelection != 4*/ true)
        {
           // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            //---->>>Sannan OnSelectedClick(4);
            GameManager.Instance.defaultSelection = 4;
            GlobalVeriableClass.callingScreen = "Profile";

            // LoaderShow(true);
            //GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
            if (FindObjectOfType<AdditiveScenesManager>() != null)
            {
                FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(true);
                FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                GameManager.Instance.defaultSelection = 4;
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(4);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }

            if (!MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            {
                MyProfileDataManager.Instance.ProfileTabButtonClick();
                FeedUIController.Instance.ResetAllFeedScreen(false);
            }
            if (UIManager.Instance.Canvas.activeSelf)
            {
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().alpha=0; // hiding home footer
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().interactable=false;
                UIManager.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts=false;
                UIManager.Instance.Canvas.SetActive(false);

                UIManager.Instance.HomeWorldScreen.SetActive(false);
            
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 1;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }

    public void ShopButtonClicked()
    {
        if (FindObjectOfType<AdditiveScenesManager>() != null)
        {
            FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
            FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
            // FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().GameManager.Instance.defaultSelection = 4;
            //  FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(4);
        }
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
        // GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
        UIManager.Instance.HomeWorldScreen.SetActive(false);
    }
    public void SetDefaultButtonSelection(int index)
    {
        switch (index)
        {
            case 3:
                OnSelectedClick(3);
                GameManager.Instance.defaultSelection = 3;
                GlobalVeriableClass.callingScreen = "Feed";
                break;
            case 4:
                OnSelectedClick(4);
                GameManager.Instance.defaultSelection = 4;
                GlobalVeriableClass.callingScreen = "Profile";
                break;
            default:
                break;
        }
    }
    public void MessageUnReadCountSetUp(int messageUnReadCount)
    {
        if (messageUnReadCount <= 0)
        {
            chatMessageUnReadCountObj.SetActive(false);
        }
        else
        {
            chatMessageUnReadCountText.text = messageUnReadCount.ToString();
            chatMessageUnReadCountObj.SetActive(true);
        }
    }

    void ClearUnloadAssetData()
    {
        Resources.UnloadUnusedAssets();
    }

    public void createBackFromSns(){ 
        XanaConstants.xanaConstants.isBackfromSns= true;    
    }


    public void ComingSoon(){ 
       SNSNotificationManager.Instance.ShowNotificationMsg("Coming soon");
    }
}