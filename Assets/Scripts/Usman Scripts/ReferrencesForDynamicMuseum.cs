using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferrencesForDynamicMuseum : MonoBehaviour
{
    public GameObject eventSystemObj;
    public GameObject[] overlayPanels;
    public GameObject workingCanvas, PlayerParent, MainPlayerParent;
    public GameObject[] disableObjects;
    public GameObject[] potraitHiddenBtnObjects , potraitdissableBtnObjects;
    public GameObject[] hiddenBtnObjects , disableBtnObjects;
    public static ReferrencesForDynamicMuseum instance;
    public Camera randerCamera;
    public List<GameObject> disableObjectsInMuseums;
    public GameObject onBtnUsername;
    public GameObject offBtnUsername;

    public GameObject m_34player;
    public PlayerControllerNew playerControllerNew;
    public GameObject minimap;
    public GameObject minimapSettingsBtn;
    public TMPro.TextMeshProUGUI totalCounter; // Counter to show total connected peoples.
    public GameObject ReferenceObject;
    public GameObject ReferenceObjectPotrait;
    public GameObject FirstPersonCam;
    public Button RotateBtn;
    public GameObject JoyStick;
    public int RoomMaxPlayerCount=0;
    public int PlayerCount = 0;
    public float MonitorDistance;
    //MoveWhileDancing add kamran
    public GameObject landscapeMoveWhileDancingButton;
    public GameObject portraitMoveWhileDancingButton;
    public int moveWhileDanceCheck;

    // Start is called before the first frame update
    private void CustomAwake()
    {

        if (instance != null && instance != this)
        {
            if (instance.m_34player != null)
            {
                m_34player = instance.m_34player;
            }
        }

        instance = this;
        if (XanaConstants.xanaConstants.IsMuseum)
        {
            foreach (GameObject go in disableObjectsInMuseums)
            {
                go.SetActive(false);
            }
        }
        if (WorldItemView.m_EnvName.Contains("AfterParty") || XanaConstants.xanaConstants.IsMuseum)
        {
            if (WorldItemView.m_EnvName.Contains("J&J WORLD_5"))
            {
                if (XanaConstants.xanaConstants.minimap == 1)
                {
                    minimap.SetActive(true);
                }
                minimapSettingsBtn.SetActive(true);
            }
            else
            {
                minimap.SetActive(false);
                minimapSettingsBtn.SetActive(false);
            }
        }
        else
        {
            if (XanaConstants.xanaConstants.minimap == 1)
            {
                minimap.SetActive(true);
            }
            else
            {
                minimap.SetActive(false); // Disable Minimap Bydefault
            }
            minimapSettingsBtn.SetActive(true);
        }
        playerControllerNew = MainPlayerParent.GetComponent<PlayerControllerNew>();
    }

    IEnumerator counterCoroutine;
    private void OnEnable()
    {
        CustomAwake();
        if (WorldItemView.m_EnvName.Contains("Xana Festival")) // for Xana Festival
        {
            RoomMaxPlayerCount = Convert.ToInt32(XanaConstants.xanaConstants.userLimit)-1 ;
            if (PhotonNetwork.CurrentRoom!=null)
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
            RoomMaxPlayerCount = Convert.ToInt32(XanaConstants.xanaConstants.userLimit);
            if (PhotonNetwork.CurrentRoom != null)
                PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
        }
        if (instance != null && instance != this/* && !FeedEventPrefab.m_EnvName.Contains("XANA Lobby")*/)
        {
            if (instance.totalCounter != null)
            {
                totalCounter.text = totalCounter.text = PlayerCount + "/" + RoomMaxPlayerCount /*XanaConstants.xanaConstants.userLimit*/;
            }
        }

        instance = this;
        print("Waqas : Reference : Instance : " + instance);

        if (ReferenceObject.activeInHierarchy && m_34player != null)
        {
            Debug.Log("call hua texture Landscap");
            m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRender.SetActive(true);
            m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRenderPotraiat.SetActive(false);
        }


        if (ReferenceObjectPotrait.activeInHierarchy && m_34player != null)
        {
            Debug.Log("call hua texture potriat");
            m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRender.SetActive(false);
            m_34player.GetComponent<MyBeachSelfieCam>().SelfieCapture_CamRenderPotraiat.SetActive(true);
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

        if (WorldItemView.m_EnvName.Contains("AfterParty") || XanaConstants.xanaConstants.IsMuseum)
        {
            if (WorldItemView.m_EnvName.Contains("J&J WORLD_5"))
            {
                if (XanaConstants.xanaConstants.minimap == 1)
                    ReferrencesForDynamicMuseum.instance.minimap.SetActive(true);
                else
                    ReferrencesForDynamicMuseum.instance.minimap.SetActive(false);
            }
            return;
        }
        else
        {
            if (XanaConstants.xanaConstants.minimap == 1)
                ReferrencesForDynamicMuseum.instance.minimap.SetActive(true);
            else
                ReferrencesForDynamicMuseum.instance.minimap.SetActive(false);
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
            if (go.name.Contains("map"))
            {
                if (!XanaConstants.xanaConstants.IsMuseum && XanaConstants.xanaConstants.minimap != 0)
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

        //To enable disable Buttons
        foreach (GameObject go in disableBtnObjects)
        {
           
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
                if (!XanaConstants.xanaConstants.IsMuseum && XanaConstants.xanaConstants.minimap != 0)
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
                    if (XanaConstants.xanaConstants.isCameraManInRoom || XanaConstants.xanaConstants.isCameraMan)
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount-1);
                    }
                    else
                    {
                        PlayerCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.PlayerCount);
                    }
                totalCounter.text = PlayerCount + "/" + RoomMaxPlayerCount /*XanaConstants.xanaConstants.userLimit*/;
                    
                    //if (XanaConstants.xanaConstants.isCameraMan)
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
                    totalCounter.text = PlayerCount + "/" + (Convert.ToInt32(RoomMaxPlayerCount) +5);
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
   
}


