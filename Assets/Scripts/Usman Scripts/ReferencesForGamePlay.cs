using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ReferencesForGamePlay : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
    public GameObject eventSystemObj;
    [Space(5)]
    public GameObject[] overlayPanels;
    public GameObject workingCanvas, PlayerParent, MainPlayerParent;
    public GameObject[] disableObjects;
    public GameObject[] potraitHiddenBtnObjects, potraitdissableBtnObjects;
    public GameObject[] hiddenBtnObjects, disableBtnObjects;
    public static ReferencesForGamePlay instance;
    public Camera randerCamera;
    public List<GameObject> disableObjectsInMuseums;
    public GameObject onBtnUsername;
    public GameObject offBtnUsername;

    public GameObject m_34player;
    public PlayerController playerControllerNew;
    public GameObject spawnedSkateBoard;
    public GameObject minimap;
    public GameObject MinimapSummit;
    public GameObject FullscreenMapSummit;

    public GameObject minimapSettingsBtn;
    public TMPro.TextMeshProUGUI totalCounter; // Counter to show total connected peoples.
    public GameObject ReferenceObject;
    public GameObject ReferenceObjectPotrait;
    public GameObject FirstPersonCam;
    public Button RotateBtn;
    public GameObject JoyStick;
    public int PlayerCount = 0;
    public float MonitorDistance;
    //MoveWhileDancing add kamran
    public GameObject landscapeMoveWhileDancingButton;
    public GameObject portraitMoveWhileDancingButton;
    public int moveWhileDanceCheck;
    public QualityManager QualityManager;
    public XanaChatSystem ChatSystemRef;

    public Image ExitBtnGameplay;
    public Sprite backBtnSprite, HomeBtnSprite;




    #region XANA PARTY WORLD
    public GameObject XANAPartyWaitingPanel;
    public GameObject XANAPartyCounterPanel;
    public TMP_Text XANAPartyCounterText;
    public bool isCounterStarted = false;
    public bool isMatchingTimerFinished = false;
    private const string InLevelProperty = "InLevel";
    public bool IsLevelPropertyUpdatedOnlevelLoad = false;
    [SerializeField]
    private List<GameObject> PenpenzDisableUi;
    #endregion



    // Start is called before the first frame update
    void Awake()
    {

        if (instance != null && instance != this)
        {
            if (instance.m_34player != null)
            {
                m_34player = instance.m_34player;
            }
            if (instance.playerControllerNew != null)
                playerControllerNew = instance.playerControllerNew;
        }

        instance = this;
        if (ConstantsHolder.xanaConstants.IsMuseum)
        {
            foreach (GameObject go in disableObjectsInMuseums)
            {
                go.SetActive(false);
            }
        }
        { // This Patch code also written in onEnable, Running twice 
            //if (WorldItemView.m_EnvName.Contains("AfterParty") || ConstantsHolder.xanaConstants.IsMuseum)
            //{
            //    if (WorldItemView.m_EnvName.Contains("J&J WORLD_5"))
            //    {
            //        if (ConstantsHolder.xanaConstants.minimap == 1)
            //        {
            //            minimap.SetActive(true);
            //        }
            //        minimapSettingsBtn.SetActive(true);
            //    }
            //    else
            //    {
            //        minimap.SetActive(false);
            //        minimapSettingsBtn.SetActive(false);
            //    }
            //}
            //else
            //{
            //    if (ConstantsHolder.xanaConstants.minimap == 1)
            //    {
            //        minimap.SetActive(true);
            //        SumitMapStatus(true);
            //    }
            //    else
            //    {
            //        minimap.SetActive(false); // Disable Minimap Bydefault
            //        SumitMapStatus(false);
            //    }
            //    minimapSettingsBtn.SetActive(true);
            //}
        }
        playerControllerNew = MainPlayerParent.GetComponent<PlayerController>();
    }

    IEnumerator counterCoroutine;
    private void OnEnable()
    {
        instance = this;
        PhotonNetwork.AddCallbackTarget(this);
        if (m_34player == null)
        {
            m_34player = GameplayEntityLoader.instance.player;
        }
        if (WorldItemView.m_EnvName.Contains("Xana Festival")) // for Xana Festival
        {
            //RoomMaxPlayerCount = (ConstantsHolder.xanaConstants.userLimit - 1);
            if (PhotonNetwork.CurrentRoom != null)
            {
                PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) - 1;
            }
        }
        //else if (FeedEventPrefab.m_EnvName.Contains("XANA Lobby"))
        //{
        //    //PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
        //    totalCounter.text = PlayerCount + "/" + (Convert.ToInt32(RoomMaxPlayerCount) + 5);
        //}
        else
        {
            //RoomMaxPlayerCount = ConstantsHolder.xanaConstants.userLimit;
            if (PhotonNetwork.CurrentRoom != null)
                PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
        }
        if (instance != null && instance != this/* && !FeedEventPrefab.m_EnvName.Contains("XANA Lobby")*/)
        {
            if (instance.totalCounter != null)
            {
                totalCounter.text = totalCounter.text = PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers /*ConstantsHolder.xanaConstants.userLimit*/;
            }
        }

        if (ReferenceObject.activeInHierarchy && m_34player != null && !ConstantsHolder.isPenguin)
        {
            if (m_34player.GetComponent<MyBeachSelfieCam>())
            {
                m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRenderPotraiat.SetActive(false);
                m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRender.SetActive(true);
            }

        }
        if (ReferenceObjectPotrait.activeInHierarchy && m_34player != null && !ConstantsHolder.isPenguin)
        {
            if (m_34player.GetComponent<MyBeachSelfieCam>())
            {
                m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRender.SetActive(false);
                m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRenderPotraiat.SetActive(true);
            }
        }
        /*if (counterCoroutine == null)
        {
           SetPlayerCounter();
           // StartCoroutine(counterCoroutine);
        }
        else
        {
           *//* StopCoroutine(counterCoroutine);
            StartCoroutine(counterCoroutine);*//*
        }*/
        SetPlayerCounter();
        if (WorldItemView.m_EnvName.Contains("AfterParty") || ConstantsHolder.xanaConstants.IsMuseum)
        {
            if (WorldItemView.m_EnvName.Contains("J&J WORLD_5"))
            {
                if (ConstantsHolder.xanaConstants.minimap == 1)
                    minimap.SetActive(true);
                else
                    minimap.SetActive(false);
            }
            return;
        }
        else
        {
            if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
            {
                GameplayEntityLoader.instance.ForcedMapCloseForSummitScene();
            }
            else if (ConstantsHolder.xanaConstants.minimap == 1)
            {
                minimap.SetActive(true);
                SumitMapStatus(true);
            }
            else
            {
                minimap.SetActive(false);
                SumitMapStatus(false);
            }
        }
        moveWhileDanceCheck = PlayerPrefs.GetInt("dancebutton"); //add kamran
        if (moveWhileDanceCheck == 0)
        {
            landscapeMoveWhileDancingButton.SetActive(false);
            instance.portraitMoveWhileDancingButton.SetActive(false);
        }
        else
        {
            landscapeMoveWhileDancingButton.SetActive(true);
            instance.portraitMoveWhileDancingButton.SetActive(true);
        }

        isMatchingTimerFinished = false;
    }

    public void ChangeExitBtnImage(bool _Status)
    {
        if (_Status)
        {
            ExitBtnGameplay.sprite = backBtnSprite;
        }
        else
        {
            ExitBtnGameplay.sprite = HomeBtnSprite;
        }
    }
    public void forcetodisable()
    {
        foreach (GameObject go in disableObjects)
        {
            go.SetActive(false);
        }
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.UpdateCanvasForMuseum(false);
    }
    public void forcetoenable()
    {
        foreach (GameObject go in disableObjects)
        {
            go.SetActive(true);
        }
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.UpdateCanvasForMuseum(true);
    }


    public bool isHidebtn = false;

    /// Added by Abdullah Rashid 23/07/05           
    public void hiddenButtonDisable()
    {
        //To Hide Buttons
        foreach (GameObject go in hiddenBtnObjects)
        {
            if (!go.GetComponent<CanvasGroup>())
            {
                go.AddComponent<CanvasGroup>();
            }
            go.GetComponent<CanvasGroup>().alpha = 0;
        }
        ActionManager.DisableCircleDialog?.Invoke();
        //To disable Buttons  
        foreach (GameObject go in disableBtnObjects)
        {
            go.SetActive(false);
        }
    }
    public void hiddenButtonEnable()
    {
        //To Visible Hide Buttons
        foreach (GameObject go in hiddenBtnObjects)
        {
            //go.SetActive(true);
            //if (go.name.Contains("map"))
            //{
            //    if (!ConstantsHolder.xanaConstants.IsMuseum)// && ConstantsHolder.xanaConstants.minimap != 0)
            //        go.SetActive(true); 
            //}
            //else
            {
                if (!go.GetComponent<CanvasGroup>())
                {
                    go.AddComponent<CanvasGroup>();

                }
                go.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        //To enable disable Buttons
        foreach (GameObject go in disableBtnObjects)
        {
            if (go.name.Contains("_Summit") && !ConstantsHolder.xanaConstants.EnviornmentName.Equals("XANA Summit"))
            {
                go.SetActive(false);
            }
            else if (!ConstantsHolder.xanaConstants.IsMuseum && !go.name.Contains("map"))
                go.SetActive(true);
        }

    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void potraithiddenButtonDisable()
    {
        //To Hide potrait Buttons
        foreach (GameObject go in potraitHiddenBtnObjects)
        {

            if (!go.GetComponent<CanvasGroup>())
            {
                go.AddComponent<CanvasGroup>();
            }
            go.GetComponent<CanvasGroup>().alpha = 0;
        }

        //To disable potrait Buttons
        foreach (GameObject go in potraitdissableBtnObjects)
        {
            go.SetActive(false);
        }

    }
    public void potraithiddenButtonEnable()
    {
        //To Visible Hide potrait Buttons
        foreach (GameObject go in potraitHiddenBtnObjects)
        {
            //go.SetActive(true);
            if (go.name.Contains("map"))
            {
                if (!ConstantsHolder.xanaConstants.IsMuseum && ConstantsHolder.xanaConstants.minimap != 0)
                    go.SetActive(true);
            }
            else
            {
                if (!go.GetComponent<CanvasGroup>())
                {
                    go.AddComponent<CanvasGroup>();
                }
                go.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        //To enable disable potrait Buttons
        foreach (GameObject go in potraitdissableBtnObjects)
        {
            go.SetActive(true);
        }

    }

    ////////////////////////////////////

    //private void Start()
    //{
    //    StartCoroutine(SetPlayerCounter());
    //}

    public void SetPlayerCounter()
    {
        try
        {
            if(ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                if (totalCounter != null)
                {
                    if (ConstantsHolder.xanaConstants.isCameraManInRoom || ConstantsHolder.xanaConstants.isCameraMan)
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount - 1);
                    }
                    else
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
                    }
                    totalCounter.text = PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers /*ConstantsHolder.xanaConstants.userLimit*/;

                    if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) + NpcSpawner.npcSpawner.npcCounter;
                        totalCounter.text = PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + 5;
                        
                    }
                }
            }
            else
            {
                if (totalCounter != null)
                {
                    if (ConstantsHolder.xanaConstants.isCameraManInRoom || ConstantsHolder.xanaConstants.isCameraMan)
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount - 1)+SummitAIChatHandler.NPCCount;
                    }
                    else
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) + SummitAIChatHandler.NPCCount;
                    }
                    totalCounter.text = PlayerCount + "/" + (PhotonNetwork.CurrentRoom.MaxPlayers+ SummitAIChatHandler.NPCCount);
                    
                    if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) + NpcSpawner.npcSpawner.npcCounter + SummitAIChatHandler.NPCCount;
                        totalCounter.text = PlayerCount + "/" + (PhotonNetwork.CurrentRoom.MaxPlayers + 5 + SummitAIChatHandler.NPCCount); ;
                    
                    }
                }

                if (((PlayerCount == ConstantsHolder.XanaPartyMaxPlayers && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame) || isMatchingTimerFinished) && !isCounterStarted)
                {  // to check if the room count is full then move all the player randomly form the list of XANA Party Rooms
                    MakeRoomPrivate();
                }
                //else
                //{
                //    PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
                //    totalCounter.text = PlayerCount + "/" + RoomMaxPlayerCount;
                //}
            }
        }
        catch (Exception e)
        {

        }
    }

    #region XANA PARTY WORLD
    public IEnumerator ShowLobbyCounterAndMovePlayer()
    {
        if (XANAPartyWaitingPanel && XANAPartyCounterPanel)
        {
            XANAPartyWaitingPanel.SetActive(false);
            XANAPartyCounterPanel.SetActive(true);
            for (int i = 3; i >= 1; i--)
            {
                XANAPartyCounterText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            XANAPartyCounterPanel.SetActive(false);

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.PenpenzLoading(FadeAction.In));
            yield return new WaitForSeconds(2);
            if (PhotonNetwork.IsMasterClient)
            {
                var xanaPartyMulitplayer = GameplayEntityLoader.instance.PenguinPlayer.GetComponent<XANAPartyMulitplayer>();
                xanaPartyMulitplayer.StartCoroutine(xanaPartyMulitplayer.MovePlayersToRandomGame());
            }
        }
    }

    public void LoadLevel(string levelName)
    {
        XANAPartyManager.Instance.ActivePlayerInCurrentLevel = 0;
        IsLevelPropertyUpdatedOnlevelLoad = false;
        Hashtable props = new Hashtable()
        {
            { InLevelProperty, (levelName+XANAPartyManager.Instance.GameIndex) }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Load the new level
        PhotonNetwork.LoadLevel(levelName);
    }

    public void CheckActivePlayerInCurrentLevel()
    {
        if (GameplayEntityLoader.instance.PenguinPlayer != null && GameplayEntityLoader.instance.PenguinPlayer.GetComponent<PhotonView>().IsMine && !IsLevelPropertyUpdatedOnlevelLoad)
        {
            XANAPartyManager.Instance.ActivePlayerInCurrentLevel = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue(InLevelProperty, out object isInLevel))
                {
                    if (isInLevel != null)
                    {
                        XANAPartyManager.Instance.ActivePlayerInCurrentLevel++;
                    }
                }
            }
            IsLevelPropertyUpdatedOnlevelLoad = true;
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && !GamificationComponentData.instance.isRaceStarted)
                GamificationComponentData.instance.StartXANAPartyRace();
        }
    }

    public void ResetActivePlayerStatusInCurrentLevel()
    {
        Hashtable props = new Hashtable();
        props.Add("IsInLevel", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void ReduceActivePlayerCountInCurrentLevel()
    {
        XANAPartyManager.Instance.ActivePlayerInCurrentLevel--;
    }

    public void MakeRoomPrivate()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    #endregion
    public void SumitMapStatus(bool _status)
    {
        if (_status && ConstantsHolder.xanaConstants.EnviornmentName.Equals("XANA Summit"))
        {
            MinimapSummit.SetActive(true);

            minimap.transform.parent.GetComponent<RawImage>().enabled = true;
            minimap.transform.parent.GetComponent<Mask>().enabled = true;

            /* if (!ScreenOrientationManager._instance.isPotrait)
                 minimap.GetComponent<RectTransform>().sizeDelta = new Vector2(530, 300);*/
        }
        else
        {
            minimap.SetActive(false);
            MinimapSummit.SetActive(false);
        }
    }

    public void FullScreenMapStatus(bool _enable)
    {
        if (ConstantsHolder.DisableFppRotation) { return; }

        if (_enable)
        {
            Input.multiTouchEnabled = false;
        }
        else
        {
            Input.multiTouchEnabled = true;
        }

        FullscreenMapSummit.SetActive(_enable);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetPlayerCounter();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetPlayerCounter();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {

    }

    public void OnCreatedRoom()
    {

    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinedRoom()
    {
        SetPlayerCounter();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {

    }

    public void OnLeftRoom()
    {
       
    }

    public void SetGameplayForPenpenz(bool flag)
    {
        foreach (var item in PenpenzDisableUi)
        {
            item.SetActive(flag);
        }
    }
}


