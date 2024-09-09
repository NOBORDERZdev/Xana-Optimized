using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferencesForGamePlay : MonoBehaviour
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

        if(m_34player==null)
        {
            m_34player=GameplayEntityLoader.instance.player;
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
            if(m_34player.GetComponent<MyBeachSelfieCam>())
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
        if (counterCoroutine == null)
        {
            counterCoroutine = SetPlayerCounter();
            StartCoroutine(counterCoroutine);
        }
        else
        {
            StopCoroutine(counterCoroutine);
            StartCoroutine(counterCoroutine);
        }

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
            if (ConstantsHolder.xanaConstants.minimap == 1)
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

    IEnumerator SetPlayerCounter()
    {
    CheckAgain:
        try
        {
            if (totalCounter != null)
            {
                if (/*FeedEventPrefab.m_EnvName.Contains("Xana Festival")*/ true) // for Xana Festival
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

                    //if (ConstantsHolder.xanaConstants.isCameraMan)
                    //{
                    //    print("NAMES ARE " + PhotonNetwork.CurrentRoom.Name);
                    //    if (false)
                    //    {
                    //        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) - 2;

                    //    }
                    //    else
                    //    {
                    //        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) - 1;
                    //    }
                    //}
                    //else
                    //{
                    //    PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
                    //}
                    // print("!!! PlayerCount"+ PlayerCount);
                }
                if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                {
                    PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount) + NpcSpawner.npcSpawner.npcCounter;
                    totalCounter.text = PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + 5;
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

        yield return new WaitForSeconds(2f);
        goto CheckAgain;
    }
    public void SumitMapStatus(bool _status)
    {
        if (_status && ConstantsHolder.xanaConstants.EnviornmentName.Equals("XANA Summit"))
        {
            MinimapSummit.SetActive(true);

            minimap.transform.parent.GetComponent<RawImage>().enabled = true;
            minimap.transform.parent.GetComponent<Mask>().enabled = true;

            if (!ScreenOrientationManager._instance.isPotrait)
                minimap.GetComponent<RectTransform>().sizeDelta = new Vector2(530, 300);
        }
        else
        {
            MinimapSummit.SetActive(false);
        }
    }

    public void FullScreenMapStatus (bool _enable)
    {
        if(ConstantsHolder.DisableFppRotation) { return; }

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
}


