using Paroxe.PdfRenderer;
using Photon.Pun;
using System.Collections.Generic;
using Toyota;
using UnityEngine;
using UnityEngine.Video;

public class NFT_Holder_Manager : MonoBehaviour
{
    public static NFT_Holder_Manager instance;
    public bool worldPlayingVideos;
    [NonReorderable]
    public List<RatioRef> ratioReferences;

    [Space(10)]
    [Header("PDF Data Refrences")]
    public GameObject pdfPanel_L;
    public GameObject pdfPanel_P;
    public PDFViewer pdfViewer_L;
    public PDFViewer pdfViewer_P;

    [NonReorderable]
    public List<VideoPlayer> VideoPlayers;
    public GameObject LandscapeObj;
    public GameObject PotraiteObj;

    public RenderTexture renderTexture_16x9;
    public RenderTexture renderTexture_9x16;
    public RenderTexture renderTexture_1x1;
    public RenderTexture renderTexture_4x3;
    [Space(5)]
    public AR_Nft_Manager currentRoom;
    [Space(5)]
    public ThaMeetingTxtUpdate meetingTxtUpdate;
    public FB_PushNotificationSender pushNotification;
    public ThaMeetingStatusUpdate meetingStatus;
    public VoiceManager voiceManager;
    public ExtendedXanaChatSystem Extended_XCS;
    public int RatioID;
    public bool IsSummit;
    private XanaChatSystem _chatSystem;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
   
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject meetingObj = Resources.Load("ThaMeetingObj") as GameObject;
            meetingObj = PhotonNetwork.InstantiateRoomObject(meetingObj.name, new Vector3(0f, 0f, 0f), Quaternion.identity);
            meetingStatus = meetingObj.GetComponent<ThaMeetingStatusUpdate>();
            //Debug.LogError("Instantiate Meeting Object");
        }
        else if (meetingStatus == null)
            meetingStatus = FindObjectOfType<ThaMeetingStatusUpdate>();

        SetChatRefrence();

        // send Space_Entry_UniqueUsers_Mobile_App
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.SE_UU_Mobile_App_THA.ToString());
    }
    //private void OnApplicationFocus(bool hasFocus)
    //{
    //    if (!hasFocus)
    //    {
    //        Debug.Log("---Infinity Labo Nft Close due to Application minimized");
    //        CloseBtnClicked();
    //    }

    //}
    public void SetChatRefrence()
    {
        _chatSystem = XanaChatSystem.instance;
        if (Extended_XCS == null)
        {
            Extended_XCS = gameObject.AddComponent<ExtendedXanaChatSystem>();
        }

        Extended_XCS.PotriatCurrentChannelText = _chatSystem.PotriatCurrentChannelText;
        Extended_XCS.CurrentChannelText = _chatSystem.CurrentChannelText;
        Extended_XCS.UserName = _chatSystem.UserName;
        Extended_XCS.chatDialogBox = _chatSystem.chatDialogBox;
        Extended_XCS.chatNotificationIcon = _chatSystem.chatNotificationIcon;
        Extended_XCS.chatButton = _chatSystem.chatButton;
        Extended_XCS.ChatScrollRect = _chatSystem.ChatScrollRect;
        Extended_XCS.InputFieldChat = _chatSystem.InputFieldChat;
    }

    public void GetMeetingObjRef(ThaMeetingStatusUpdate meetingRef)
    {
        meetingStatus = meetingRef;
    }

    public void CloseBtnClicked()
    {
        currentRoom?.CloseInfoPop();
        if (currentRoom != null) currentRoom = null;
        renderTexture_16x9?.Release();
        renderTexture_9x16?.Release();
        renderTexture_1x1?.Release();
        renderTexture_4x3?.Release();

        if (IsSummit)
        {
            ratioReferences[RatioID].l_obj.SetActive(false);
            ratioReferences[RatioID].p_obj.SetActive(false);
            ratioReferences[RatioID].p_Loader.SetActive(false);
            ratioReferences[RatioID].l_Loader.SetActive(false);
            LandscapeObj.SetActive(false);
            PotraiteObj.SetActive(false);
            if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
            {
                GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
            }
        }
    }


    public void PdfClosed()
    {
        currentRoom?.EnableControlls();
        if (currentRoom != null) currentRoom = null;
    }

}
