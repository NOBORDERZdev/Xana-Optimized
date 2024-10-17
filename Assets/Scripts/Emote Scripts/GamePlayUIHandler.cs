using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GamePlayUIHandler : MonoBehaviour
{
    static GamePlayUIHandler _inst;
    public static GamePlayUIHandler inst
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<GamePlayUIHandler>();
            return _inst;
        }
    }

    public PlayerController ref_PlayerControllerNew;

    [Header("GamePlay ui")]
    public GameObject gamePlayUIParent;

    public GameObject actionsContainer;
    public Transform actionToggleImg;
    public ActionSelectionPanelHandler ActionSelectionPanel;
    public GameObject EmoteReactionPanelMainScreen;
    public GameObject FavoriteSelectionPanel;
    public GameObject AnimationBtnClose;
    public GameObject ReactionBtnClose;
    public Button rotateOrientationLand;

    [Header("FPS Button Reference")]
    public GameObject fPSButton;

    public bool stopCurrentPlayingAnim = false;

    public LoadEmoteAnimations ref_LoadEmoteAnimations;

    [HideInInspector]
    public bool isHideButton = false;
    [HideInInspector]
    public bool isFreeCam = false;
    public GameObject portraitJoystick;
    public GameObject jumpBtn;
    public GameObject JumpUI;
    public GameObject ChatSystem;
    //Summit related UI References
    public EmailEntryUIController SummitCXOEmailAuthUIHandle;

    public GameObject JJPortalPopup;
    public GameObject currentPortalObject;
    public TextMeshProUGUI JJPortalPopupText;
    public string[] JJPortalPopupTextData;
    public MainCharactorControllerValue mainCharactorControllerValue = new MainCharactorControllerValue();

    public class MainCharactorControllerValue
    {
        public float slopeLimit;
        public float stepOffcet;
        public float skinWidth;
        public float minMoveDistance;
        public Vector3 center;
        public float radius;
        public float height;
    }

    #region XANA PARTY WORLD
    [Header("Penpenz Leaderboard")]
    public Text MyRankText;
    public Text MyPointsText;
    public GameObject LeaderboardPanel;
    public GameObject PlayerLeaderboardStatsContainer;
    public GameObject PlayerLeaderboardStatsPrefab;
    public GameObject MoveToLobbyBtn;
    public GameObject SignInPopupForGuestUser;
    #endregion

    private void Start()
    {
        if (rotateOrientationLand)
            rotateOrientationLand.onClick.AddListener(ChangeOrientation);
        ref_PlayerControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
        GetMainCharactorControllerValueHandler();
    }

    public void GetMainCharactorControllerValueHandler()
    {
        //mainCharactorControllerValue.slopeLimit = ref_PlayerControllerNew.characterController.slopeLimit;
        //mainCharactorControllerValue.stepOffcet = ref_PlayerControllerNew.characterController.stepOffset;
        //mainCharactorControllerValue.skinWidth = ref_PlayerControllerNew.characterController.skinWidth;
        //mainCharactorControllerValue.minMoveDistance = ref_PlayerControllerNew.characterController.minMoveDistance;
       mainCharactorControllerValue.center = new Vector3(0,0.86f,0);
       mainCharactorControllerValue.radius = 0.2f;
        //mainCharactorControllerValue.height = ref_PlayerControllerNew.characterController.height;

    }

    public void SetMainCharactorControllerValueHandler()
    {
        //ref_PlayerControllerNew.characterController.slopeLimit = mainCharactorControllerValue.slopeLimit;
        //ref_PlayerControllerNew.characterController.stepOffset = mainCharactorControllerValue.stepOffcet;
        //ref_PlayerControllerNew.characterController.skinWidth = mainCharactorControllerValue.skinWidth;
        //ref_PlayerControllerNew.characterController.minMoveDistance = mainCharactorControllerValue.minMoveDistance;
        ref_PlayerControllerNew.characterController.center = mainCharactorControllerValue.center;
        ref_PlayerControllerNew.GetComponent<CharacterController>().radius = mainCharactorControllerValue.radius;
        //ref_PlayerControllerNew.characterController.height = mainCharactorControllerValue.height;

    }

    private void OnEnable()
    {
        if (_inst != this)
            _inst = this;

        ConstantsHolder.xanaConstants.EnableSignInPanelByDefault = false;
    }
    void ChangeOrientation()
    {
        ScreenOrientationManager._instance.ChangeOrientation_editor();
    }

    public void OnGotoAnotherWorldClick()
    {
        GamePlayButtonEvents.inst.OnGotoAnotherWorldClick();
    }

    public void OnWordrobeClick()
    {
        GamePlayButtonEvents.inst.OnWordrobeClick();
    }

    public void OnHelpButtonClick(bool isOn)
    {
        gamePlayUIParent.SetActive(!isOn);//rik.......
        JumpUI.SetActive(!isOn);
        ChatSystem.SetActive(!isOn);
        GamePlayButtonEvents.inst.UpdateHelpObjects(isOn);
    }

    public void OnSettingButtonClick()
    {
        GamePlayButtonEvents.inst.OnSettingButtonClick();
    }

    public void OnExitButtonClick()
    {
        ConstantsHolder.xanaConstants.LastLobbyName = "";
        if (ConstantsHolder.isFromXANASummit)
        {
            ConstantsHolder.IsSummitDomeWorld = false;
        }
        GamePlayButtonEvents.inst.OnExitButtonClick();
        SetMainCharactorControllerValueHandler();

    }

    public void OnPeopeClick()
    {
        GamePlayButtonEvents.inst.OnPeopeClick();
    }

    public void OnAnnouncementClick()
    {
        GamePlayButtonEvents.inst.OnAnnouncementClick();
    }

    public void OnInviteClick()
    {
        GamePlayButtonEvents.inst.OnInviteClick();
    }

    public void EnableJJPortalPopup(GameObject obj, int indexForText)
    {
        if (LoadingHandler.Instance != null)
        {
            LoadingHandler.Instance.ResetLoadingValues();
        }
        JJPortalPopupText.text = JJPortalPopupTextData[indexForText].ToString();
        currentPortalObject = obj;
        JJPortalPopup.SetActive(true);
    }

    public void MoveFromPortal()
    {
        JJPortalPopup.SetActive(false);
        ref_PlayerControllerNew.m_IsMovementActive = true;
        if (currentPortalObject.GetComponent<PlayerPortal>())
            currentPortalObject.GetComponent<PlayerPortal>().RedirectToWorld();
        else if (currentPortalObject.GetComponent<JjWorldChanger>())
            currentPortalObject.GetComponent<JjWorldChanger>().RedirectToWorld();
        else if (currentPortalObject.GetComponent<MeetingRoomTeleport>())
            currentPortalObject.GetComponent<MeetingRoomTeleport>().RedirectToWorld();
    }

    public void ClosePortalPopup()
    {
        JJPortalPopup.SetActive(false);
        ref_PlayerControllerNew.m_IsMovementActive = true;
    }

    public void OnSwitchCameraClick()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("fp_camera"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        GamePlayButtonEvents.inst.OnSwitchCameraClick();
    }

    public void OnChangehighlightedFPSbutton(bool isSelected)
    {
        fPSButton.GetComponent<Image>().enabled = isSelected;
    }

    public void OnSelfiBtnClick()
    {
        GamePlayButtonEvents.inst.OnSelfieClick();
    }

    public void OnOpenAnimationPanel()
    {
        ref_LoadEmoteAnimations.OpenAnimationSelectionPanel();
        Debug.Log("call hua times 3===" + GamePlayButtonEvents.inst.selectionPanelOpen);
        GamePlayButtonEvents.inst.selectionPanelOpen = true;
        GamePlayButtonEvents.inst.OpenAllAnims();
    }
    public void CloseAnimationButtonClick()
    {
        if(!ActionManager.IsAnimRunning)
            AnimationBtnClose.SetActive(false);
        EmoteReactionPanelMainScreen.SetActive(false);
        FavoriteSelectionPanel.SetActive(false);
    }

    public void CloseReactionButtonClick()
    {
        if (ActionManager.IsAnimRunning)
            AnimationBtnClose.SetActive(true);
        ReactionBtnClose.SetActive(false);
        EmoteReactionPanelMainScreen.SetActive(false);
        FavoriteSelectionPanel.SetActive(false);
    }

    public void CloseEmoteSelectionPanel()
    {
        BuilderEventManager.UIToggle?.Invoke(false);

        EmoteAnimationHandler.Instance.isEmoteActive = false;      // AH working

        if (ActionManager.IsAnimRunning)                            // AH working
        {
            ActionManager.StopActionAnimation?.Invoke();

            // EmoteAnimationHandler.Instance.StopAnimation();
        }

        ref_LoadEmoteAnimations.CloseAnimationSelectionPanel();
        GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();

        if (ReactionFilterManager.Instance && ReactionFilterManager.Instance.gameObject.activeInHierarchy)            // AH working
            ReactionFilterManager.Instance.HideReactionPanel();


        ReactScreen.Instance.HideEmoteScreen();
        // GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();
        GamePlayButtonEvents.inst.selectionPanelOpen = false;

    }

    public void OnJumpBtnUp()
    {
        GamePlayButtonEvents.inst.OnJumpBtnUp();
    }

    public void OnJumpBtnDown()
    {
        GamePlayButtonEvents.inst.OnJumpBtnDown();
    }

    bool isActionShowing;
    public void OnActionsToggleClicked()
    {
        if (ScreenOrientationManager._instance.isPotrait)
        {
            if (ScreenOrientationManager._instance.joystickInitPosY == 0)
                ScreenOrientationManager._instance.joystickInitPosY = portraitJoystick.transform.localPosition.y;
        }
        if (!UserPassManager.Instance.CheckSpecificItem("env_actions"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        isActionShowing = !isActionShowing;
        actionsContainer.SetActive(isActionShowing);
        Vector3 rot = new Vector3(0f, 0f, (isActionShowing) ? 0f : 180f);
        actionToggleImg.rotation = Quaternion.Euler(rot);
        if (jumpBtn)
            jumpBtn.transform.DOLocalMoveX((isActionShowing) ? 277f : 372.6f, 0.1f);
    }
    public void EnableJJPortalPopup(GameObject obj)
    {
        currentPortalObject = obj;
        JJPortalPopup.SetActive(true);
    }

    #region XANA PARTY WORLD
    public void MoveToLobbyBtnClick()
    {
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().RaceStartWithPlayers = 0;
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().PlayerIDs.Clear();
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().WinnerPlayerIds.Clear();
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().RaceFinishTime.Clear();
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().isLeaderboardShown = false;
        XANAPartyManager.Instance.GetComponent<PenpenzLpManager>().ResetGame();
        ConstantsHolder.xanaConstants.isXanaPartyWorld = false;
        ConstantsHolder.xanaConstants.isBuilderGame = false;
        ConstantsHolder.xanaConstants.isJoinigXanaPartyGame = false;
        ConstantsHolder.xanaConstants.LastLobbyName = "";
        //StartCoroutine(GameplayEntityLoader.instance.PenguinPlayer.GetComponent<XANAPartyMulitplayer>().MoveToLobby());
        //LeaderboardPanel.SetActive(false);
        //ReferencesForGamePlay.instance.SetGameplayForPenpenz(true);
        GamePlayButtonEvents.inst.OnExitButtonClick();
    }

    public void OnSignInBtnClick()
    {
        LoadingHandler.Instance.ShowLoading();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        ConstantsHolder.xanaConstants.EnableSignInPanelByDefault = true;
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
        SignInPopupForGuestUser.SetActive(false);
    }
    #endregion
}