using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HomeFooterTabCanvas : MonoBehaviour
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
    public GameObject postingBtn;
    public Image PostButton;
    public GameObject chatMessageUnReadCountObj;
    public TextMeshProUGUI chatMessageUnReadCountText;
    AdditiveScenesController additiveScenesManager;
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
        if (UIHandler.Instance != null)
        {
            GameManager.Instance.defaultSelection = 0;
        }
        //---->>>Sannan  OnSelectedClick(GameManager.Instance.defaultSelection);


        if (UIHandler.Instance != null && GameManager.Instance.defaultSelection == 0)
        {
            CheckLoginOrNotForFooterButton();
        }
        if (additiveScenesManager== null)
        {
            additiveScenesManager = GameManager.Instance.additiveScenesManager;
        }
        if (XanaConstantsHolder.xanaConstants.CurrentSceneName == "Addressable" && !XanaConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            XanaConstantsHolder.xanaConstants.CurrentSceneName = "";
            Invoke(nameof(OnClickHomeWorldButton), 5);
        }
    }


    private void OnEnable()
    {
        MainSceneEventHandler.OnSucessFullLogin += CheckLoginOrNotForFooterButton;
    }

    private void OnDisable()
    {
        MainSceneEventHandler.OnSucessFullLogin -= CheckLoginOrNotForFooterButton;
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
            if (i == 2 || i == 3)
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
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[2].transform.GetComponent<Image>().color = DisableButtonColor;
            allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[3].transform.GetComponent<Image>().color = DisableButtonColor;
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = false;
            allButtonIcon[4].transform.GetComponent<Image>().color = DisableButtonColor;
        }
        else
        {
            if (postingBtn != null)
            {
                postingBtn.transform.GetComponent<Button>().interactable = true;
                postingBtn.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            }
            allButtonIcon[2].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[2].transform.GetComponent<Image>().color = ActiveButtonColor;
            allButtonIcon[3].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[3].transform.GetComponent<Image>().color = ActiveButtonColor;
            allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = true;
            allButtonIcon[4].transform.GetComponent<Image>().color = ActiveButtonColor;
        }
        if (CommonAPIHandler.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIHandler.Instance.RequestGetAllChatUnReadMessagesCount();
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
                //if (i == 2 || i == 3){
                //    break;
                //}
                allButtonIcon[i].color = ActiveButtonColor;
                //if (i == 2)
                //{
                //    allButtonIcon[i].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                //}
                if (AllTitleText.Count> i && AllTitleText[i] != null) 
                {
                    AllTitleText[i].color = unSellectedColor;
                }
                allButtonIcon[i].transform.parent.GetComponent<Button>().interactable = true;

            }
        }

        if (CommonAPIHandler.Instance != null && PlayerPrefs.GetInt("IsLoggedIn") != 0)//For Get All Chat UnRead Message Count.......
        {
            CommonAPIHandler.Instance.RequestGetAllChatUnReadMessagesCount();
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
        if (!(GlobalVeriableClass.callingScreen == "Home"))
        {
            GlobalVeriableClass.callingScreen = "Home";
            if (/*GameManager.Instance.defaultSelection != 0*/ true)
            {
                GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(true);
                GameManager.Instance.defaultSelection = 0;
                if (additiveScenesManager != null)
                {
                    additiveScenesManager.SNSmodule.SetActive(false);
                    additiveScenesManager.SNSMessage.SetActive(false);
                }
                //  GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(false);
                GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
                if (UIHandler.Instance != null)
                {
                    CheckLoginOrNotForFooterButton();
                    UIHandler.Instance.HomeWorldScreen.SetActive(false);
                    UIHandler.Instance.HomePage.SetActive(true);
                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    UIHandler.Instance.Canvas.SetActive(true);

                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 1;
                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;

                    if (FeedsManager.Instance)
                    {
                        FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                        FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                        FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                }
            }
            GameManager.Instance.ActorManager.IdlePlayerAvatorForPostMenu(false);
            GameManager.Instance.HomeCamera.GetComponent<HomeCameraHandler>().CenterAlignCam();
            DisableSubScreen();
        }
        GameManager.Instance.HomeCameraInputHandler(true);
        //GlobalVeriableClass.callingScreen = "";
    }
    public void OnClickHomeButtonIdleAvatar()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        OnClickHomeButton();
    }
    public void OnClickHomeWorldButton()
    {
        GameManager.Instance.HomeCameraInputHandler(false);

        GlobalVeriableClass.callingScreen = "";
        Debug.Log("Home button onclick");
        if (GameManager.Instance.defaultSelection != 1)
        {
            GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
            GameManager.Instance.defaultSelection = 1;
            //  GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
            //OnSelectedClick(0);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
                additiveScenesManager.SNSMessage.SetActive(false);
            }
            ////---->>>Sannan   if (UIHandler.Instance != null)
            //   {
            //     UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().defaultSelection = 0;
            //     UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().OnSelectedClick(0);
            // }
            if (UIHandler.Instance != null)
            {
                CheckLoginOrNotForFooterButton();
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                UIHandler.Instance.Canvas.SetActive(true);
                UIHandler.Instance.HomeWorldScreen.SetActive(true);
                UIHandler.Instance.HomePage.SetActive(false);
                UIHandler.Instance.SwitchToScreen(0);
                if (FeedsManager.Instance)
                {
                    FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                    FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                    FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }

            }
           
            WorldsHandler.LoadHomeScreenWorlds?.Invoke();
            RectModifire.OnAdjustSize?.Invoke(false);
            DisableSubScreen();
            //WorldsHandler.instance.ChangeWorld(APIURL.Hot);
            //WorldsHandler.instance.AllWorldTabReference.ScrollEnableDisable(0);
        }

        if (WorldSearchManager.IsSearchBarActive)
        {
            WorldSearchManager.IsSearchBarActive = false;
            WorldsHandler.instance.worldSearchManager.ClearInputField();
        }
    }

    /*public void OnClickNewWorldButton()
    {
        //if (!UIHandler.Instance.WorldPage.activeSelf)
        {
            Debug.Log("World button onclick");
            if (GameManager.Instance.defaultSelection != 1)
            {
               // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
                OnSelectedClick(1);
                if (FindObjectOfType<AdditiveScenesController>() != null)
                {
                    FindObjectOfType<AdditiveScenesController>().SNSmodule.SetActive(false);
                    FindObjectOfType<AdditiveScenesController>().SNSMessage.SetActive(false);
                }
                if (UIHandler.Instance != null)
                {
                    GameManager.Instance.defaultSelection = 1;
                    UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().OnSelectedClick(1);
                }
               // UIHandler.Instance.Canvas.SetActive(true);
                UIHandler.Instance.SwitchToScreen(1);
                WorldsHandler.instance.ChangeWorld(APIURL.Hot);
                WorldsHandler.instance.AllWorldTabReference.ScrollEnableDisable(0);
            }
        }
    }*/

    public void SetProfileButton()
    {
        allButtonIcon[4].transform.parent.GetComponent<Button>().interactable = true;
        allButtonIcon[4].transform.GetComponent<Image>().color = ActiveButtonColor;
    }
    public void OnClickAvatarButton()
    {
        if (GameManager.Instance.defaultSelection != 0)
        {
            // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
            OnSelectedClick(1);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
                additiveScenesManager.SNSMessage.SetActive(false);
            }
            if (UIHandler.Instance != null)
            {
                GameManager.Instance.defaultSelection = 0;
                UIHandler.Instance._footerCan.transform.GetChild(0).GetComponent<HomeFooterTabCanvas>().OnSelectedClick(0);
            }

            // UIHandler.Instance.Canvas.SetActive(true);
        }
        GameManager.Instance.BottomAvatarBtnPressed();
    }
    //this method is used to Explore button click.......
    public void OnClickWorldButton()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("WorldButton"))
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
            GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
            // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
            OnSelectedClick(1);
            if(additiveScenesManager != null)
            {
                if (MessageController.Instance != null)
                {
                    MessageController.Instance.isChatDetailsScreenDeactive = true;
                }
                additiveScenesManager.SNSMessage.SetActive(true);
                additiveScenesManager.SNSmodule.SetActive(false);
                GameManager.Instance.defaultSelection = 1;
                MessageController.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().OnSelectedClick(1);
            }
            else
            {
                Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
            }

            if (UIHandler.Instance.Canvas.activeSelf)
            {
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = true;
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                UIHandler.Instance.Canvas.SetActive(true);
                // UIHandler.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }
    /*public void OnclickEventButton()
    {
        Debug.Log("OnclickEventButton");
        UserPassManager.Instance.OpenComingSoonPopUp();
    }*/

    //this method is used to create button click.......
    public void OnClickCreateButton()
    {
        Debug.Log("Create button onclick");

        if (GameManager.Instance.defaultSelection != 5)
        {
            GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
            //OnSelectedClick(5);
            GameManager.Instance.defaultSelection = 5;
            GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
            if (XanaConstantsHolder.xanaConstants.r_MainSceneAvatar != null)
            {
                Destroy(XanaConstantsHolder.xanaConstants.r_MainSceneAvatar);
                XanaConstantsHolder.xanaConstants.r_MainSceneAvatar = null;
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
            XanaConstantsHolder.xanaConstants.r_MainSceneAvatar = MainSceneAvatar;

        }
    }

    //this method is used to feed button click.......
    public void OnClickFeedButton()
    {
        //if (!UserPassManager.Instance.CheckSpecificItem("sns_feed"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        GameManager.Instance.HomeCameraInputHandler(false);

        if (GameManager.Instance.defaultSelection != 3)
        {
            GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
            // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
            // LoaderShow(true);
            OnSelectedClick(3);
            GameManager.Instance.defaultSelection = 3;
            // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
            GlobalVeriableClass.callingScreen = "Feed";
            // GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                additiveScenesManager.SNSMessage.SetActive(false);
                GameManager.Instance.defaultSelection = 3;
                FeedsManager.Instance.feedUiScreen.SetActive(true);
                FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().OnSelectedClick(3);
                FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                UIHandler.Instance.HomeWorldScreen.SetActive(false);
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 0;
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = false;
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }

            }
            if (MyProfileManager.Instance.myProfileScreen.activeSelf)
            {
                //FeedsManager.Instance.FadeInOutScreenShow();//show fade in out.......
                FeedsManager.Instance.ResetAllFeedScreen(true);
                MyProfileManager.Instance.MyProfileSceenShow(false);//false my profile screen
            }
            //else
            //{
            //    SNS_APIResponseManager.Instance.RequestGetUserDetails("myProfile");
            //}

            if (FeedsManager.Instance != null)
            {
                FeedsManager.Instance.SetAddFriendScreen(false);
                FeedsManager.Instance.feedUiScreen.SetActive(true);
                FeedsManager.Instance.profileFollowerFollowingListScreen.SetActive(false);
                // OLD FEED UI
                ////if (FeedsManager.Instance.feedUiScreen.activeSelf)
                ////{
                ////    FeedsManager.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
                ////}
                // End Old Feed UI
            }
            UIHandler.Instance.HomeWorldScreen.SetActive(false);
            if (UIHandler.Instance.Canvas.activeSelf)
            {
                // UIHandler.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }


            //home page thumnbail images destroy
            WorldsHandler.instance.ClearHomePageData();
            DisableSubScreen();
        }
    }

    public void OnClickAddFriends()
    {
        //if (!UserPassManager.Instance.CheckSpecificItem("AdFriends"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        GameManager.Instance.HomeCameraInputHandler(false);

        if (!XanaConstantsHolder.loggedIn) // Show login page for not sign in
        {
            //show popup here to login for adding friends
            //UserRegisterationManager.instance.OpenUIPanal(17);
            return;
        }
        GameManager.Instance.ActorManager._cinemaCam.SetActive(false);
        if (GameManager.Instance.defaultSelection != 5)
        {
            GameManager.Instance.defaultSelection = 5;
            GlobalVeriableClass.callingScreen = "Feed";

            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                additiveScenesManager.SNSMessage.SetActive(false);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            //below camera line was Commented before but i uncommented it in order to make profile 2.0 work ------- UMER
            GameManager.Instance.m_MainCamera.gameObject.SetActive(true);
            FeedsManager.Instance.SetAddFriendScreen(true);
            SNS_APIResponseManager.Instance.SetHotFriend();
            FeedsManager.Instance.findFriendInputFieldAdvanced.Text = "";
            FeedsManager.Instance.findFriendScreen.gameObject.SetActive(false);
            //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
            FeedsManager.Instance.OnClickHotFrnd();
            FeedsManager.Instance.ResetAllFeedScreen(true);
            FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();


            //Invoke(nameof(InvokeDisableFeed),1f);
            //if (MyProfileManager.Instance.myProfileScreen.activeSelf)
            //{
            //    //FeedsManager.Instance.FadeInOutScreenShow();//show fade in out.......
            //    FeedsManager.Instance.ResetAllFeedScreen(true);
            //    MyProfileManager.Instance.MyProfileSceenShow(false);//false my profile screen
            //}
            //else
            //{
            //    SNS_APIResponseManager.Instance.RequestGetUserDetails("myProfile");
            //}

            //if (FeedsManager.Instance != null)
            //{
            //    if (FeedsManager.Instance.feedUiScreen.activeSelf)
            //    {
            //        FeedsManager.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
            //    }
            //}
            UIHandler.Instance.HomeWorldScreen.SetActive(false);
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 0;
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = false;
            UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

            FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
            FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
            FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (UIHandler.Instance.Canvas.activeSelf)
            {
                // UIHandler.Instance.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }

            DisableSubScreen();
        }
        if (MyProfileManager.Instance)
        {
            MyProfileManager.Instance.MyProfileSceenShow(false);
            MyProfileManager.Instance.OtherPlayerdataObj.SetActive(true);
            FeedsManager.Instance.AddFriendPanel.SetActive(true);
            MyProfileManager.Instance.gameObject.SetActive(false);
        }
        else
        {
            FeedsManager.Instance.AddFriendPanel.SetActive(true);
            OtherUserProfileManager.Instance.myPlayerdataObj.GetComponent<MyProfileManager>().myProfileScreen.SetActive(false);
            OtherUserProfileManager.Instance.myPlayerdataObj.gameObject.SetActive(false);
        }
        FeedsManager.Instance.feedUiScreen.SetActive(false);

    }

    void DisableSubScreen ()
    {
        if (FeedsManager.Instance != null)
        {
            SNSSettingManager.Instance.settingScreen.SetActive(false);
            SNSSettingManager.Instance.myAccountScreen.SetActive(false);
        }
    }   


    //void InvokeDisableFeed(){ 
    //    FeedsManager.Instance.feedUiScreen.SetActive(false);
    //}

    //this method is used to Profile button click.......
    public void OnClickProfileButton()
    {
        GameManager.Instance.HomeCameraInputHandler(false);

        if (/*GameManager.Instance.defaultSelection != 4*/ true)
        {
            // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
            //---->>>Sannan OnSelectedClick(4);
            if (GlobalVeriableClass.callingScreen == "Profile")
                return;
            if (FeedsManager.Instance)
            {
                FeedsManager.Instance.feedUiScreen.SetActive(false);
            }

            if (ProfileScreenController.instance)
            {
                // Reset Scroller position 
                Transform contantObj = ProfileScreenController.instance.mainscrollControllerRef.m_ScrollRect.content.transform;
                Vector2 tempPos = contantObj.position;
                tempPos.y = 0f;
                contantObj.position = tempPos;
            }

            GameManager.Instance.defaultSelection = 4;
            GlobalVeriableClass.callingScreen = "Profile";
            GameManager.Instance.ActorManager._cinemaCam.SetActive(true);
            // LoaderShow(true);
            //GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);

            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                additiveScenesManager.SNSMessage.SetActive(false);
                GameManager.Instance.defaultSelection = 4;
                FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().OnSelectedClick(4);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
            //if (!MyProfileManager.Instance.myProfileScreen.activeSelf)
            //{
            //MyProfileManager.Instance.ProfileTabButtonClick();
            //FeedsManager.Instance.ResetAllFeedScreen(false);
            //}
            if (MyProfileManager.Instance)
            {
                MyProfileManager.Instance.ProfileTabButtonClick();
                FeedsManager.Instance.ResetAllFeedScreen(false);
                FeedsManager.Instance.AddFriendPanel.SetActive(false);
                FeedsManager.Instance.ShowLoader(true);
            }
            if (UIHandler.Instance.Canvas.activeSelf)
            {
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().alpha = 0; // hiding home footer
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().interactable = false;
                UIHandler.Instance._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                UIHandler.Instance.Canvas.SetActive(false);

                UIHandler.Instance.HomeWorldScreen.SetActive(false);
                FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 1;
                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedsManager.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                Invoke("ClearUnloadAssetData", 0.2f);
            }
            //GameManager.Instance.ActorManager.IdlePlayerAvatorForPostMenu(true);
            if (OtherUserProfileManager.Instance)
            {
                OtherUserProfileManager.Instance.myPlayerdataObj.SetActive(true);
                MyProfileManager.Instance.ResetMainScrollDefaultTopPos();
            }
            if (MyProfileManager.Instance)
            {
                MyProfileManager.Instance.OtherPlayerdataObj.SetActive(false);
            }
            ProfileScreenController.instance.SwitchBetwenUserAndOtherProfileUI(true);
            ProfileScreenController.instance.SetMainScrolRefs();
            ProfileScreenController.instance.SetUserAvatarClothing(GameManager.Instance.mainCharacter.GetComponent<AvatarSetupController>()._PCharacterData);
            ProfileScreenController.instance.editProfileBtn.SetActive(true);
            ProfileScreenController.instance.followProfileBtn.SetActive(false);

            DisableSubScreen();
        }

        //home page thumnbail images destroy
        WorldsHandler.instance.ClearHomePageData();
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(false);
    }

    public void ShopButtonClicked()
    {
        if (additiveScenesManager != null)
        {
            additiveScenesManager.SNSmodule.SetActive(false);
            additiveScenesManager.SNSMessage.SetActive(false);
            // FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().GameManager.Instance.defaultSelection = 4;
            //  FeedsManager.Instance.footerCan.GetComponent<HomeFooterTabCanvas>().OnSelectedClick(4);
        }
        // GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        //  GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
        // GameManager.Instance.mainCharacter.GetComponent<HomeAvatarHandler>().UpdateState(true);
        UIHandler.Instance.HomeWorldScreen.SetActive(false);
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
        //Resources.UnloadUnusedAssets();
    }

    public void createBackFromSns()
    {
        XanaConstantsHolder.xanaConstants.isBackfromSns = true;
    }


    public void ComingSoon()
    {
        SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
    }
}