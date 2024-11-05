using AIFLogger;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class SummitDomeNFTDataController : MonoBehaviour
{
    public static SummitDomeNFTDataController Instance { get; private set; }
    [NonReorderable]
    public List<RatioReferences> ratioReferences;
    [NonReorderable]
    public List<JJWorldInfo> worldInfos;
    [NonReorderable]
    public List<GameObject> NftPlaceholder;
    public RenderTexture renderTexture_16x9;
    public RenderTexture renderTexture_9x16;
    public RenderTexture renderTexture_1x1;
    public RenderTexture renderTexture_4x3;
    public string nftTitle;
    public string firebaseEventName;
    public int clickedNftInd;
    public List<Texture> NFTLoadedSprites = new List<Texture>();
    public List<RenderTexture> NFTLoadedVideos = new List<RenderTexture>();
    public GameObject videoRenderObject;
    public AudioSource videoPlayerSource;
    public MediaPlayer livePlayerSource;
    public int clRoomId;
    public string roomName;
    public NFTFromServer NFTDataFetchScrptRef;
    public GalleryImageManager NFTDataHandlerScrptRef;
    public DomeNFTDataArray DoomNFTData;
    public int RoomCount;
    public System.Action VideoOpened;
    public System.Action NFTClosed;

    [SerializeField] bool worldPlayingVideos;
    [SerializeField] int RetryChances = 3;
    [NonReorderable]
    [SerializeField] List<VideoPlayer> VideoPlayers;
    [SerializeField] GameObject LandscapeObj;
    [SerializeField] GameObject PotraiteObj;
    [SerializeField] int WidthParam = 256;
    [SerializeField]
    List<DomeNFTData> AllS3Nfts = new List<DomeNFTData>();
    int ratioId;
    int videoRetry = 0;
    JjRatio _Ratio;
    string _Title;
    string _Aurthor;
    string _Des;
    string _URL;
    Texture _image;
    DataType _Type;
    string _VideoLink;
    VideoTypeRes _videoType;
    bool isNFTUploaded = false;
    private string _singleDomeMusuemApi = "/domes/getDomecontent/";
    string _mussuemLink;

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
        if (VideoPlayers.Count > 0)
        {
            foreach (VideoPlayer player in VideoPlayers)
            {
                player.errorReceived += ErrorOnVideo;
                player.prepareCompleted += VideoReady;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (NFTDataFetchScrptRef && NFTDataFetchScrptRef.dynamicManager)
        {
            RoomCount = NFTDataFetchScrptRef.dynamicManager.rooms.Count;
        }
        else
        {
            RoomCount = 4;
        }
        _mussuemLink = _singleDomeMusuemApi + ConstantsHolder.domeId;
        Invoke(nameof(GetNFTDataDetails), 1f);
    }

    public void GetNFTDataDetails()
    {
        //if (!NFTDataFetchScrptRef.isDynamicMuseum) // is met gallery
        //{
        //    //StartCoroutine(GetNftData());
        //}
        //else // is dynamic museum
        //{
        StartCoroutine(GetNFTDatAForDynamicMuseum());
        //}
    }

    public IEnumerator InitData(DomeNFTDataArray data, List<GameObject> NftPlaceholderList)
    {
        int nftPlaceHolder = data.getcontentbyDomeId.Count;
        List<JjAsset> worldData = new List<JjAsset>(new JjAsset[data.getcontentbyDomeId.Count]);
        for (int i = 0; i < nftPlaceHolder; i++)
        {
            isNFTUploaded = false;
            // if(worldData.Count > i ){
            // int tempIndex = worldData[i].index-1;
            for (int j = 0; j < worldData.Count; j++)
            {
                worldData[j] = new JjAsset();
                InitJJAssetObjtData(worldData[j], data.getcontentbyDomeId[j]);
                if (i == worldData[j].index - 1)
                {
                    isNFTUploaded = true;

                    bool isWithDes = false;
                    string compersionPrfex = "";
                    if (data.width != 0)
                    {
                        compersionPrfex = "?width=" + data.width;
                        WidthParam = data.width;
                    }
                    else
                    {
                        compersionPrfex = "?width=" + WidthParam;  //"?width=500&height=600";
                    }
                    switch (worldData[j].ratio)
                    {
                        case "1:1":
                            if (JJFrameManager.instance)
                                JJFrameManager.instance.SetTransformForFrameSpotLight(0);
                            worldInfos[i].JjRatio = JjRatio.OneXOneWithDes;
                            //compersionPrfex = "?width=512&height=512";
                            if (NFTDataFetchScrptRef._ExhibitComponentType.Equals(NFTFromServer.ExhibitComponentType.Dynamic))
                            {
                                NftPlaceholderList[i].transform.localScale = NFTDataFetchScrptRef._ExhibitSize.square;
                            }
                            break;
                        case "16:9":
                            if (JJFrameManager.instance)
                                JJFrameManager.instance.SetTransformForFrameSpotLight(1);
                            worldInfos[i].JjRatio = JjRatio.SixteenXNineWithDes;
                            //compersionPrfex = "?width=800&height=450";//"?width=500&height=600";
                            if (NFTDataFetchScrptRef._ExhibitComponentType.Equals(NFTFromServer.ExhibitComponentType.Dynamic))
                            {
                                NftPlaceholderList[i].transform.localScale = NFTDataFetchScrptRef._ExhibitSize.landscape;
                            }
                            break;
                        case "9:16":
                            if (JJFrameManager.instance)
                                JJFrameManager.instance.SetTransformForFrameSpotLight(2);
                            worldInfos[i].JjRatio = JjRatio.NineXSixteenWithDes;
                            //compersionPrfex = "?width=450&height=800"; //"?width=700&height=500";
                            if (NFTDataFetchScrptRef._ExhibitComponentType.Equals(NFTFromServer.ExhibitComponentType.Dynamic))
                            {
                                NftPlaceholderList[i].transform.localScale = NFTDataFetchScrptRef._ExhibitSize.potrait;
                            }
                            break;
                        case "4:3":
                            if (JJFrameManager.instance)
                                JJFrameManager.instance.SetTransformForFrameSpotLight(3);
                            worldInfos[i].JjRatio = JjRatio.FourXThreeWithDes;
                            //compersionPrfex = "?width=640&height=480";
                            break;
                        default:
                            if (JJFrameManager.instance)
                                JJFrameManager.instance.SetTransformForFrameSpotLight(0);
                            worldInfos[i].JjRatio = JjRatio.OneXOneWithDes;
                            //compersionPrfex = "?width=512&height=512";
                            break;
                    }
                    NftPlaceholderList[i].SetActive(true);
                    if (worldData[j].media_type == "IMAGE")
                    {

                        worldInfos[i].Type = DataType.Image;
                        NftPlaceholderList[i].name = i.ToString();
                        NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().id = i;
                        NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().isCreateFrame = true;
                        NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().InitData(worldData[j].asset_link + compersionPrfex, null, worldInfos[i].JjRatio, DataType.Image, VideoTypeRes.none);
                        //if (!string.IsNullOrEmpty(worldData[j].title[0]) && !string.IsNullOrEmpty(worldData[j].authorName[0]) && !string.IsNullOrEmpty(worldData[j].description[0]))
                        //{
                        isWithDes = true;
                        worldInfos[i].Title = worldData[j].title;
                        worldInfos[i].Aurthor = worldData[j].authorName;
                        worldInfos[i].Des = worldData[j].description;
                        worldInfos[i].url = worldData[j].descriptionHyperlink;
                        //}
                        //else
                        //{
                        //    isWithDes = false;
                        //    worldInfos[i].Title = null;
                        //    worldInfos[i].Aurthor = null;
                        //    worldInfos[i].Des = null;
                        //}

                    }
                    else if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                    {
                        worldInfos[i].Type = DataType.Video;
                        if (worldPlayingVideos) // to play video's in world
                        {
                            Debug.Log("Dome Id" + worldData[j].id + "is youtube " + worldData[j].youtubeUrlCheck);
                            NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().isCreateFrame = true;
                            if (worldData[j].PrercrdOrLiveURL == "Live" && !string.IsNullOrEmpty(worldData[j].youtubeUrl) && worldData[j].youtubeUrlCheck)  //for Live Video 
                            {
                                yield return new WaitForSeconds(1f);
                                worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                worldInfos[i].videoType = VideoTypeRes.islive;
                                NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().InitData(worldData[j].thumbnail + compersionPrfex, worldData[j].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.islive);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].youtubeUrl;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }
                            else if (worldData[j].PrercrdOrLiveURL == "Prerecorded" && !string.IsNullOrEmpty(worldData[j].youtubeUrl)&& worldData[j]. youtubeUrlCheck)  // for Prerecorded video
                            {
                                yield return new WaitForSeconds(1f);
                                worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                worldInfos[i].videoType = VideoTypeRes.prerecorded;
                                NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().InitData(worldData[j].thumbnail + compersionPrfex, worldData[j].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.prerecorded);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].youtubeUrl;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }
                            else if (!string.IsNullOrEmpty(worldData[j].asset_link))
                            {
                                worldInfos[i].VideoLink = worldData[j].asset_link;
                                worldInfos[i].videoType = VideoTypeRes.aws;
                                NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().InitData(worldData[j].thumbnail + compersionPrfex, worldData[j].asset_link + compersionPrfex, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.aws);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].asset_link;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }
                            //if (!string.IsNullOrEmpty(worldData[j].title[0]) && !string.IsNullOrEmpty(worldData[j].authorName[0]) && !string.IsNullOrEmpty(worldData[j].description[0]))
                            //{
                            isWithDes = true;
                            worldInfos[i].Title = worldData[j].title;
                            worldInfos[i].Aurthor = worldData[j].authorName;
                            worldInfos[i].Des = worldData[j].description;
                            worldInfos[i].url = worldData[j].descriptionHyperlink;
                            //}
                            //else
                            //{
                            //    isWithDes = false;
                            //    worldInfos[i].Title = null;
                            //    worldInfos[i].Aurthor = null;
                            //    worldInfos[i].Des = null;
                            //}
                        }
                    }
                    if (NFTDataFetchScrptRef)
                    {
                        if (i >= NFTDataFetchScrptRef.spawnPoints.Count) break;

                        NftPlaceholderList[i].transform.position = new Vector3(NFTDataFetchScrptRef.spawnPoints[j].transform.position.x,
    (NFTDataFetchScrptRef.spawnPoints[j].transform.position.y + 0.72f),
    NFTDataFetchScrptRef.spawnPoints[j].transform.position.z);
                        NftPlaceholderList[i].transform.eulerAngles = NFTDataFetchScrptRef.spawnPoints[j].transform.eulerAngles;
                    }
                    else if (NFTDataHandlerScrptRef)
                    {
                        NftPlaceholderList[i].transform.position = new Vector3(NFTDataHandlerScrptRef.NFTSpawnPoints[j].transform.position.x,
(NFTDataHandlerScrptRef.NFTSpawnPoints[j].transform.position.y + 0.72f),
NFTDataHandlerScrptRef.NFTSpawnPoints[j].transform.position.z);
                        NftPlaceholderList[i].transform.eulerAngles = NFTDataHandlerScrptRef.NFTSpawnPoints[j].transform.eulerAngles;
                    }
                    break;
                }
                else
                {
                    if (j == worldData.Count - 1)
                    {
                        NftPlaceholderList[i].gameObject.SetActive(false);
                        NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().TurnOffAllImageAndVideo();
                        //Debug.Log("INDEX is off!");
                    }
                }
            }
            if (!isNFTUploaded)
            {
                NftPlaceholderList[i].gameObject.SetActive(false);
                NftPlaceholderList[i].GetComponent<SummitVideoAndImageController>().TurnOffAllImageAndVideo();
            }
            //}
            // else
            //{
            //   _nftPlaceHolderPrefab.gameObject.SetActive(false);
            //   _nftPlaceHolderPrefab.GetComponent<SummitVideoAndImageController>().TurnOffAllImageAndVideo();
            //}
        }

    }

    public void InitJJAssetObjtData(JjAsset _jjAssetObj, DomeNFTData _domeNFTDataObj)
    {
        //Debug.Log(_domeNFTDataObj.id);
        //Debug.Log(_jjAssetObj.id);
        _jjAssetObj.id = _domeNFTDataObj.id;
        _jjAssetObj.index = _domeNFTDataObj.index;
        _jjAssetObj.check = true;

        string creatorName = ConstantsHolder.xanaConstants.domeCreatorName;
        if(string.IsNullOrEmpty(creatorName))
        {
            creatorName = "XANA";
        }
        _jjAssetObj.authorName = new string[] { creatorName };
        _jjAssetObj.description = new string[] { _domeNFTDataObj.description, _domeNFTDataObj.jpDescription };
        _jjAssetObj.descriptionHyperlink = "";
        _jjAssetObj.title = new string[] { _domeNFTDataObj.name, _domeNFTDataObj.jpName };
        _jjAssetObj.ratio = _domeNFTDataObj.proportionType;
        _jjAssetObj.asset_link = _domeNFTDataObj.thumbnail;
        _jjAssetObj.PrercrdOrLiveURL = _domeNFTDataObj.videoType;
        _jjAssetObj.youtubeUrlCheck = _domeNFTDataObj.isYoutubeUrl;
        if (_domeNFTDataObj.type == 1)
        {
            _jjAssetObj.media_type = "VIDEO";
            if (_jjAssetObj.youtubeUrlCheck)//Youtube Pre-recorded/Live Video Case
            {
                _jjAssetObj.youtubeUrl = _domeNFTDataObj.videoUrl;
            }
            else//AWS Video Case
            {
                _jjAssetObj.youtubeUrl = "";
                _jjAssetObj.asset_link = _domeNFTDataObj.videoUrl;
            }
            _jjAssetObj.thumbnail = _domeNFTDataObj.thumbnail;
            worldPlayingVideos = true;
        }
        else
        {
            _jjAssetObj.media_type = "IMAGE";
            _jjAssetObj.asset_link = _domeNFTDataObj.thumbnail;
        }
        _jjAssetObj.createdAt = _domeNFTDataObj.createdAt;
        _jjAssetObj.updatedAt = _domeNFTDataObj.updatedAt;

    }
    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, string url, Texture image, DataType type, string videoLink, VideoTypeRes videoType, int nftId = 0, SummitVideoAndImageController.MuseumType museumType = SummitVideoAndImageController.MuseumType.AtomMuseum, int roomNum = 1)
    {
        nftTitle = title;
        _Ratio = ratio;
        _Title = title;
        _Aurthor = aurthur;
        _Des = des;
        _URL = url;
        _image = image;
        _Type = type;
        _VideoLink = videoLink;
        _videoType = videoType;

        ratioId = ((int)ratio);

        //renderTexture.Release();
        ratioReferences[ratioId].l_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_image.gameObject.SetActive(true);
        ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
        ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);

        if (!ScreenOrientationManager._instance.isPotrait) // for Landscape
        {
            //ratioReferences[ratioId].l_image.gameObject.SetActive(true);
            //ratioReferences[ratioId].p_image.gameObject.SetActive(true);
            //ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
            //ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
            ratioReferences[ratioId].l_Title.text = title;
            ratioReferences[ratioId].l_Aurthur.text = aurthur;
            ratioReferences[ratioId].l_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
            LandscapeObj.SetActive(true);
            PotraiteObj.SetActive(false);
            ratioReferences[ratioId].l_obj.SetActive(true);
            ratioReferences[ratioId].p_obj.SetActive(false);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].l_image.gameObject.SetActive(false);
                ratioReferences[ratioId].l_Loader.SetActive(true);
                ratioReferences[ratioId].p_Loader.SetActive(false);
                //ratioReferences[ratioId].l_videoPlayer.Play();

                if (videoType == VideoTypeRes.islive)
                {
                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = false;
                    ratioReferences[ratioId].l_videoPlayer.enabled = false;
                    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].l_LivePlayer.GetComponent<MediaPlayer>().enabled = true;
                    ratioReferences[ratioId].l_LivePlayer.SetActive(true);
                    ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.Events.AddListener(TurnLiveVideoImageOn);
                    StartCoroutine(PlayVideoAfterDelay(ratioReferences[ratioId].l_videoPlayer, ratioId, _VideoLink, _videoType, true));
                    //ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                    //ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                }
                else if (videoType == VideoTypeRes.prerecorded)
                {
                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].l_LivePlayer.GetComponent<MediaPlayer>().enabled = false;
                    //ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                    //ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                    ratioReferences[ratioId].l_videoPlayer.playOnAwake = true;
                    ratioReferences[ratioId].l_videoPlayer.enabled = true;
                    _VideoLink = ExtractVideoIdFromUrl(_VideoLink);
                    StartCoroutine(PlayVideoAfterDelay(ratioReferences[ratioId].l_videoPlayer, ratioId, _VideoLink, _videoType, true));
                }
                else if (videoType == VideoTypeRes.aws)
                {
                    if (ratioReferences[ratioId].l_PrerecordedPlayer)
                        ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);

                    if (ratioReferences[ratioId].l_LivePlayer)
                    {
                        ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                        ratioReferences[ratioId].l_LivePlayer.GetComponent<MediaPlayer>().enabled = false;
                    }

                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].l_videoPlayer.enabled = true;
                    PlayAWSVideo(ratioReferences[ratioId].l_videoPlayer, _VideoLink);
                    //ratioReferences[ratioId].l_videoPlayer.url = videoLink;
                    //ratioReferences[ratioId].l_videoPlayer.Play();

                }

                VideoOpened?.Invoke();
            }
            else// To Show Image
            {
                ratioReferences[ratioId].l_image.texture = image;
                ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
            }
        }
        else // for Potraite
        {
            ratioReferences[ratioId].p_Title.text = title;
            ratioReferences[ratioId].p_Aurthur.text = aurthur;
            ratioReferences[ratioId].p_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
            ratioReferences[ratioId].p_image.texture = image;
            LandscapeObj.SetActive(false);
            PotraiteObj.SetActive(true);
            ratioReferences[ratioId].l_obj.SetActive(false);
            ratioReferences[ratioId].p_obj.SetActive(true);
            if (type == DataType.Video)
            {
                ratioReferences[ratioId].p_image.gameObject.SetActive(false);
                ratioReferences[ratioId].l_Loader.SetActive(false);
                ratioReferences[ratioId].p_Loader.SetActive(true);
                //ratioReferences[ratioId].p_videoPlayer.Play();

                if (videoType == VideoTypeRes.islive)
                {
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = false;
                    ratioReferences[ratioId].p_videoPlayer.enabled = false;
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].p_LivePlayer.GetComponent<MediaPlayer>().enabled = true;
                    ratioReferences[ratioId].p_LivePlayer.SetActive(true);
                    ratioReferences[ratioId].p_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.Events.AddListener(TurnLiveVideoImageOn);
                    StartCoroutine(PlayVideoAfterDelay(ratioReferences[ratioId].p_videoPlayer, ratioId, _VideoLink, _videoType, false));
                    ratioReferences[ratioId].p_Loader.SetActive(false);
                    ratioReferences[ratioId].l_Loader.SetActive(false);
                    //ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                    //ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                }
                else if (videoType == VideoTypeRes.prerecorded)
                {
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(true);
                    ratioReferences[ratioId].p_LivePlayer.GetComponent<MediaPlayer>().enabled = false;
                    ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                    //ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                    //ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                    ratioReferences[ratioId].p_videoPlayer.playOnAwake = true;
                    ratioReferences[ratioId].p_videoPlayer.enabled = true;
                    _VideoLink = ExtractVideoIdFromUrl(_VideoLink);
                    StartCoroutine(PlayVideoAfterDelay(ratioReferences[ratioId].p_videoPlayer, ratioId, _VideoLink, _videoType, false));
                }
                else if (videoType == VideoTypeRes.aws)
                {
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].p_LivePlayer.GetComponent<MediaPlayer>().enabled = false;
                    ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].p_videoPlayer.enabled = true;
                    PlayAWSVideo(ratioReferences[ratioId].p_videoPlayer, _VideoLink);
                    //ratioReferences[ratioId].p_videoPlayer.url = videoLink;
                    //ratioReferences[ratioId].p_videoPlayer.Play();

                }

                VideoOpened?.Invoke();
            }
            else// For portrait images
            {
                ratioReferences[ratioId].p_image.texture = image;
                ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
            }

        }
        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
        }

        //#region For firebase analytics
        ////if (roomNum != 0)
        ////    SendCallAnalytics(type, title, nftId, museumType, roomNum);         // firebase event calling in this method
        //clickedNftInd = nftId;
        //clRoomId = roomNum;
        //roomName = museumType.ToString();
        //#endregion
    }

    public void PlayAWSVideo(VideoPlayer _videoPLayer, string _videoURL)
    {
        _videoPLayer.source = VideoSource.Url;
        _videoPLayer.url = _videoURL;
        _videoPLayer.Prepare();
        _videoPLayer.prepareCompleted += AWSVideoPrepared;
    }

    void AWSVideoPrepared(VideoPlayer vp)
    {
        vp.time = 0;
        vp.Play();
    }

    IEnumerator PlayVideoAfterDelay(VideoPlayer _videoPLayer, int _id, string _videoLink, VideoTypeRes _videoType, bool _isLandscape)
    {
        yield return new WaitForSeconds(0.5f);
        //_videoPLayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(_videoLink, _isLive);
        if (_isLandscape == true)//For Landscape
        {
            ratioReferences[_id].l_obj.GetComponent<AdvancedYoutubePlayer>().VideoId = _videoLink;
            bool _isLive = _videoType.Equals(VideoTypeRes.islive) ? true : false;
            ratioReferences[_id].l_obj.GetComponent<AdvancedYoutubePlayer>().IsLive = _isLive;
            ratioReferences[_id].l_obj.GetComponent<AdvancedYoutubePlayer>().PlayVideo();
        }
        else//For Portrait
        {
            ratioReferences[_id].p_obj.GetComponent<AdvancedYoutubePlayer>().VideoId = _videoLink;
            bool _isLive = _videoType.Equals(VideoTypeRes.islive) ? true : false;
            ratioReferences[_id].p_obj.GetComponent<AdvancedYoutubePlayer>().IsLive = _isLive;
            ratioReferences[_id].p_obj.GetComponent<AdvancedYoutubePlayer>().PlayVideo();
        }
    }

    public string ExtractVideoIdFromUrl(string url)
    {
        // Find the position of the "v=" parameter
        int startIndex = url.IndexOf("v=");

        if (startIndex != -1)
        {
            // Extract the substring after "v="
            startIndex += 2; // Move past "v="
            int endIndex = url.IndexOf('&', startIndex);
            if (endIndex == -1)
                endIndex = url.Length;

            // Get the video ID
            string videoId = url.Substring(startIndex, endIndex - startIndex);
            return videoId;
        }

        // If "v=" parameter is not found, handle accordingly (e.g., return null or an error message)
        return null;
    }

    //public void TurnOfLdrOnPlayLiveVideo()
    //{
    //    Invoke(nameof(TurnLiveVideoImageOn), 5f);
    //}

    public void TurnLiveVideoImageOn(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        if (eventType == MediaPlayerEvent.EventType.Started)
        {
            ratioReferences[ratioId].p_Loader.SetActive(false);
            ratioReferences[ratioId].l_Loader.SetActive(false);
            ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.transform.GetChild(0).gameObject.SetActive(true);
            ratioReferences[ratioId].p_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void CloseInfoPop()
    {
        renderTexture_16x9.Release();
        renderTexture_9x16 .Release();
        renderTexture_1x1.Release();
        ratioReferences[ratioId].l_obj.SetActive(false);
        ratioReferences[ratioId].p_obj.SetActive(false);
        ratioReferences[ratioId].p_Loader.SetActive(false);
        ratioReferences[ratioId].l_Loader.SetActive(false);
        ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.gameObject.SetActive(false);
        ratioReferences[ratioId].p_obj.GetComponent<AdvancedYoutubePlayer>().AVProVideoPlayer.gameObject.SetActive(false);
        LandscapeObj.SetActive(false);
        PotraiteObj.SetActive(false);
        ratioReferences[ratioId].l_videoPlayer.Stop();
        ratioReferences[ratioId].l_videoPlayer.url = "";
        ratioReferences[ratioId].p_videoPlayer.Stop();
        ratioReferences[ratioId].p_videoPlayer.url = "";
        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
        }
        if (SoundSettings.soundManagerSettings.bgmSource)
        {
            SoundSettings.soundManagerSettings.bgmSource.mute = false;
        }
        NFTClosed?.Invoke();
    }

    IEnumerator GetNFTDatAForDynamicMuseum()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }
        // Making list for Data
        //for (int i = 0; i < 65; i++)
        //{
        //    AllS3Nfts.Add(new S3NftDetail());
        //}

        //print("~~~~~~~~~ " + ConstantsGod.API_BASEURL + _mussuemLink);
        //print("TOKEN : "+ ConstantsGod.AUTH_TOKEN);
        using (UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL + _mussuemLink))
        {

            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            //request.SetRequestHeader("Authorization", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MzIyNTAsImlhdCI6MTY4NzUxNDQxMywiZXhwIjoxNjg3Njg3MjEzfQ.FudsBYovB4KQYuR6EMmw0Mh8qAX0ZwaPHsoUyJmfuPk");

            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    //print("~!~!~! " + request.downloadHandler.text);

                    /*DomeNFTDataArray */
                    DoomNFTData = new DomeNFTDataArray();
                    DoomNFTData = JsonUtility.FromJson<DomeNFTDataArray>(request.downloadHandler.text);

                    if (DoomNFTData.getcontentbyDomeId.Count == 0)
                    {
                        Debug.Log(" NO DATA GET FROM API ");
                    }
                    else
                    {
                        //foreach (var item in DoomNFTData.getcontentbyDomeId)
                        //{
                        //    Debug.Log($"ID: {item.id}, Name: {item.name}");
                        //    Debug.Log("Count of array: " + DoomNFTData.getcontentbyDomeId.Count);
                        //}
                        StartCoroutine(InitData(DoomNFTData, NftPlaceholder));
                    }// data count check if end
                } // error check if end
            }
            else
            {
                Debug.Log(request.error);
            }
            request.Dispose();
            if (NFTDataFetchScrptRef)
            {
                InitializeS3NFTObjt();
            }
            //yield return StartCoroutine(UdpdatData(AllS3Nfts));
        }
    }

    public void InitializeS3NFTObjt()
    {
        if (DoomNFTData.getcontentbyDomeId.Count > 0)
        {
            for (int i = 0; i < DoomNFTData.getcontentbyDomeId.Count; i++)
            {
                DoomNFTData.getcontentbyDomeId[i].check = true;
            }
            UpdateRoomStatus(DoomNFTData.getcontentbyDomeId);
        }
    }

    IEnumerator UdpdatData(List<S3NftDetail> allS3Nfts)
    {
        //allS3Nfts = UpdateRoomStatus(allS3Nfts);
        int i = 0;
        foreach (S3NftDetail d in allS3Nfts)
        {
            if (d == null)
                break;
            if (i >= NFTDataFetchScrptRef.spawnPoints.Count)
                break;
            NFTDataFetchScrptRef.spawnPoints[i].SetActive(true);
            NFTDataFetchScrptRef.spawnPoints[i].AddComponent<DynamicGalleryData>().detail = d;
            NFTDataFetchScrptRef.spawnPoints[i].GetComponent<DynamicGalleryData>().videoPlayer = ShowNFTDetails.instance.VideoObject.GetComponent<VideoPlayer>();
            NFTDataFetchScrptRef.spawnPoints[i].GetComponent<DynamicGalleryData>().videoPlayerWithStats = ShowNFTDetails.instance.VideoObjectWithDes.GetComponent<VideoPlayer>();
            DynamicGalleryData data = NFTDataFetchScrptRef.spawnPoints[i].GetComponent<DynamicGalleryData>();
            NFTDataFetchScrptRef.spawnPoints[i].AddComponent<Fram_Image>();

            data.NewFrame = NFTDataFetchScrptRef.spawnPoints[i].GetComponent<Fram_Image>();

            data.SpotLightObj = NFTDataFetchScrptRef.spotLightSprite;
            data.isDynamicMuseum = NFTDataFetchScrptRef.isDynamicMuseum;
            data.framePrefab = NFTDataFetchScrptRef.dynamicManager.frame;

            int tempIndex;
            string ratio;
            if (d.ratio != "" && d.ratio != null)
            {
                ratio = d.ratio;
                if (ratio == "1:1") // is square
                {
                    tempIndex = 0;
                }
                else if (ratio == "9:16") // is potraite
                {
                    tempIndex = 1;

                }
                else // is landsacpe
                {
                    tempIndex = 2;
                }
            }
            else
            {
                tempIndex = 0;
            }
            data.FrameLocalPos = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].FrameLocalPos;
            data.FrameLocalScale = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].FrameLocalScale;
            data.SpotLightPos = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].SpotLightPos;
            data.ColliderSize = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].ColliderSize;
            data.ColiderPos = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].ColiderPos;
            data.spotLightPrefabPos = NFTDataFetchScrptRef.dynamicManager.exhibatsSizes[tempIndex].SpotLightPrefabPos;

            //data.FrameLocalPos = NFTDataFetchScrptRef.dynamicManager.FrameLocalPos;
            //data.FrameLocalScale = NFTDataFetchScrptRef.dynamicManager.FrameLocalScale;
            //data.SpotLightPos = NFTDataFetchScrptRef.dynamicManager.SpotLightPos;
            //data.ColliderSize = NFTDataFetchScrptRef.dynamicManager.ColliderSize;
            data.spotLightPrefab = NFTDataFetchScrptRef.dynamicManager.spotLightPrefab;
            data.FrameMat = NFTDataFetchScrptRef.dynamicManager.FrameMaterial;
            data.potraiteSize = NFTDataFetchScrptRef.dynamicManager.potraiteSize;
            data.landscapeSize = NFTDataFetchScrptRef.dynamicManager.landscapeSize;

            // if user is joining through event then video square size should be this 
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                data.squareSize = NFTDataFetchScrptRef.dynamicManager.VideosquarSize;

            }
            else
            {
                data.squareSize = NFTDataFetchScrptRef.dynamicManager.squarSize;
            }

            NFTDataFetchScrptRef.spawnPoints[i].GetComponent<DynamicGalleryData>().StartNow();
            i++;
        }
        yield return null;

    }

    /// <summary>
    /// update room according to data fetch from server
    /// </summary>
    /// <param name="dataList"> sever response data </param>
    void UpdateRoomStatus(List<DomeNFTData> allS3Nfts)
    {
        int index = 0;
        int endIndex = 0;
        int nftCount = 0;
        //int groupFirstIndex=0;
        for (int roomNo = 0; roomNo < RoomCount; roomNo++)
        {
            endIndex = endIndex + 16;
            for (int no = index; no <= endIndex; no++)
            {
                if (no < allS3Nfts.Count)
                {
                    if (allS3Nfts[no].check == true/*allS3Nfts[no].Title != "" && allS3Nfts[no].NFTImageLink != ""*/)
                    {
                        nftCount++;
                    }
                }
                else
                {
                    break;
                }
            }
            if (NFTDataFetchScrptRef.dynamicManager != null)
            {
                if (nftCount <= 0)
                {
                    NFTDataFetchScrptRef.dynamicManager.rooms[roomNo].IsInUse = false;
                }
                else
                {
                    NFTDataFetchScrptRef.dynamicManager.rooms[roomNo].IsInUse = true;
                }
            }
            index = endIndex;
            nftCount = 0;
        }
        //List<DomeNFTData> temp = new List<DomeNFTData>(allS3Nfts);
        //allS3Nfts = SwapData(temp);
        CloseAndOpenRoom();
    }

    /// <summary>
    /// Swap data in rooms 
    /// </summary>
    /// <param name="allS3Nfts"> nft list from the server</param>
    /// <returns></returns>
    List<DomeNFTData> SwapData(List<DomeNFTData> allS3Nfts)
    {
        int endIndex;
        int groupFirstIndex;
        endIndex = 64;
        groupFirstIndex = endIndex - 16;
        for (int r = RoomCount - 1; r >= 1; r--)
        {
            if (NFTDataFetchScrptRef.dynamicManager.rooms[r].IsInUse) // check current room is in use ?
            {
                if (!NFTDataFetchScrptRef.dynamicManager.rooms[r - 1].IsInUse) // check previous room is in use?
                {
                    for (int i = endIndex; i > groupFirstIndex; i--)
                    {
                        allS3Nfts[i - 16] = allS3Nfts[i];
                        allS3Nfts[i] = null;
                    }
                    NFTDataFetchScrptRef.dynamicManager.rooms[r].IsInUse = false;
                    NFTDataFetchScrptRef.dynamicManager.rooms[r - 1].IsInUse = true;
                    return SwapData(allS3Nfts);
                }
            }
            endIndex -= 16;
            groupFirstIndex -= 16;
        }
        return allS3Nfts;
    }

    /// <summary>
    /// Close and open the room according to the room use status
    /// </summary>
    void CloseAndOpenRoom()
    {
        if (NFTDataFetchScrptRef.dynamicManager != null)
        {
            for (int i = 1; i < RoomCount; i++)
            {
                if (NFTDataFetchScrptRef.dynamicManager.rooms[i].IsInUse)
                {
                    NFTDataFetchScrptRef.dynamicManager.rooms[i].Obj.SetActive(true);
                    if (NFTDataFetchScrptRef.dynamicManager.rooms[i].Door)
                    {
                        NFTDataFetchScrptRef.dynamicManager.rooms[i].Door.SetActive(false);
                    }
                }
                else
                {
                    NFTDataFetchScrptRef.dynamicManager.rooms[i].Obj.SetActive(false);
                    if (NFTDataFetchScrptRef.dynamicManager.rooms[i].Door)
                        NFTDataFetchScrptRef.dynamicManager.rooms[i].Door.SetActive(true);
                }
            }
        }
    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        if (videoRetry <= RetryChances)
        {
            videoRetry++;
            SetInfo(_Ratio, _Title, _Aurthor, _Des, _URL, _image, _Type, _VideoLink, _videoType);
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

}
