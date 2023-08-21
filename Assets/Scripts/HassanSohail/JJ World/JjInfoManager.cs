using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


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
    public string analyticMuseumID;
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
    public string firebaseEventName;
    public int clickedNftInd;
    public List<Texture> NFTLoadedSprites = new List<Texture>();
    public List<RenderTexture> NFTLoadedVideos = new List<RenderTexture>();

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
                InitData(json, NftPlaceholder);

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


    public void InitData(JjJson data, List<GameObject> NftPlaceholderList)
    {
        int nftPlaceHolder = NftPlaceholderList.Count;
        List<JjAsset> worldData = data.data;
        for (int i = 0; i < nftPlaceHolder; i++)
        {
            if (worldData.Count > i)
            {
                int tempIndex = worldData[i].index - 1;
                if (/*worldData[i].check &&*/ tempIndex == i)
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
                        NftPlaceholderList[i].GetComponent<JJVideoAndImage>().InitData(worldData[i].asset_link, null, worldInfos[i].JjRatio, DataType.Image, VideoTypeRes.none);

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
                                NftPlaceholderList[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.islive);
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
                                NftPlaceholderList[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].youtubeUrl, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.prerecorded);
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
                                NftPlaceholderList[i].GetComponent<JJVideoAndImage>().InitData(null, worldData[i].asset_link, worldInfos[i].JjRatio, DataType.Video, VideoTypeRes.aws);
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
                    NftPlaceholderList[i].gameObject.SetActive(false);
                    NftPlaceholderList[i].GetComponent<JJVideoAndImage>().TurnOffAllImageAndVideo();
                    Debug.Log("INDEX is off!");
                }
            }
            else
            {
                NftPlaceholderList[i].gameObject.SetActive(false);
                NftPlaceholderList[i].GetComponent<JJVideoAndImage>().TurnOffAllImageAndVideo();
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

    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, Texture2D image, DataType type, string videoLink, VideoTypeRes videoType, int id = -1)
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
                    if(ratioReferences[ratioId].l_PrerecordedPlayer)
                        ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);

                    if (ratioReferences[ratioId].l_LivePlayer)
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

        #region For firebase analytics
      
        SendCallAnalytics(type, title , id);
        clickedNftInd = id;
        #endregion
    }

    public void SendCallAnalytics(DataType type , string title ,int id=-1)
    {

        #region OLD Implementation
        //if (type == DataType.Image)
        //{
        //    int maxLength = 10;
        //    string originalString = title;
        //    originalString = Regex.Replace(originalString, @"\s", "");
        //    string trimmedString = originalString.Substring(0, Mathf.Min(originalString.Length, maxLength));
        //    string worldName = XanaConstants.xanaConstants.EnviornmentName;

        //    if (worldName.Contains("ZONE-X"))
        //    {
        //        //1F_Mainloby_A_ZoneX
        //        Firebase.Analytics.FirebaseAnalytics.LogEvent("1F_Mainloby_" + trimmedString + "_Clicked");
        //    }
        //    else if (worldName.Contains("ZONE X Musuem") || worldName.Contains("FIVE ELEMENTS"))
        //    {
        //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "_" + trimmedString + "_NFTClicked");
        //    }
        //    else
        //    {
        //        if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro))
        //            worldName = "Astro_";
        //        else if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
        //            worldName = "Rental_";

        //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + analyticMuseumID + "_" + trimmedString + "_NFTClicked");
        //    }
        //    Debug.Log("<color=red>" + worldName + analyticMuseumID + "_" + trimmedString + "_NFTClicked </color>");
        //}
        //else if (type == DataType.Video)
        //{
        //    int maxLength = 10;
        //    string originalString = title;
        //    originalString = Regex.Replace(originalString, @"\s", "");
        //    string trimmedString = originalString.Substring(0, Mathf.Min(originalString.Length, maxLength));

        //    string worldName = XanaConstants.xanaConstants.EnviornmentName;
        //    if (worldName.Contains("ZONE-X"))
        //    {
        //        Firebase.Analytics.FirebaseAnalytics.LogEvent("Lobby_" + trimmedString + "_Clicked");
        //    }
        //    else if (worldName.Contains("ZONE X Musuem") || worldName.Contains("FIVE ELEMENTS"))
        //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "_" + trimmedString + "_Video_Clicked");
        //    else
        //    {
        //        if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro))
        //            worldName = "Astro";
        //        else if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
        //            worldName = "Rental";

        //        Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + "_" + analyticMuseumID + "_" + trimmedString + "_Video_Clicked");
        //    }

        //    Debug.Log("<color=red>"+ worldName + "_" + analyticMuseumID + "_" + trimmedString + "_Video_Clicked </color>");
        //}
        #endregion

       // if (type == DataType.Image)
        {
            string worldName = XanaConstants.xanaConstants.EnviornmentName;
            if (!string.IsNullOrEmpty(firebaseEventName))
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(firebaseEventName +"NFT_" + id);
                Debug.Log("<color=red>" + firebaseEventName + "NFT_" + id + " </color>");
                return;
            }
            if (worldName.Contains("ZONE-X"))
            {
                //1F_Mainloby_A_ZoneX
                string myString = "";
                switch (id)
                {
                    case 0:
                        myString = "A_ZoneX";
                        break;

                    case 1:
                        myString = "B_FiveElement";
                        break;

                    case 2:
                        myString = "C_AtomMuseum";
                        break;

                    case 3:
                        myString = "D_RentalSpace";
                        break;

                    default:
                        myString = "";
                        break;
                }

                Firebase.Analytics.FirebaseAnalytics.LogEvent("F1_Mainloby_" + myString);
                Debug.Log("<color=red> F1_Mainloby_" + myString +"</color>");
            }
            else if (worldName.Contains("ZONE X Musuem") )
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("F1_ZoneX_NFT_No_" + clickedNftInd);
                Debug.Log("<color=red> F1_ZoneX_NFT_No_" + clickedNftInd+ " </color>");
            }
            else if (worldName.Contains("FIVE ELEMENTS"))
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("F1_FiveElement_NFT_No_" + clickedNftInd);
                Debug.Log("<color=red> F1_FiveElement_NFT_No_" + clickedNftInd + " </color>");
            }
            else
            {
                if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Astro))
                    worldName = "F2_Atom";
                else if (XanaConstants.xanaConstants.mussuemEntry.Equals(JJMussuemEntry.Rental))
                    worldName = "F2_Rental";

                Firebase.Analytics.FirebaseAnalytics.LogEvent(worldName + analyticMuseumID + "NFT_No_"+ (clickedNftInd + 1));
                Debug.Log("<color=red>" + worldName + analyticMuseumID + "NFT_No_" + (clickedNftInd + 1) + " </color>");
            }
            //Debug.Log("<color=red>" + worldName + analyticMuseumID + "_" + trimmedString + "_NFTClicked </color>");
        }
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