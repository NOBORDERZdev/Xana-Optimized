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
    public ThaMeetingStatusUpdate meetingStatus;

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
            GameObject meetingObj = GameplayEntityLoader.instance.SpawnThaMeetingObject();
            meetingStatus = meetingObj.GetComponent<ThaMeetingStatusUpdate>();
            Debug.LogError("Instantiate Meeting Object");
        }
        else if (meetingStatus == null)
            meetingStatus = FindObjectOfType<ThaMeetingStatusUpdate>();
    }

    public void CloseBtnClicked()
    {
        currentRoom.CloseInfoPop();
        if (currentRoom != null) currentRoom = null;

        renderTexture_16x9.Release();
        renderTexture_9x16.Release();
        renderTexture_1x1.Release();
        renderTexture_4x3.Release();
    }

    public void PdfClosed()
    {
        currentRoom.EnableControlls();
        if (currentRoom != null) currentRoom = null;
    }

}
