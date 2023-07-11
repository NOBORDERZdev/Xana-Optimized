using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class JjInfoManager : MonoBehaviour
{
    [NonReorderable]
    public List<RatioReferences> ratioReferences;
    [NonReorderable]
    [SerializeField] List<VideoPlayer> VideoPlayers;
    [SerializeField] GameObject LandscapeObj;
    [SerializeField] GameObject PotraiteObj;

    [NonReorderable]
    public List<JJWorldInfo> worldInfos;
    public static JjInfoManager Instance { get; private set; }
    
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] int RetryChances = 3;
    int ratioId;
    int videoRetry=0;

    JjRatio _Ratio;
    string _Title;
    string _Aurthor;
    string _Des;
    Sprite _image;
    DataType _Type;
    string _VideoLink;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        foreach (VideoPlayer player in VideoPlayers)
        {
            player.errorReceived += ErrorOnVideo;
            player.prepareCompleted += VideoReady;
        }
    }


    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, Sprite image, DataType type, string videoLink)
    {
        _Ratio = ratio;
        _Title = title;
        _Aurthor = aurthur;
        _Des = des;
        _image = image;
        _Type = type;
        _VideoLink = videoLink;

        ratioId = ((int)ratio);
        renderTexture.Release();
        // Setting Landscape Data
        ratioReferences[ratioId].l_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
        ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
        ratioReferences[ratioId].l_Title.text = title;
        ratioReferences[ratioId].l_Aurthur.text = aurthur;
        ratioReferences[ratioId].l_Description.text = des;
        if (type == DataType.Image)
        {
            ratioReferences[ratioId].l_image.sprite = image;
            ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            ratioReferences[ratioId].l_image.gameObject.SetActive(false);
            ratioReferences[ratioId].l_videoPlayer.url = videoLink;
        }

        // Setting Potraite Data
        ratioReferences[ratioId].p_Title.text = title;
        ratioReferences[ratioId].p_Aurthur.text = aurthur;
        ratioReferences[ratioId].p_Description.text = des;
        ratioReferences[ratioId].p_image.sprite = image;
        if (type == DataType.Image)
        {
            ratioReferences[ratioId].p_image.sprite = image;
            ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            ratioReferences[ratioId].p_image.gameObject.SetActive(false);
            ratioReferences[ratioId].p_videoPlayer.url = videoLink;
        }
        if (!ChangeOrientation_waqas._instance.isPotrait) // for Landscape
        {
            LandscapeObj.SetActive(true);
            PotraiteObj.SetActive(false);
            ratioReferences[ratioId].l_obj.SetActive(true);
            ratioReferences[ratioId].p_obj.SetActive(false);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].l_Loader.SetActive(true);
                ratioReferences[ratioId].p_Loader.SetActive(false);
                ratioReferences[ratioId].l_videoPlayer.Play();
            }
        }
        else // for Potraite
        {
            LandscapeObj.SetActive(false);
            PotraiteObj.SetActive(true);
            ratioReferences[ratioId].l_obj.SetActive(false);
            ratioReferences[ratioId].p_obj.SetActive(true);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].l_Loader.SetActive(false);
                ratioReferences[ratioId].p_Loader.SetActive(true);
                ratioReferences[ratioId].p_videoPlayer.Play();
            }
        }

        CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(false);
        // infoParent.SetActive(true);
    }

    public void CloseInfoPop()
    {
        ratioReferences[ratioId].l_obj.SetActive(false);
        ratioReferences[ratioId].p_obj.SetActive(false);
        ratioReferences[ratioId].p_Loader.SetActive(false);
        ratioReferences[ratioId].l_Loader.SetActive(false); 
        LandscapeObj.SetActive(false);
        PotraiteObj.SetActive(false);
        CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(true);
    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        if (videoRetry <= RetryChances)
        {
            videoRetry++;
            SetInfo(_Ratio,_Title,_Aurthor,_Des,_image,_Type,_VideoLink);
        }
        else
        {
            videoRetry = 0;
            CloseInfoPop();
        }
    }
    private void VideoReady(VideoPlayer source)
    {
        ratioReferences[ratioId].p_Loader.SetActive(false);
        ratioReferences[ratioId].l_Loader.SetActive(false);
        videoRetry = 0;
    }

    private void OnDisable()
    {
        foreach (VideoPlayer player in VideoPlayers)
        {
            player.errorReceived -= ErrorOnVideo;
            player.prepareCompleted -= VideoReady;
        }
    }
}

[Serializable]
public class JJWorldInfo {
    public string[] Title;
    public string[] Aurthor;
    public string[] Des;
    public DataType Type;
    public Sprite WorldImage;
    public string VideoLink;
}

public enum DataType { 
    Image,
    Video
}

public enum JjRatio { 
    OneXOneWithDes,
    SixteenXNineWithDes,
    NineXSixteenWithDes,
    FourXThreeWithDes,

    OneXOneWithoutDes,
    SixteenXNineWithoutDes,
    NineXSixteenWithoutDes,
    FourXThreeWithoutDes,
    //OneXOneWithDesPotraite,
    //SixteenXNineWithDesPotraite,
    //NineXSixteenWithDesPotraite,
    //FourXThreeWithDesPotraite,
    //OneXOneWithoutDesPotraite,
    //SixteenXNineWithoutDesPotraite,
    //NineXSixteenWithoutDesPotraite,
    //FourXThreeWithoutDesPotraite,
}

[Serializable]
public class RatioReferences
{
    public string name;

    public GameObject l_obj;
    public TMP_Text l_Title;
    public TMP_Text l_Aurthur;
    public TMP_Text l_Description;
    public Image l_image;
    public VideoPlayer l_videoPlayer;
    public GameObject l_Loader;

    public GameObject p_obj;
    public TMP_Text p_Title;
    public TMP_Text p_Aurthur;
    public TMP_Text p_Description;
    public Image p_image;
    public VideoPlayer p_videoPlayer;
    public GameObject p_Loader;
}