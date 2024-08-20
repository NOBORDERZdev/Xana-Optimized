using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using static GlobalConstants;
using Paroxe.PdfRenderer;

namespace PMY
{
    public class PMY_Nft_Manager_Extension : MonoBehaviour
    {
        [SerializeField] bool worldPlayingVideos;
        [NonReorderable]
        public List<RatioRef> ratioReferences;

        [Space(10)]
        [Header("PDF Data Refrences")]
        public GameObject pdfPanel_L;
        public GameObject pdfPanel_P;
        public PDFViewer pdfViewer_L; 
        public PDFViewer pdfViewer_P;

        [Space(10)]
        [Header("Quiz Data Refrences")]
        public GameObject quizPanel_L;
        public GameObject quizPanel_P;

        [NonReorderable]
        [SerializeField] List<VideoPlayer> VideoPlayers;
        [SerializeField] GameObject LandscapeObj;
        [SerializeField] GameObject PotraiteObj;

        [NonReorderable]
        public List<PMY_WorldData> worldInfos;

        [NonReorderable]
        public List<GameObject> NftPlaceholder;
        public static PMY_Nft_Manager_Extension Instance { get; private set; }

        public RenderTexture renderTexture_16x9;
        public RenderTexture renderTexture_9x16;
        public RenderTexture renderTexture_1x1;
        public RenderTexture renderTexture_4x3;
        [SerializeField] int RetryChances = 3;
        [SerializeField] int PMY_RoomId_test;
        [SerializeField] int PMY_RoomId_main;
        public int PMY_RoomId;
        public bool PMY_RoomIdFromXanaConstant = false;

        public string analyticMuseumID;
        int ratioId;
        int videoRetry = 0;

        PMY_Ratio _Ratio;
        string _Title;
        string _Aurthor;
        string _Des;
        string _URL;
        Texture2D _image;
        PMY_DataType _Type;
        string _VideoLink;
        PMY_VideoTypeRes _videoType;
        public string _pdfURL;

        public QuizData _quiz_data;

        public string nftTitle;
        public string firebaseEventName;
        public int clickedNftInd;
        public List<Texture> NFTLoadedSprites = new List<Texture>();
        //public List<RenderTexture> NFTLoadedVideos = new List<RenderTexture>();

        public GameObject videoRenderObject;

        public AudioSource videoPlayerSource;
        public MediaPlayer livePlayerSource;

        public Action OnVideoEnlargeAction;
        public Action<int> exitClickedAction;


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

        private void Start()
        {
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
                if (PMY_RoomIdFromXanaConstant)
                    PMY_RoomId = XanaConstants.xanaConstants.pmy_classRoomID_Main;
                else
                    PMY_RoomId = PMY_RoomId_main;
            }
            else
            {
                if (PMY_RoomIdFromXanaConstant)
                    PMY_RoomId = XanaConstants.xanaConstants.pmy_classRoomID_Test;
                else
                    PMY_RoomId = PMY_RoomId_test;
            }
                Int_PMY_Nft_Manager();
        }

        /// <summary>
        /// It will clear the worldInfos list and Set infos
        /// </summary>
        public async void Int_PMY_Nft_Manager()
        {
            StringBuilder apiUrl = new StringBuilder();
            apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.PMYWorldASSET + PMY_RoomId);

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
                    StringBuilder data = new StringBuilder();
                    data.Append(request.downloadHandler.text);
                    PMY_Json json = JsonConvert.DeserializeObject<PMY_Json>(data.ToString());
                    StartCoroutine(InitData(json, NftPlaceholder));
                }
            }
        }

        bool isNFTUploaded = false;
        public IEnumerator InitData(PMY_Json data, List<GameObject> NftPlaceholderList)
        {
            int nftPlaceHolder = NftPlaceholderList.Count;
            List<PMY_Asset> worldData = data.data;
            for (int i = 0; i < nftPlaceHolder; i++)
            {
                isNFTUploaded = false;
                for (int j = 0; j < worldData.Count; j++)
                {
                    if (i == worldData[j].index - 1)
                    {
                        isNFTUploaded = true;
                        bool isWithDes = false;
                        string compersionPrfex = "";

                        switch (worldData[j].ratio)
                        {
                            case "1:1":
                                if (JJFrameManager.instance)
                                    JJFrameManager.instance.SetTransformForFrameSpotLight(0);
                                if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                                    worldInfos[i].pmyRatio = PMY_Ratio.OneXOneWithoutDes;
                                else
                                    worldInfos[i].pmyRatio = PMY_Ratio.OneXOneWithDes;
                                compersionPrfex = "?width=512&height=512";
                                break;
                            case "16:9":
                                if (JJFrameManager.instance)
                                    JJFrameManager.instance.SetTransformForFrameSpotLight(1);
                                if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                                    worldInfos[i].pmyRatio = PMY_Ratio.SixteenXNineWithoutDes;
                                else
                                    worldInfos[i].pmyRatio = PMY_Ratio.SixteenXNineWithDes;
                                compersionPrfex = "?width=800&height=450";//"?width=500&height=600";
                                break;
                            case "9:16":
                                if (JJFrameManager.instance)
                                    JJFrameManager.instance.SetTransformForFrameSpotLight(2);
                                if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                                    worldInfos[i].pmyRatio = PMY_Ratio.NineXSixteenWithoutDes;
                                else
                                    worldInfos[i].pmyRatio = PMY_Ratio.NineXSixteenWithDes;
                                compersionPrfex = "?width=450&height=800"; //"?width=700&height=500";
                                break;
                            case "4:3":
                                if (JJFrameManager.instance)
                                    JJFrameManager.instance.SetTransformForFrameSpotLight(3);
                                if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                                    worldInfos[i].pmyRatio = PMY_Ratio.FourXThreeWithoutDes;
                                else
                                    worldInfos[i].pmyRatio = PMY_Ratio.FourXThreeWithDes;
                                compersionPrfex = "?width=640&height=480";
                                break;
                            default:
                                if (JJFrameManager.instance)
                                    JJFrameManager.instance.SetTransformForFrameSpotLight(0);
                                if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                                    worldInfos[i].pmyRatio = PMY_Ratio.OneXOneWithoutDes;
                                else
                                    worldInfos[i].pmyRatio = PMY_Ratio.OneXOneWithDes;
                                compersionPrfex = "?width=512&height=512";
                                break;
                        }
                        NftPlaceholderList[i].SetActive(true);

                        if (worldData[j].media_type == "IMAGE")
                        {

                            worldInfos[i].Type = PMY_DataType.Image;
                            NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].asset_link + compersionPrfex, null, worldInfos[i].pmyRatio, PMY_DataType.Image, PMY_VideoTypeRes.none);

                            isWithDes = true;
                            worldInfos[i].Title = worldData[j].title;
                            worldInfos[i].Aurthor = worldData[j].authorName;
                            worldInfos[i].Des = worldData[j].description;
                            worldInfos[i].url = worldData[j].descriptionHyperlink;
                        }
                        else if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                        {
                            worldInfos[i].Type = PMY_DataType.Video;
                            if (worldPlayingVideos) // to play video's in world
                            {
                                if (worldData[j].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[j].youtubeUrl))  //for Live Video 
                                {
                                    yield return new WaitForSeconds(1f);
                                    worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.islive;
                                    NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].thumbnail, worldData[j].youtubeUrl, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.islive);
                                }
                                else if (!worldData[j].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[j].youtubeUrl))  // for Prerecorded video
                                {
                                    yield return new WaitForSeconds(1f);
                                    worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.prerecorded;
                                    NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].thumbnail, worldData[j].youtubeUrl, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.prerecorded);
                                }
                                else if (!string.IsNullOrEmpty(worldData[j].asset_link))
                                {
                                    worldInfos[i].VideoLink = worldData[j].asset_link;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.aws;
                                    NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].thumbnail, worldData[j].asset_link + compersionPrfex, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.aws);
                                }
                                isWithDes = true;
                                worldInfos[i].Title = worldData[j].title;
                                worldInfos[i].Aurthor = worldData[j].authorName;
                                worldInfos[i].Des = worldData[j].description;
                                worldInfos[i].url = worldData[j].descriptionHyperlink;
                            }
                        }
                        else if(worldData[j].media_type == "PDF")
                        {
                            worldInfos[i].Type = PMY_DataType.PDF;
                            worldInfos[i].pdfURL = worldData[j].pdf_url;
                            worldInfos[i].thumbnail= worldData[j].thumbnail;
                            NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].thumbnail, null, worldInfos[i].pmyRatio, PMY_DataType.PDF, PMY_VideoTypeRes.none);
                        }
                        else if (worldData[j].media_type == "QUIZ")
                        {
                            worldInfos[i].Type = PMY_DataType.Quiz;
                            worldInfos[i].thumbnail = worldData[j].thumbnail;
                            worldInfos[i].quiz_data = worldData[j].quiz_data;
                            NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().InitData(worldData[j].thumbnail, null, worldInfos[i].pmyRatio, PMY_DataType.Quiz, PMY_VideoTypeRes.none);
                        }
                        break;
                    }
                    else
                    {
                        if (j == worldData.Count - 1)
                        {
                            NftPlaceholderList[i].gameObject.SetActive(false);
                            NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().TurnOffAllImageAndVideo();
                            Debug.Log("INDEX is off!");
                        }
                    }
                }
                if (!isNFTUploaded)
                {
                    NftPlaceholderList[i].gameObject.SetActive(false);
                    NftPlaceholderList[i].GetComponent<PMY_VideoAndImage_Extension>().TurnOffAllImageAndVideo();
                }
            }
        }

        public void LoadPrerecordedIfNoLongerLive(GameObject obj, string precorderUrl)
        {
            worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].VideoLink = precorderUrl;
            worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].videoType = PMY_VideoTypeRes.prerecorded;
            obj.GetComponent<PMY_VideoAndImage_Extension>().InitData(null, precorderUrl, worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.prerecorded);
        }

        public void LoadLiveIfFirstTimeNotLoaded(GameObject obj, string url)
        {
            if (obj.GetComponent<PMY_VideoAndImage_Extension>() != null)
            {
                worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].VideoLink = url;
                worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].videoType = PMY_VideoTypeRes.islive;
                obj.GetComponent<PMY_VideoAndImage_Extension>().InitData(null, url, worldInfos[obj.GetComponent<PMY_VideoAndImage_Extension>().id].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.islive);
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
                        var tempTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        Sprite sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(.5f, 0f));
                        print("Texture is " + sprite);
                        callback(sprite, index);
                    }
                }
            }
        }


        public void SetInfo(PMY_Ratio ratio, string title, string aurthur, string des, string url, Texture2D image, PMY_DataType type, string videoLink, PMY_VideoTypeRes videoType, string pdfURL, QuizData quizData ,int nftId = 0, PMY_VideoAndImage_Extension.RoomType roomType = PMY_VideoAndImage_Extension.RoomType.Gallery)
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
            _pdfURL = pdfURL;
            _quiz_data = quizData;

            ratioId = ((int)ratio);

            if (type == PMY_DataType.PDF)
            {
                pdfViewer_L.FileURL= pdfURL;
                pdfViewer_P.FileURL = pdfURL;
                Enable_PDF_Panel();
            }
            else if (type == PMY_DataType.Quiz)
            {
                quizPanel_L.GetComponent<PMY_QuizController>().SetQuizData(quizData);

                quizPanel_P.GetComponent<PMY_QuizController>().SetQuizData(quizData);
                EnableQuizPanel();
            }
            else
            {
                // Setting Landscape Data
                ratioReferences[ratioId].l_image.gameObject.SetActive(true);
                ratioReferences[ratioId].p_image.gameObject.SetActive(true);
                //ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
                ratioReferences[ratioId].p_PrerecordedPlayer.gameObject.SetActive(true);
                ratioReferences[ratioId].l_PrerecordedPlayer.gameObject.SetActive(true);

                if (ratioId < 4)
                {
                    ratioReferences[ratioId].l_Title.text = title;
                    ratioReferences[ratioId].l_Aurthur.text = aurthur;
                    ratioReferences[ratioId].l_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
                }
                if (type == PMY_DataType.Image)
                {
                    ratioReferences[ratioId].l_image.texture = image;
                    ratioReferences[ratioId].l_PrerecordedPlayer.gameObject.SetActive(false);
                }
                else
                {
                    ratioReferences[ratioId].l_image.gameObject.SetActive(false);
                    ratioReferences[ratioId].l_PrerecordedPlayer.url = videoLink;
                }

                // Setting Potraite Data
                if (ratioId < 4)
                {
                    ratioReferences[ratioId].p_Title.text = title;
                    ratioReferences[ratioId].p_Aurthur.text = aurthur;
                    ratioReferences[ratioId].p_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
                }
                ratioReferences[ratioId].p_image.texture = image;
                if (type == PMY_DataType.Image)
                {
                    ratioReferences[ratioId].p_image.texture = image;
                    ratioReferences[ratioId].p_PrerecordedPlayer.gameObject.SetActive(false);
                }
                else
                {
                    ratioReferences[ratioId].p_image.gameObject.SetActive(false);
                    ratioReferences[ratioId].p_PrerecordedPlayer.url = videoLink;
                }
                if (!ChangeOrientation_waqas._instance.isPotrait) // for Landscape
                {
                    LandscapeObj.SetActive(true);
                    PotraiteObj.SetActive(false);
                    ratioReferences[ratioId].l_obj.SetActive(true);
                    ratioReferences[ratioId].p_obj.SetActive(false);
                    if (type == PMY_DataType.Video)
                    {
                        ratioReferences[ratioId].l_Loader.SetActive(true);
                        ratioReferences[ratioId].p_Loader.SetActive(false);
                        ratioReferences[ratioId].l_PrerecordedPlayer.Play();

                        if (videoType == PMY_VideoTypeRes.islive)
                        {
                            //ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<RawImage>().enabled = false;
                            //ratioReferences[ratioId].l_videoPlayer.enabled = false;
                            //ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                            //ratioReferences[ratioId].l_LivePlayer.SetActive(true);
                            //ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                            //ratioReferences[ratioId].l_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                            ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, true);
                        }
                        else if (videoType == PMY_VideoTypeRes.prerecorded)
                        {
                            //ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<RawImage>().enabled = true;
                            //ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(true);
                            //ratioReferences[ratioId].l_LivePlayer.SetActive(false);
                            ratioReferences[ratioId].l_obj.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, false);
                        }
                        else if (videoType == PMY_VideoTypeRes.aws)
                        {
                            //if (ratioReferences[ratioId].l_PrerecordedPlayer)
                            //    ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);

                            //if (ratioReferences[ratioId].l_LivePlayer)
                            //    ratioReferences[ratioId].l_LivePlayer.SetActive(false);

                            //ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<RawImage>().enabled = true;
                            //ratioReferences[ratioId].l_videoPlayer.enabled = true;
                            ratioReferences[ratioId].l_PrerecordedPlayer.gameObject.SetActive(true);
                            ratioReferences[ratioId].l_PrerecordedPlayer.url = videoLink;
                            ratioReferences[ratioId].l_PrerecordedPlayer.Play();

                        }

                        OnVideoEnlargeAction?.Invoke();
                    }
                }
                else // for Potraite
                {
                    LandscapeObj.SetActive(false);
                    PotraiteObj.SetActive(true);
                    ratioReferences[ratioId].l_obj.SetActive(false);
                    ratioReferences[ratioId].p_obj.SetActive(true);
                    if (type == PMY_DataType.Video)
                    {
                        ratioReferences[ratioId].l_Loader.SetActive(false);
                        ratioReferences[ratioId].p_Loader.SetActive(true);
                        //ratioReferences[ratioId].p_videoPlayer.Play();

                        if (videoType == PMY_VideoTypeRes.islive)
                        {
                            //ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = false;
                            //ratioReferences[ratioId].p_videoPlayer.enabled = false;
                            //ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                            //ratioReferences[ratioId].p_LivePlayer.SetActive(true);
                            ratioReferences[ratioId].p_obj.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, true);
                            //ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                            //ratioReferences[ratioId].p_LivePlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
                        }
                        else if (videoType == PMY_VideoTypeRes.prerecorded)
                        {
                            //ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                            //ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(true);
                            //ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                            //ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                            //ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                            ratioReferences[ratioId].p_obj.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, false);
                            //ratioReferences[ratioId].p_videoPlayer.playOnAwake = true;
                            //ratioReferences[ratioId].p_videoPlayer.enabled = true;
                        }
                        else if (videoType == PMY_VideoTypeRes.aws)
                        {
                            ratioReferences[ratioId].p_PrerecordedPlayer.gameObject.SetActive(true);
                            //ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                            //ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                            //ratioReferences[ratioId].p_videoPlayer.enabled = true;
                            ratioReferences[ratioId].p_PrerecordedPlayer.url = videoLink;
                            ratioReferences[ratioId].p_PrerecordedPlayer.Play();

                        }

                        OnVideoEnlargeAction.Invoke();
                    }

                }
            }
            if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
            {
                CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(false);
            }

            #region For firebase analytics
            SendCallAnalytics(nftId, roomType);         // firebase event calling in this method
            clickedNftInd = nftId;
            #endregion
        }

        public void SendCallAnalytics(int id = -1, PMY_VideoAndImage_Extension.RoomType roomType = PMY_VideoAndImage_Extension.RoomType.Gallery)
        {
            // For firebase event
            string eventName = "";
            switch (roomType)
            {
                case PMY_VideoAndImage_Extension.RoomType.PMYLobby:
                    eventName = FirebaseTrigger.CL_NFT_PMYLobby.ToString() + "_" + (id + 1);
                    break;
                case PMY_VideoAndImage_Extension.RoomType.RoomA_1:
                    eventName = FirebaseTrigger.CL_NFT_CRoom1.ToString() + "_" + (id + 1);
                    break;
                case PMY_VideoAndImage_Extension.RoomType.RoomA_2:
                    eventName = FirebaseTrigger.CL_NFT_CRoom2.ToString() + "_" + (id + 1);
                    break;
                case PMY_VideoAndImage_Extension.RoomType.Gallery:
                    eventName = FirebaseTrigger.CL_NFT_Gallery.ToString() + "_" + (id + 1);
                    break;
            }
            SendFirebaseEvent(eventName);
        }

        public void ActionOnExitBtn()
        {
            exitClickedAction?.Invoke(clickedNftInd);
        }

        public void CloseInfoPop()
        {
            ActionOnExitBtn();
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
                SetInfo(_Ratio, _Title, _Aurthor, _Des, _URL, _image, _Type, _VideoLink, _videoType, _pdfURL,_quiz_data);
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
        public async void EnableQuizPanel()
        {
            if (!ChangeOrientation_waqas._instance.isPotrait)
            {
                quizPanel_L.SetActive(true);
            }
            else
            {
                quizPanel_P.SetActive(true);
            }
        }
        public void Enable_PDF_Panel()
        {
            if (!ChangeOrientation_waqas._instance.isPotrait)
                pdfPanel_L.SetActive(true);
            else
                pdfPanel_L.SetActive(true);
        }

        public void EnableControlls()
        {
            ActionOnExitBtn();
            if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
            {
                CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (VideoPlayers.Count > 0)
                foreach (VideoPlayer player in VideoPlayers)
                {
                    player.errorReceived -= ErrorOnVideo;
                    player.prepareCompleted -= VideoReady;
                }
        }
    }
}