using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class JjInfoManager : MonoBehaviour
{
    [SerializeField] bool IsJjWorld;
    [SerializeField] bool worldPlayingVideos;
    [NonReorderable]
    public List<RatioReferences> ratioReferences;
    [NonReorderable]
    [SerializeField] List<VideoPlayer> VideoPlayers;
    [SerializeField] GameObject LandscapeObj;
    [SerializeField] GameObject PotraiteObj;

    [NonReorderable]
    public List<JJWorldInfo> worldInfos;

    [NonReorderable]
    public List<GameObject> NftPlaceholder;
    public static JjInfoManager Instance { get; private set; }

    public RenderTexture renderTexture_16x9;
    public RenderTexture renderTexture_9x16;
    public RenderTexture renderTexture_1x1;
    public RenderTexture renderTexture_4x3;
    [SerializeField] int RetryChances = 3;
    [SerializeField] int JJMusuemId;
    int ratioId;
    int videoRetry = 0;

    JjRatio _Ratio;
    string _Title;
    string _Aurthor;
    string _Des;
    Texture2D _image;
    DataType _Type;
    string _VideoLink;
    VideoTypeRes _videoType;

    public string nftTitle;


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
        if (VideoPlayers.Count>0)
        {
            foreach (VideoPlayer player in VideoPlayers)
            {
                player.errorReceived += ErrorOnVideo;
                player.prepareCompleted += VideoReady;
            }
        }
    }

    private void Start()
    {
        if (IsJjWorld)
        {
            IntJjInfoManager();
        }
    }

    /// <summary>
    /// It will clear the worldInfos list and Set infos
    /// </summary>
    public async void IntJjInfoManager()
    {
        //worldInfos.Clear();
        //for (int i = 0; i < NftPlaceholder.Count; i++)
        //{
        //    worldInfos.Add(null);
        //}
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.JJWORLDASSET + JJMusuemId);

        //try
        //{
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("<color=red>" + request.error + " </color>");
            }
            else
            {
                //Debug.Log("Get UnReadMessagesCount Success!");
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                //Debug.Log("JJ World Req" + data.ToString());
                JjJson json = JsonConvert.DeserializeObject<JjJson>(data.ToString());
                InitData(json);

            }
        }

        //}
        //catch
        //{
        //    Debug.Log("<color=red>jj APi not call in " + XanaConstants.xanaConstants.EnviornmentName + "</color>");
        //}
        //finally
        //{

        //}

    }


    void InitData(JjJson data)
    {
        int nftPlaceHolder = NftPlaceholder.Count;
        List<JjAsset> worldData = data.data;
        for (int i = 0; i < nftPlaceHolder; i++)
        {
            if(worldData.Count > i ){
                 int tempIndex = worldData[i].index-1;
                if (worldData[i].check && tempIndex == i)
                {
                    //Debug.Log("<color=red> INDEX IS : " + i + " </color>");
                    bool isWithDes = false;
                    string compersionPrfex = "";
                    //Debug.LogError(i + "-----" + nftPlaceHolder + "----"+worldData.Count);
                    switch (worldData[i].ratio)
                    {
                        case "1:1":
                            worldInfos[i].JjRatio = JjRatio.OneXOneWithDes;
                            compersionPrfex = "?width=512&height=512";
                            break;
                        case "16:9":
                            worldInfos[i].JjRatio = JjRatio.SixteenXNineWithDes;
                            compersionPrfex = "?width=500&height=600";
                            break;
                        case "9:16":
                            worldInfos[i].JjRatio = JjRatio.NineXSixteenWithDes;
                            compersionPrfex = "?width=700&height=500";
                            break;
                        case "4:3":
                            worldInfos[i].JjRatio = JjRatio.FourXThreeWithDes;
                            compersionPrfex = "?width=640&height=480";
                            break;
                        default:
                            worldInfos[i].JjRatio = JjRatio.OneXOneWithDes;
                            compersionPrfex = "?width=512&height=512";
                            break;
                    }

                    //Debug.LogError("-----" + worldData[i].media_type);
                    if (worldData[i].media_type == "IMAGE")
                    {

                        worldInfos[i].Type = DataType.Image;
                        NftPlaceholder[i].GetComponent<JJVideoAndImage>().InitData(worldData[i].asset_link, null, worldInfos[i].JjRatio, DataType.Image, VideoTypeRes.none);

                        if (!string.IsNullOrEmpty(worldData[i].title[0]) && !string.IsNullOrEmpty(worldData[i].authorName[0]) && !string.IsNullOrEmpty(worldData[i].description[0]))
                        {
                            isWithDes = true;
                            worldInfos[i].Title = worldData[i].title;
                            worldInfos[i].Aurthor = worldData[i].authorName;
                            worldInfos[i].Des = worldData[i].description;

                        }
                        else
                        {
                            isWithDes = false;
                            worldInfos[i].Title = null;
                            worldInfos[i].Aurthor = null;
                            worldInfos[i].Des = null;
                        }

                    }
                    else if (worldData[i].media_type == "VIDEO" || worldData[i].media_type == "LIVE")
                    {
                        worldInfos[i].Type = DataType.Video;
                        if (worldPlayingVideos) // to play video's in world
                        {
                            if (worldData[i].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[i].youtubeUrl))  //for Live Video 
                            {
                                worldInfos[i].VideoLink = worldData[i].youtubeUrl;
                                worldInfos[i].videoType = VideoTypeRes.islive;
                                NftPlaceholder[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.islive);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].youtubeUrl;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }
                            else if (!worldData[i].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[i].youtubeUrl))  // for Prerecorded video
                            {
                                worldInfos[i].VideoLink = worldData[i].youtubeUrl;
                                worldInfos[i].videoType = VideoTypeRes.prerecorded;
                                NftPlaceholder[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.prerecorded);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].youtubeUrl;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }
                            else if (!string.IsNullOrEmpty(worldData[i].asset_link))
                            {
                                worldInfos[i].VideoLink = worldData[i].asset_link;
                                worldInfos[i].videoType = VideoTypeRes.aws;
                                NftPlaceholder[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].asset_link, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.aws);
                                //NftPlaceholder[i].GetComponent<JjVideo>().isLiveVideo = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isPrerecoreded = false;
                                //NftPlaceholder[i].GetComponent<JjVideo>().isFromAws = true;
                                //NftPlaceholder[i].GetComponent<JjVideo>().videoLink = worldData[i].asset_link;
                                //NftPlaceholder[i].GetComponent<JjVideo>().CheckForPlayValidPlayer();
                            }


                            if (!string.IsNullOrEmpty(worldData[i].title[0]) && !string.IsNullOrEmpty(worldData[i].authorName[0]) && !string.IsNullOrEmpty(worldData[i].description[0]))
                            {
                                isWithDes = true;
                                worldInfos[i].Title = worldData[i].title;
                                worldInfos[i].Aurthor = worldData[i].authorName;
                                worldInfos[i].Des = worldData[i].description;
                            }
                            else
                            {
                                isWithDes = false;
                                worldInfos[i].Title = null;
                                worldInfos[i].Aurthor = null;
                                worldInfos[i].Des = null;
                            }
                        }
                    }
                }
                else
                {
                    NftPlaceholder[i].gameObject.SetActive(false);
                    NftPlaceholder[i].GetComponent<JJVideoAndImage>().TurnOffAllImageAndVideo();
                    Debug.Log("INDEX is off!");
                }
            }
            else
            {
                 NftPlaceholder[i].gameObject.SetActive(false);
                 NftPlaceholder[i].GetComponent<JJVideoAndImage>().TurnOffAllImageAndVideo();
            }
        }

    }

    IEnumerator GetSprite(string path, int index, System.Action<Sprite, int> callback)
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ERror in loading sprite" + www.error);
            }
            else
            {
                if (www.isDone)
                {
                    Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                    var rect = new Rect(1, 1, 1, 1);
                    //thunbNailImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    var tempTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = /*Sprite.Create(tempTex, rect, new Vector2(0.5f, 0.5f))*/ Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                    print("Texture is " + sprite);
                    callback(sprite, index);
                }
            }
        }
    }

    //Sprite DownloadSprite( string path)
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(path);
    //    yield return www.WaitForCompletion();

    //     if (www.IsSuccess) {
    //        Texture2D texture = www.GetTexture();
    //        spriteImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    //    }
    //}

    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, Texture2D image, DataType type, string videoLink, VideoTypeRes videoType)
    {
        nftTitle = title;
        _Ratio = ratio;
        _Title = title;
        _Aurthor = aurthur;
        _Des = des;
        _image = image;
        _Type = type;
        _VideoLink = videoLink;
        _videoType = videoType;

        ratioId = ((int)ratio);
        //renderTexture.Release();
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
            ratioReferences[ratioId].l_image.texture = image;
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
        ratioReferences[ratioId].p_image.texture = image;
        if (type == DataType.Image)
        {
            ratioReferences[ratioId].p_image.texture = image;
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

                if (videoType == VideoTypeRes.islive)
                {
                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = false;
                    ratioReferences[ratioId].l_videoPlayer.enabled = false;
                    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].l_LivePlayer.SetActive(true);
                    ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                    ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                }
                else if (videoType == VideoTypeRes.prerecorded)
                {
                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(true);
                    ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                    ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                    ratioReferences[ratioId].l_videoPlayer.playOnAwake = true;
                    ratioReferences[ratioId].l_videoPlayer.enabled = true;
                }
                else if (videoType == VideoTypeRes.aws)
                {
                    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].l_videoPlayer.enabled = true;
                    ratioReferences[ratioId].l_videoPlayer.url = videoLink;
                    ratioReferences[ratioId].l_videoPlayer.Play();

                }
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

                if (videoType == VideoTypeRes.islive)
                {
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = false;
                    ratioReferences[ratioId].p_videoPlayer.enabled = false;
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].p_LivePlayer.SetActive(true);
                    ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                    ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                }
                else if (videoType == VideoTypeRes.prerecorded)
                {
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(true);
                    ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                    ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                    ratioReferences[ratioId].p_videoPlayer.playOnAwake = true;
                    ratioReferences[ratioId].p_videoPlayer.enabled = true;
                }
                else if (videoType == VideoTypeRes.aws)
                {
                    ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                    ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                    ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                    ratioReferences[ratioId].p_videoPlayer.enabled = true;
                    ratioReferences[ratioId].p_videoPlayer.url = videoLink;
                    ratioReferences[ratioId].p_videoPlayer.Play();

                }
            }

        }
        if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
        {
            CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(false);
        }
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
        if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
        {
            CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(true);
        }

    }

    private void ErrorOnVideo(VideoPlayer source, string message)
    {
        if (videoRetry <= RetryChances)
        {
            videoRetry++;
            SetInfo(_Ratio, _Title, _Aurthor, _Des, _image, _Type, _VideoLink, _videoType);
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
        if (VideoPlayers.Count>0)
            foreach (VideoPlayer player in VideoPlayers)
            {
                player.errorReceived -= ErrorOnVideo;
                player.prepareCompleted -= VideoReady;
            }
    }
}

[Serializable]
public class JJWorldInfo
{
    public string[] Title;
    public string[] Aurthor;
    public string[] Des;
    public DataType Type;
    public Sprite WorldImage;
    public string VideoLink;
    public bool isAWSVideo;
    public bool isLiveVideo;
    public bool isPrerecordedVideo;
    public JjRatio JjRatio;
    public VideoTypeRes videoType;
}

public enum DataType
{
    Image,
    Video
}

public enum VideoTypeRes
{
    none,
    islive,
    prerecorded,
    aws
}

public enum JjRatio
{
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
    public RawImage l_image;
    public VideoPlayer l_videoPlayer;
    public GameObject l_LivePlayer;
    public GameObject l_PrerecordedPlayer;
    public GameObject l_Loader;

    public GameObject p_obj;
    public TMP_Text p_Title;
    public TMP_Text p_Aurthur;
    public TMP_Text p_Description;
    public RawImage p_image;
    public VideoPlayer p_videoPlayer;
    public GameObject p_LivePlayer;
    public GameObject p_PrerecordedPlayer;
    public GameObject p_Loader;
}


public class JjJson
{
    public bool success;
    public List<JjAsset> data;
    public string msg;
}

public class JjAsset
{
    public int id;
    public int museumId;
    public int index;
    public string asset_link;
    public bool check;
    public string[] authorName;
    public string[] description;
    public string[] title;
    public string ratio;
    public string thumbnail;
    public string media_type;
    public string env_class;
    public string user_id;
    public string event_id;
    public string category;
    public bool youtubeUrlCheck;
    public string youtubeUrl;
    public DateTime createdAt;
    public DateTime updatedAt;
    public string event_env_class;
}