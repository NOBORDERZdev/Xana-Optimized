using System.Collections;
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

    //public int gameManager.defaultSelection = 0;
    public bool WaitToLoadAvatarData = false;
    public CanvasGroup canvasGroup;
    public GameObject postingBtn;
    public Image PostButton;
    public GameObject chatMessageUnReadCountObj;
    public TextMeshProUGUI chatMessageUnReadCountText;
    AdditiveScenesManager additiveScenesManager;
    GameManager gameManager;
    private void Awake()
    {
        gameManager = GameManager.Instance;
        if (gameManager.defaultSelection == 3)
        {
            if (GlobalVeriableClass.callingScreen == "Profile")
            {
                gameManager.defaultSelection = 4;
            }
            else
            {
                GlobalVeriableClass.callingScreen = "Feed";
            }
        }
        
    }
    void Start()
    {
        if (gameManager.UiManager != null)
        {
            gameManager.defaultSelection = 0;
        }
        //---->>>Sannan  OnSelectedClick(gameManager.defaultSelection);


        if (gameManager.UiManager != null && gameManager.defaultSelection == 0)
        {
            CheckLoginOrNotForFooterButton();
        }
        if (additiveScenesManager== null)
        {
            additiveScenesManager = gameManager.additiveScenesManager;
        }
        if (XanaConstants.xanaConstants.CurrentSceneName == "Addressable" && !XanaConstants.xanaConstants.isFromXanaLobby)
        {
            XanaConstants.xanaConstants.CurrentSceneName = "";
            GameManager.Instance.defaultSelection = 10;
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
                gameManager.defaultSelection = index;
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
        if (!(GlobalVeriableClass.callingScreen == "Home"))
        {
            GlobalVeriableClass.callingScreen = "Home";
            if (/*gameManager.defaultSelection != 0*/ true)
            {
                gameManager.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(true);
                gameManager.defaultSelection = 0;
                if (additiveScenesManager != null)
                {
                    additiveScenesManager.SNSmodule.SetActive(false);
                    //additiveScenesManager.SNSMessage.SetActive(false);
                }
                //  gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);
                gameManager.ActorManager._cinemaCam.SetActive(false);
                if (gameManager.UiManager != null)
                {
                    CheckLoginOrNotForFooterButton();
                    gameManager.UiManager.HomeWorldScreen.SetActive(false);
                    gameManager.UiManager.HomePage.SetActive(true);
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    gameManager.UiManager.Canvas.SetActive(true);

                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                    gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;

                    if (FeedUIController.Instance)
                    {
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                        FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                }
            }
            gameManager.ActorManager.IdlePlayerAvatorForPostMenu(false);
            gameManager.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
            DisableSubScreen();
        }
        gameManager.HomeCameraInputHandler(true);
        //GlobalVeriableClass.callingScreen = "";
    }
    public void OnClickHomeButtonIdleAvatar()
    {
        gameManager.ActorManager.IdlePlayerAvatorForMenu(false);
        OnClickHomeButton();
    }
    public void OnClickHomeWorldButton()
    {
        gameManager.HomeCameraInputHandler(false);

        GlobalVeriableClass.callingScreen = "";
        Debug.Log("Home button onclick");
        if (gameManager.defaultSelection != 1)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            gameManager.defaultSelection = 1;
            //  gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            //OnSelectedClick(0);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
               // additiveScenesManager.SNSMessage.SetActive(false);
            }
            ////---->>>Sannan   if (gameManager.UiManager != null)
            //   {
            //     gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().defaultSelection = 0;
            //     gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            // }
            if (gameManager.UiManager != null)
            {
                CheckLoginOrNotForFooterButton();
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                gameManager.UiManager.Canvas.SetActive(true);
                gameManager.UiManager.HomeWorldScreen.SetActive(true);
                gameManager.UiManager.HomePage.SetActive(false);
                gameManager.UiManager.SwitchToScreen(0);
                if (FeedUIController.Instance)
                {
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 0;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = false;
                    FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }

            }
           
            WorldManager.LoadHomeScreenWorlds?.Invoke();
            //FlexibleRect.OnAdjustSize?.Invoke(false);
            DisableSubScreen();
            //WorldManager.instance.ChangeWorld(APIURL.Hot);
            //WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
        }

        if (WorldSearchManager.IsSearchBarActive)
        {
            WorldSearchManager.IsSearchBarActive = false;
            WorldManager.instance.worldSearchManager.ClearInputField();
        }
    }

    /*public void OnClickNewWorldButton()
    {
        //if (!gameManager.UiManager.WorldPage.activeSelf)
        {
            Debug.Log("World button onclick");
            if (gameManager.defaultSelection != 1)
            {
               // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
                OnSelectedClick(1);
                if (FindObjectOfType<AdditiveScenesManager>() != null)
                {
                    FindObjectOfType<AdditiveScenesManager>().SNSmodule.SetActive(false);
                    FindObjectOfType<AdditiveScenesManager>().SNSMessage.SetActive(false);
                }
                if (gameManager.UiManager != null)
                {
                    gameManager.defaultSelection = 1;
                    gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(1);
                }
               // gameManager.UiManager.Canvas.SetActive(true);
                gameManager.UiManager.SwitchToScreen(1);
                WorldManager.instance.ChangeWorld(APIURL.Hot);
                WorldManager.instance.AllWorldTabReference.ScrollEnableDisable(0);
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
        if (gameManager.defaultSelection != 0)
        {
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            OnSelectedClick(1);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(false);
               // additiveScenesManager.SNSMessage.SetActive(false);
            }
            if (gameManager.UiManager != null)
            {
                gameManager.defaultSelection = 0;
                gameManager.UiManager._footerCan.transform.GetChild(0).GetComponent<BottomTabManager>().OnSelectedClick(0);
            }

            // gameManager.UiManager.Canvas.SetActive(true);
        }
        gameManager.BottomAvatarBtnPressed();
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

        if (gameManager.defaultSelection != 1)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            OnSelectedClick(1);
            if(additiveScenesManager != null)
            {
                if (MessageController.Instance != null)
                {
                    MessageController.Instance.isChatDetailsScreenDeactive = true;
                }
               // additiveScenesManager.SNSMessage.SetActive(true);
                additiveScenesManager.SNSmodule.SetActive(false);
                gameManager.defaultSelection = 1;
                MessageController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(1);
            }
            else
            {
                Initiate.Fade("SNSMessageModuleScene", Color.black, 1.0f, true);
            }

            if (gameManager.UiManager.Canvas.activeSelf)
            {
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 1; // hiding home footer
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = true;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                gameManager.UiManager.Canvas.SetActive(true);
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }
        }
    }
    /*public void OnclickEventButton()
    {
        Debug.Log("OnclickEventButton");
        PremiumUsersDetails.Instance.OpenComingSoonPopUp();
    }*/

    //this method is used to create button click.......
    public void OnClickCreateButton()
    {
        Debug.Log("Create button onclick");

        if (gameManager.defaultSelection != 5)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            //OnSelectedClick(5);
            gameManager.defaultSelection = 5;
            gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
            if (XanaConstants.xanaConstants.r_MainSceneAvatar != null)
            {
                Destroy(XanaConstants.xanaConstants.r_MainSceneAvatar);
                XanaConstants.xanaConstants.r_MainSceneAvatar = null;
            }
            GameObject MainSceneAvatar = Instantiate(gameManager.mainCharacter);
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
        //if (!PremiumUsersDetails.Instance.CheckSpecificItem("sns_feed"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        gameManager.HomeCameraInputHandler(false);

        if (gameManager.defaultSelection != 3)
        {
            gameManager.ActorManager._cinemaCam.SetActive(false);
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            // LoaderShow(true);
            OnSelectedClick(3);
            gameManager.defaultSelection = 3;
            // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
            GlobalVeriableClass.callingScreen = "Feed";
            // gameManager.m_MainCamera.gameObject.SetActive(true);
            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
                //additiveScenesManager.SNSMessage.SetActive(false);
                gameManager.defaultSelection = 3;
                FeedUIController.Instance.feedUiScreen.SetActive(true);
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(3);
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                gameManager.UiManager.HomeWorldScreen.SetActive(false);
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
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
            //else
            //{
            //    APIManager.Instance.RequestGetUserDetails("myProfile");
            //}

            if (FeedUIController.Instance != null)
            {
                FeedUIController.Instance.SetAddFriendScreen(false);
                FeedUIController.Instance.feedUiScreen.SetActive(true);
                FeedUIController.Instance.profileFollowerFollowingListScreen.SetActive(false);
                // OLD FEED UI
                ////if (FeedUIController.Instance.feedUiScreen.activeSelf)
                ////{
                ////    FeedUIController.Instance.SetUpFeedTabDefaultTop();//set default scroll top.......
                ////}
                // End Old Feed UI
            }
            gameManager.UiManager.HomeWorldScreen.SetActive(false);
            if (gameManager.UiManager.Canvas.activeSelf)
            {
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }


            //home page thumnbail images destroy
            WorldManager.instance.ClearHomePageData();
            DisableSubScreen();
        }
    }

    public void OnClickAddFriends()
    {
        //if (!PremiumUsersDetails.Instance.CheckSpecificItem("AdFriends"))
        //{
        //    print("Please Upgrade to Premium account");
        //    return;
        //}
        //else
        //{
        //    print("Horayyy you have Access");
        //}
        // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
        gameManager.HomeCameraInputHandler(false);

        if (!XanaConstants.loggedIn) // Show login page for not sign in
        {
            //show popup here to login for adding friends
            //UserRegisterationManager.instance.OpenUIPanal(17);
            return;
        }
        gameManager.ActorManager._cinemaCam.SetActive(false);
        if (gameManager.defaultSelection != 5)
        {
            gameManager.defaultSelection = 5;
            GlobalVeriableClass.callingScreen = "Feed";

            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
               // additiveScenesManager.SNSMessage.SetActive(false);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            //below camera line was Commented before but i uncommented it in order to make profile 2.0 work ------- UMER
            gameManager.m_MainCamera.gameObject.SetActive(true);
            FeedUIController.Instance.SetAddFriendScreen(true);
            APIManager.Instance.SetHotFriend();
            FeedUIController.Instance.findFriendInputFieldAdvanced.Text = "";
            FeedUIController.Instance.findFriendScreen.gameObject.SetActive(false);
            //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
            FeedUIController.Instance.OnClickHotFrnd();
            FeedUIController.Instance.ResetAllFeedScreen(true);
            FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();


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
            gameManager.UiManager.HomeWorldScreen.SetActive(false);
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0;
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
            gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;

            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha = 1;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
            FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (gameManager.UiManager.Canvas.activeSelf)
            {
                // gameManager.UiManager.Canvas.SetActive(false);
                Invoke("ClearUnloadAssetData", 0.2f);
            }

            DisableSubScreen();
        }
        if (MyProfileDataManager.Instance)
        {
            MyProfileDataManager.Instance.MyProfileSceenShow(false);
            MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(true);
            FeedUIController.Instance.AddFriendPanel.SetActive(true);
            MyProfileDataManager.Instance.gameObject.SetActive(false);
        }
        else
        {
            FeedUIController.Instance.AddFriendPanel.SetActive(true);
            OtherPlayerProfileData.Instance.myPlayerdataObj.GetComponent<MyProfileDataManager>().myProfileScreen.SetActive(false);
            OtherPlayerProfileData.Instance.myPlayerdataObj.gameObject.SetActive(false);
        }
        FeedUIController.Instance.feedUiScreen.SetActive(false);

    }

    void DisableSubScreen ()
    {
        if (FeedUIController.Instance != null)
        {
            SNSSettingController.Instance.settingScreen.SetActive(false);
            SNSSettingController.Instance.myAccountScreen.SetActive(false);
        }
    }   


    //void InvokeDisableFeed(){ 
    //    FeedUIController.Instance.feedUiScreen.SetActive(false);
    //}

    //this method is used to Profile button click.......
    public void OnClickProfileButton()
    {
        gameManager.HomeCameraInputHandler(false);

        if (/*gameManager.defaultSelection != 4*/ true)
        {
            // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
            //---->>>Sannan OnSelectedClick(4);
            if (GlobalVeriableClass.callingScreen == "Profile")
                return;
            if (FeedUIController.Instance)
            {
                FeedUIController.Instance.feedUiScreen.SetActive(false);
            }

            if (ProfileUIHandler.instance)
            {
                // Reset Scroller position 
                Transform contantObj = ProfileUIHandler.instance.mainscrollControllerRef.m_ScrollRect.content.transform;
                Vector2 tempPos = contantObj.position;
                tempPos.y = 0f;
                contantObj.position = tempPos;
            }

            gameManager.defaultSelection = 4;
            GlobalVeriableClass.callingScreen = "Profile";
            gameManager.ActorManager._cinemaCam.SetActive(true);
            // LoaderShow(true);
            //gameManager.ActorManager.IdlePlayerAvatorForMenu(true);

            if (additiveScenesManager != null)
            {
                additiveScenesManager.SNSmodule.SetActive(true);
               // additiveScenesManager.SNSMessage.SetActive(false);
                gameManager.defaultSelection = 4;
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(4);
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "SNSFeedModuleScene")
                {
                    Initiate.Fade("SNSFeedModuleScene", Color.black, 1.0f, true);
                }
            }
            //Commented in order to make profile 2.0 work after ahsan removed old feedui object from scene ----- UMER
            //if (!MyProfileDataManager.Instance.myProfileScreen.activeSelf)
            //{
            //MyProfileDataManager.Instance.ProfileTabButtonClick();
            //FeedUIController.Instance.ResetAllFeedScreen(false);
            //}
            if (MyProfileDataManager.Instance)
            {
                MyProfileDataManager.Instance.ProfileTabButtonClick();
                FeedUIController.Instance.ResetAllFeedScreen(false);
                FeedUIController.Instance.AddFriendPanel.SetActive(false);
                FeedUIController.Instance.ShowLoader(true);
            }
            if (gameManager.UiManager.Canvas.activeSelf)
            {
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().alpha = 0; // hiding home footer
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().interactable = false;
                gameManager.UiManager._footerCan.GetComponent<CanvasGroup>().blocksRaycasts = false;
                gameManager.UiManager.Canvas.SetActive(false);

                gameManager.UiManager.HomeWorldScreen.SetActive(false);
                FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().HomeSceneFooterSNSButtonIntrectableTrueFalse();
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().alpha= 1;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().interactable = true;
                FeedUIController.Instance.footerCan.GetComponent<CanvasGroup>().blocksRaycasts = true;
                Invoke("ClearUnloadAssetData", 0.2f);
            }
            //gameManager.ActorManager.IdlePlayerAvatorForPostMenu(true);
            if (OtherPlayerProfileData.Instance)
            {
                OtherPlayerProfileData.Instance.myPlayerdataObj.SetActive(true);
                MyProfileDataManager.Instance.ResetMainScrollDefaultTopPos();
            }
            if (MyProfileDataManager.Instance)
            {
                MyProfileDataManager.Instance.OtherPlayerdataObj.SetActive(false);
            }
            ProfileUIHandler.instance.SwitchBetwenUserAndOtherProfileUI(true);
            ProfileUIHandler.instance.SetMainScrolRefs();
            ProfileUIHandler.instance.SetUserAvatarClothing(gameManager.mainCharacter.GetComponent<AvatarController>()._PCharacterData);
            ProfileUIHandler.instance.editProfileBtn.SetActive(true);
            ProfileUIHandler.instance.followProfileBtn.SetActive(false);

            DisableSubScreen();
        }

        //home page thumnbail images destroy
        WorldManager.instance.ClearHomePageData();
        gameManager.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(false);
    }

    public void ShopButtonClicked()
    {
        if (additiveScenesManager != null)
        {
            additiveScenesManager.SNSmodule.SetActive(false);
           // additiveScenesManager.SNSMessage.SetActive(false);
            // FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().gameManager.defaultSelection = 4;
            //  FeedUIController.Instance.footerCan.GetComponent<BottomTabManager>().OnSelectedClick(4);
        }
        // gameManager.ActorManager.IdlePlayerAvatorForMenu(true);
        //  gameManager.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
        // gameManager.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
        gameManager.UiManager.HomeWorldScreen.SetActive(false);
    }
    public void SetDefaultButtonSelection(int index)
    {
        switch (index)
        {
            case 3:
                OnSelectedClick(3);
                gameManager.defaultSelection = 3;
                GlobalVeriableClass.callingScreen = "Feed";
                break;
            case 4:
                OnSelectedClick(4);
                gameManager.defaultSelection = 4;
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
        XanaConstants.xanaConstants.isBackfromSns = true;
    }


    public void ComingSoon()
    {
        SNSNotificationManager.Instance.ShowNotificationMsg("Coming soon");
    }
}