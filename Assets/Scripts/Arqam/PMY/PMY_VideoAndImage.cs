using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using System.Diagnostics.Eventing.Reader;

public class PMY_VideoAndImage : MonoBehaviour
{
    public enum MyDataType { None, PDF, Quiz};
    public MyDataType myDataType;
    [Space(5)]
    public int id;

    private Texture2D _texture;

    public GameObject imgVideo16x9;
    public GameObject imgVideo9x16;
    public GameObject imgVideo1x1;
    public GameObject imgVideo4x3;

    public GameObject liveVideoPlayer;
    public GameObject preRecordedPlayer;

    public GameObject pdfPanelLanscape;
    public GameObject pdfPanelPortrait;
    public GameObject quizPanelLanscape;
    public GameObject quizPanelPortrait;

    public string videoLink;
    public string imageLink;

    public PMY_VideoTypeRes _videoType;
    public PMY_Ratio _imgVideoRatio;
    [SerializeField] bool ApplyImageOnTexture; // If image is not on Sqaure
    [SerializeField] MeshRenderer imageMesh;
    [SerializeField] Material imageMat;

    [SerializeField] bool applyVideoMesh; // If play video on mesh 
    [SerializeField] VideoPlayer videoMesh;

    public string firebaseEventName = "";
    // Start is called before the first frame update


    public GameObject imgVideoFrame16x9;
    public GameObject imgVideoFrame9x16;
    public GameObject imgVideoFrame1x1;
    public GameObject imgVideoFrame4x3;

    public bool isMultipleScreen = false;

    public enum RoomType
    {
        RoomA_1,
        RoomA_2,
        Gallery
    }
    [Space(5)]
    [Header("For Firebase Enum")]
    public RoomType roomType;
    [Space(5)]
    [Header("For Firebase roomNumber")]
    [Range(0, 12)]
    public int roomNumber = 1;

    private void Start()
    {
        if (myDataType.Equals(MyDataType.PDF))
        {
            imgVideo1x1.AddComponent<Button>();
            imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => Enable_PDF_Panel());
        }
        else if (myDataType.Equals(MyDataType.Quiz))
        {
            imgVideo1x1.AddComponent<Button>();
            imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => EnableQuizPanel());
        }
        else
        {
            imgVideo16x9.AddComponent<Button>();
            imgVideo16x9.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo9x16.AddComponent<Button>();
            imgVideo9x16.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo1x1.AddComponent<Button>();
            imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo4x3.AddComponent<Button>();
            imgVideo4x3.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());
        }
    }


    public void InitData(string imageurl, string videourl, PMY_Ratio imgvideores, PMY_DataType dataType, PMY_VideoTypeRes videoType)
    {
        imageLink = imageurl;
        videoLink = videourl;
        _imgVideoRatio = imgvideores;
        _videoType = videoType;
        if (dataType == PMY_DataType.Image)
            SetImage();
        else if (dataType == PMY_DataType.Video)
            SetVideo();
    }

    void SetImage()
    {
        if (imgVideo16x9)
            imgVideo16x9.SetActive(false);
        if (imgVideo9x16)
            imgVideo9x16.SetActive(false);
        if (imgVideo1x1)
            imgVideo1x1.SetActive(false);
        if (imgVideo4x3)
            imgVideo4x3.SetActive(false);
        if (liveVideoPlayer)
            liveVideoPlayer.SetActive(false);
        if (preRecordedPlayer)
            preRecordedPlayer.SetActive(false);


        StartCoroutine(GetSprite(imageLink, (response) =>
        {
            if (PMY_Nft_Manager.Instance && response != null)
                PMY_Nft_Manager.Instance.NFTLoadedSprites.Add(response);

            if (ApplyImageOnTexture && imageMesh != null)
            {
                imageMesh.material = imageMat;
                imageMesh.material.mainTexture = response;
            }
            else if (_imgVideoRatio == PMY_Ratio.SixteenXNineWithDes || _imgVideoRatio == PMY_Ratio.SixteenXNineWithoutDes)
            {
                if (imgVideo16x9)
                {
                    if (imgVideoFrame16x9)
                    {
                        EnableImageVideoFrame(imgVideoFrame16x9);
                    }
                    imgVideo16x9.SetActive(true);
                    imgVideo16x9.GetComponent<RawImage>().texture = response;
                    imgVideo16x9.GetComponent<VideoPlayer>().enabled = false;
                    if (imgVideo16x9.transform.childCount > 0)
                    {
                        foreach (Transform g in imgVideo16x9.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.NineXSixteenWithDes || _imgVideoRatio == PMY_Ratio.NineXSixteenWithoutDes)
            {
                if (imgVideo9x16)
                {
                    if (imgVideoFrame9x16)
                    {
                        EnableImageVideoFrame(imgVideoFrame9x16);
                    }
                    imgVideo9x16.SetActive(true);
                    imgVideo9x16.GetComponent<RawImage>().texture = response;
                    imgVideo9x16.GetComponent<VideoPlayer>().enabled = false;
                    if (imgVideo9x16.transform.childCount > 0)
                    {
                        foreach (Transform g in imgVideo9x16.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.OneXOneWithDes || _imgVideoRatio == PMY_Ratio.OneXOneWithoutDes)
            {
                if (imgVideo1x1)
                {
                    if (imgVideoFrame1x1)
                    {
                        EnableImageVideoFrame(imgVideoFrame1x1);
                    }
                    imgVideo1x1.SetActive(true);
                    imgVideo1x1.GetComponent<RawImage>().texture = response;
                    imgVideo1x1.GetComponent<VideoPlayer>().enabled = false;
                    if (imgVideo1x1.transform.childCount > 0)
                    {
                        foreach (Transform g in imgVideo1x1.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.FourXThreeWithDes || _imgVideoRatio == PMY_Ratio.FourXThreeWithoutDes)
            {
                if (imgVideo4x3)
                {
                    if (imgVideoFrame4x3)
                    {
                        EnableImageVideoFrame(imgVideoFrame4x3);
                    }
                    imgVideo4x3.SetActive(true);
                    imgVideo4x3.GetComponent<RawImage>().texture = response;
                    imgVideo4x3.GetComponent<VideoPlayer>().enabled = false;
                    if (imgVideo4x3.transform.childCount > 0)
                    {
                        foreach (Transform g in imgVideo4x3.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }));
    }

    void EnableImageVideoFrame(GameObject frameToEnable)
    {
        imgVideoFrame16x9.SetActive(false);
        imgVideoFrame9x16.SetActive(false);
        imgVideoFrame1x1.SetActive(false);
        imgVideoFrame4x3.SetActive(false);

        frameToEnable.SetActive(true);
    }

    public void TurnOffAllImageAndVideo()
    {
        imgVideo16x9.SetActive(false);
        imgVideo9x16.SetActive(false);
        imgVideo1x1.SetActive(false);
        imgVideo4x3.SetActive(false);
        liveVideoPlayer.SetActive(false);
        preRecordedPlayer.SetActive(false);
        imgVideo16x9.SetActive(false);
        imgVideo9x16.SetActive(false);
        imgVideo1x1.SetActive(false);
        imgVideo4x3.SetActive(false);
        liveVideoPlayer.SetActive(false);
        preRecordedPlayer.SetActive(false);
    }

    IEnumerator GetSprite(string path, System.Action<Texture> callback)
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ERror in loading sprite" + www.error);
            }
            else
            {
                if (www.isDone)
                {
                    Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                    _texture = loadedTexture;
                    //var rect = new Rect(1, 1, 1, 1);
                    //thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    //Texture2D tempTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    //Sprite sprite = /*Sprite.Create(tempTex, rect, new Vector2(0.5f, 0.5f))*/ Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    //print("Texture is " + sprite);
                    callback(_texture);
                }
            }
            www.Dispose();
        }
    }

    RenderTexture renderTexture_temp;
    void SetVideo()
    {
        if (imgVideo16x9)
            imgVideo16x9.SetActive(false);
        if (imgVideo9x16)
            imgVideo9x16.SetActive(false);
        if (imgVideo1x1)
            imgVideo1x1.SetActive(false);
        if (imgVideo4x3)
            imgVideo4x3.SetActive(false);
        if (liveVideoPlayer)
            liveVideoPlayer.SetActive(false);
        if (preRecordedPlayer)
            preRecordedPlayer.SetActive(false);

        if (_videoType == PMY_VideoTypeRes.islive && liveVideoPlayer)
        {
            PMY_Nft_Manager.Instance.videoRenderObject = liveVideoPlayer;
            if (liveVideoPlayer)
                liveVideoPlayer.SetActive(true);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
            SoundManager.Instance.livePlayerSource = liveVideoPlayer.GetComponent<MediaPlayer>();
            SoundManagerSettings.soundManagerSettings.setNewSliderValues();
        }
        else if (_videoType == PMY_VideoTypeRes.prerecorded && preRecordedPlayer)
        {
            RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_16x9); 
            SoundManager.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
            SoundManagerSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
            SoundManagerSettings.soundManagerSettings.setNewSliderValues();
            PMY_Nft_Manager.Instance.videoRenderObject = imgVideo16x9;
            renderTexture_temp = renderTexture;
            imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
            imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
            if (isMultipleScreen)
            {
                for (int i = 0; i < imgVideo16x9.transform.childCount; i++)
                {
                    imgVideo16x9.transform.GetChild(i).GetComponent<RawImage>().texture = renderTexture;
                    imgVideo16x9.transform.GetChild(i).GetComponent<VideoPlayer>().targetTexture = renderTexture;
                }
            }
            preRecordedPlayer.GetComponent<YoutubeSimplified>().player.showThumbnailBeforeVideoLoad = false;
            VideoPlayer tempVideoPlayer;
            if (applyVideoMesh)
            {
                tempVideoPlayer = videoMesh;
            }
            else
            {
                tempVideoPlayer = imgVideo16x9.GetComponent<VideoPlayer>();
            }

            preRecordedPlayer.SetActive(true);
            preRecordedPlayer.GetComponent<YoutubeSimplified>().videoPlayer = tempVideoPlayer;
            preRecordedPlayer.GetComponent<YoutubeSimplified>().player.videoPlayer = tempVideoPlayer;
            preRecordedPlayer.GetComponent<YoutubeSimplified>().player.audioPlayer = tempVideoPlayer;
            preRecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
            preRecordedPlayer.GetComponent<YoutubeSimplified>().Play();
            imgVideo16x9.GetComponent<VideoPlayer>().playOnAwake = true;
            imgVideo16x9.SetActive(true);
            if (imgVideoFrame16x9)
            {
                EnableImageVideoFrame(imgVideoFrame16x9);
            }
        }
        else if (_videoType == PMY_VideoTypeRes.aws)
        {
            if (_imgVideoRatio == PMY_Ratio.SixteenXNineWithDes || _imgVideoRatio == PMY_Ratio.SixteenXNineWithoutDes)
            {
                if (imgVideo16x9)
                {
                    if (imgVideoFrame16x9)
                    {
                        EnableImageVideoFrame(imgVideoFrame16x9);
                    }
                    imgVideo16x9.SetActive(true);
                    imgVideo16x9.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo16x9.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_16x9);
                    renderTexture_temp = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                    imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo16x9.GetComponent<VideoPlayer>().Play();
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.NineXSixteenWithDes || _imgVideoRatio == PMY_Ratio.NineXSixteenWithoutDes)
            {
                if (imgVideo9x16)
                {
                    if (imgVideoFrame9x16)
                    {
                        EnableImageVideoFrame(imgVideoFrame9x16);
                    }
                    imgVideo9x16.SetActive(true);
                    imgVideo9x16.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo9x16.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_9x16);
                    renderTexture_temp = renderTexture;
                    imgVideo9x16.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                    imgVideo9x16.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo9x16.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo9x16.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo9x16.GetComponent<VideoPlayer>().Play();
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.OneXOneWithDes || _imgVideoRatio == PMY_Ratio.OneXOneWithoutDes)
            {
                if (imgVideo1x1)
                {
                    if (imgVideoFrame1x1)
                    {
                        EnableImageVideoFrame(imgVideoFrame1x1);
                    }
                    imgVideo1x1.SetActive(true);
                    imgVideo1x1.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo1x1.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_1x1);
                    renderTexture_temp = renderTexture;
                    imgVideo1x1.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                    imgVideo1x1.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo1x1.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo1x1.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo1x1.GetComponent<VideoPlayer>().Play();
                }
            }
            else if (_imgVideoRatio == PMY_Ratio.FourXThreeWithDes || _imgVideoRatio == PMY_Ratio.FourXThreeWithoutDes)
            {
                if (imgVideo4x3)
                {
                    if (imgVideoFrame4x3)
                    {
                        EnableImageVideoFrame(imgVideoFrame4x3);
                    }
                    imgVideo4x3.SetActive(true);
                    imgVideo4x3.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo4x3.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_4x3);
                    renderTexture_temp = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                    imgVideo4x3.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo4x3.GetComponent<VideoPlayer>().Play();
                }
            }
        }

        if (PMY_Nft_Manager.Instance && renderTexture_temp != null)
            PMY_Nft_Manager.Instance.NFTLoadedVideos.Add(renderTexture_temp);
    }

    public void OpenWorldInfo()
    {
        if (SelfieController.Instance.m_IsSelfieFeatureActive) return;
        if (PlayerControllerNew.isJoystickDragging == true)
            return;
        
        if (PMY_Nft_Manager.Instance != null && _videoType != PMY_VideoTypeRes.islive)
        {
            if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
            {
                PMY_Nft_Manager.Instance.SetInfo(_imgVideoRatio, PMY_Nft_Manager.Instance.worldInfos[id].Title[0], PMY_Nft_Manager.Instance.worldInfos[id].Aurthor[0], PMY_Nft_Manager.Instance.worldInfos[id].Des[0], PMY_Nft_Manager.Instance.worldInfos[id].url, _texture, PMY_Nft_Manager.Instance.worldInfos[id].Type, PMY_Nft_Manager.Instance.worldInfos[id].VideoLink, PMY_Nft_Manager.Instance.worldInfos[id].videoType, id, roomType, roomNumber);
            }
            else if (CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                PMY_Nft_Manager.Instance.SetInfo(_imgVideoRatio, PMY_Nft_Manager.Instance.worldInfos[id].Title[1], PMY_Nft_Manager.Instance.worldInfos[id].Aurthor[1], PMY_Nft_Manager.Instance.worldInfos[id].Des[1], PMY_Nft_Manager.Instance.worldInfos[id].url, _texture, PMY_Nft_Manager.Instance.worldInfos[id].Type, PMY_Nft_Manager.Instance.worldInfos[id].VideoLink, PMY_Nft_Manager.Instance.worldInfos[id].videoType, id, roomType, roomNumber);

            }
        }
    }

    private void EnableQuizPanel()
    {
        if (!ChangeOrientation_waqas._instance.isPotrait)
            quizPanelLanscape.SetActive(true);
        else
            quizPanelPortrait.SetActive(true);
    }
    private void Enable_PDF_Panel()
    {
        if (!ChangeOrientation_waqas._instance.isPotrait)
            pdfPanelLanscape.SetActive(true);
        else
            pdfPanelPortrait.SetActive(true);   
    }

}
