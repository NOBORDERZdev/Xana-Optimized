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
using UnityEngine.Events;

namespace Toyota
{
    public class AR_Nft_Manager : MonoBehaviour
    {
        public enum NFT_NAME { Stage, Factory, Home, Architec, LandInfo }
        [SerializeField] NFT_NAME nftName;
        [SerializeField] bool isShowDataAtStart = true;

        [SerializeField] int RetryChances = 3;
        [SerializeField] int PMY_RoomId_test;
        [SerializeField] int PMY_RoomId_main;
        public bool PMY_RoomIdFromXanaConstant = false;
        [HideInInspector]
        public int PMY_RoomId;

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

        [NonReorderable]
        public List<PMY_WorldData> worldInfos;
        [NonReorderable]
        public List<GameObject> NftPlaceholder;
        [HideInInspector]
        public string roomName;
        [Space(5)]
        public UnityEvent allDataLoaded;
        private NFT_Holder_Manager nftHolder;
        private PMY_Json json = new PMY_Json();

        private void Awake()
        {
            nftHolder = NFT_Holder_Manager.instance;
        }

        private void Start()
        {
            // bind events
            if (nftHolder.VideoPlayers.Count > 0)
            {
                foreach (VideoPlayer player in nftHolder.VideoPlayers)
                {
                    player.errorReceived += ErrorOnVideo;
                    player.prepareCompleted += VideoReady;
                }
            }
            InRoomSoundHandler.playerInRoom += UpdateNFTData;

            if (APIBasepointManager.instance && APIBasepointManager.instance.IsXanaLive)
                PMY_RoomId = PMY_RoomId_main;
            else
                PMY_RoomId = PMY_RoomId_test;

            Int_PMY_Nft_Manager();
        }

        /// <summary>
        /// It will clear the worldInfos list and Set infos
        /// </summary>
        public async void Int_PMY_Nft_Manager()
        {
            StringBuilder apiUrl = new StringBuilder();
            if (ConstantsHolder.xanaConstants.MuseumID == "2871")
            {
                apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.JjTestWorldAssets + PMY_RoomId);
            }
            else
            {
                apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaApi + PMY_RoomId);
            }
            Debug.Log("<color=red>PMY_AdminApi: " + apiUrl + "</color>");
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
                    json = JsonConvert.DeserializeObject<PMY_Json>(data.ToString());
                    if (isShowDataAtStart)
                        StartCoroutine(InitData(json, NftPlaceholder));
                }
            }
        }

        // This method will call when player enter in respective room
        private void UpdateNFTData(bool playerInRoom, string roomName)
        {
            if (roomName != nftName.ToString()) return;

            if (playerInRoom)
                StartCoroutine(InitData(json, NftPlaceholder));
            else if (!playerInRoom)
            {
                for (int i = 0; i < NftPlaceholder.Count; i++)
                    NftPlaceholder[i].GetComponent<AR_VideoAndImage>().EraseDownloadedData();
            }
            NFTLoadedSprites.Clear();
            //NFTLoadedVideos.Clear();
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
                            NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().InitData(worldData[j].asset_link + compersionPrfex, null, worldInfos[i].pmyRatio, PMY_DataType.Image, PMY_VideoTypeRes.none);

                            isWithDes = true;
                            worldInfos[i].Title = worldData[j].title;
                            worldInfos[i].Aurthor = worldData[j].authorName;
                            worldInfos[i].Des = worldData[j].description;
                            worldInfos[i].url = worldData[j].descriptionHyperlink;
                        }
                        else if (worldData[j].media_type == "VIDEO" || worldData[j].media_type == "LIVE")
                        {
                            worldInfos[i].Type = PMY_DataType.Video;
                            if (nftHolder.worldPlayingVideos) // to play video's in world
                            {
                                if (worldData[j].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[j].youtubeUrl))  //for Live Video 
                                {
                                    yield return new WaitForSeconds(1f);
                                    worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.islive;
                                    NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().InitData(worldData[j].thumbnail, worldData[j].youtubeUrl, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.islive);
                                }
                                else if (!worldData[j].youtubeUrlCheck && !string.IsNullOrEmpty(worldData[j].youtubeUrl))  // for Prerecorded video
                                {
                                    yield return new WaitForSeconds(1f);
                                    worldInfos[i].VideoLink = worldData[j].youtubeUrl;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.prerecorded;
                                    NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().InitData(worldData[j].thumbnail, worldData[j].youtubeUrl, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.prerecorded);
                                }
                                else if (!string.IsNullOrEmpty(worldData[j].asset_link))
                                {
                                    worldInfos[i].thumbnail = worldData[j].thumbnail;
                                    worldInfos[i].VideoLink = worldData[j].asset_link;
                                    worldInfos[i].videoType = PMY_VideoTypeRes.aws;
                                    NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().InitData(worldData[j].thumbnail, worldData[j].asset_link + compersionPrfex, worldInfos[i].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.aws);
                                }
                                isWithDes = true;
                                worldInfos[i].Title = worldData[j].title;
                                worldInfos[i].Aurthor = worldData[j].authorName;
                                worldInfos[i].Des = worldData[j].description;
                                worldInfos[i].url = worldData[j].descriptionHyperlink;
                            }
                        }
                        else if (worldData[j].media_type == "PDF")
                        {
                            worldInfos[i].Type = PMY_DataType.PDF;
                            worldInfos[i].pdfURL = worldData[j].pdf_url;
                            worldInfos[i].thumbnail = worldData[j].thumbnail;
                            NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().InitData(worldData[j].thumbnail, null, worldInfos[i].pmyRatio, PMY_DataType.PDF, PMY_VideoTypeRes.none);
                        }
                        //else if (worldData[j].media_type == "QUIZ")
                        //{
                        //    worldInfos[i].Type = PMY_DataType.Quiz;
                        //    worldInfos[i].thumbnail = worldData[j].thumbnail;
                        //    worldInfos[i].quiz_data = worldData[j].quiz_data;
                        //    NftPlaceholderList[i].GetComponent<PMY_VideoAndImage>().InitData(worldData[j].thumbnail, null, worldInfos[i].pmyRatio, PMY_DataType.Quiz, PMY_VideoTypeRes.none);
                        //}
                        break;
                    }
                    else
                    {
                        if (j == worldData.Count - 1)
                        {
                            NftPlaceholderList[i].gameObject.SetActive(false);
                            NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().TurnOffAllImageAndVideo();
                            Debug.Log("INDEX is off!");
                        }
                    }
                }
                if (!isNFTUploaded)
                {
                    NftPlaceholderList[i].gameObject.SetActive(false);
                    NftPlaceholderList[i].GetComponent<AR_VideoAndImage>().TurnOffAllImageAndVideo();
                }
            }
            allDataLoaded.Invoke();
        }

        public void LoadPrerecordedIfNoLongerLive(GameObject obj, string precorderUrl)
        {
            worldInfos[obj.GetComponent<AR_VideoAndImage>().id].VideoLink = precorderUrl;
            worldInfos[obj.GetComponent<AR_VideoAndImage>().id].videoType = PMY_VideoTypeRes.prerecorded;
            obj.GetComponent<AR_VideoAndImage>().InitData(null, precorderUrl, worldInfos[obj.GetComponent<AR_VideoAndImage>().id].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.prerecorded);
        }

        public void LoadLiveIfFirstTimeNotLoaded(GameObject obj, string url)
        {
            worldInfos[obj.GetComponent<AR_VideoAndImage>().id].VideoLink = url;
            worldInfos[obj.GetComponent<AR_VideoAndImage>().id].videoType = PMY_VideoTypeRes.islive;
            obj.GetComponent<AR_VideoAndImage>().InitData(null, url, worldInfos[obj.GetComponent<AR_VideoAndImage>().id].pmyRatio, PMY_DataType.Video, PMY_VideoTypeRes.islive);
        }

        public void SetInfo(PMY_Ratio ratio, string title, string aurthur, string des, string url, Texture2D image, PMY_DataType type, string videoLink, PMY_VideoTypeRes videoType, string pdfURL, QuizData quizData, int nftId = 0, AR_VideoAndImage.RoomType roomType = AR_VideoAndImage.RoomType.Stage, int roomNum = 1)
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
                nftHolder.pdfViewer_L.FileURL = pdfURL;
                nftHolder.pdfViewer_P.FileURL = pdfURL;
                Enable_PDF_Panel();
            }
            //else if (type == PMY_DataType.Quiz)
            //{
            //    quizPanel_L.GetComponent<PMY_QuizController>().SetQuizData(quizData);

            //    quizPanel_P.GetComponent<PMY_QuizController>().SetQuizData(quizData);
            //    EnableQuizPanel();
            //}
            else
            {
                // Setting Landscape Data
                nftHolder.ratioReferences[ratioId].l_image.gameObject.SetActive(true);
                nftHolder.ratioReferences[ratioId].p_image.gameObject.SetActive(true);
                nftHolder.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
                nftHolder.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
                if (ratioId < 4)
                {
                    nftHolder.ratioReferences[ratioId].l_Title.text = title;
                    nftHolder.ratioReferences[ratioId].l_Aurthur.text = aurthur;
                    nftHolder.ratioReferences[ratioId].l_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
                }
                if (type == PMY_DataType.Image)
                {
                    nftHolder.ratioReferences[ratioId].l_image.texture = image;
                    nftHolder.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
                }
                else
                {
                    nftHolder.ratioReferences[ratioId].l_image.gameObject.SetActive(false);
                    nftHolder.ratioReferences[ratioId].l_videoPlayer.url = videoLink;
                }

                // Setting Potraite Data
                if (ratioId < 4)
                {
                    nftHolder.ratioReferences[ratioId].p_Title.text = title;
                    nftHolder.ratioReferences[ratioId].p_Aurthur.text = aurthur;
                    nftHolder.ratioReferences[ratioId].p_Description.text = des + "\n" + "<link=" + url + "><u>" + url + "</u></link>";
                }
                nftHolder.ratioReferences[ratioId].p_image.texture = image;
                if (type == PMY_DataType.Image)
                {
                    nftHolder.ratioReferences[ratioId].p_image.texture = image;
                    nftHolder.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
                }
                else
                {
                    nftHolder.ratioReferences[ratioId].p_image.gameObject.SetActive(false);
                    nftHolder.ratioReferences[ratioId].p_videoPlayer.url = videoLink;
                }

                if (!ScreenOrientationManager._instance.isPotrait) // for Landscape
                {
                    nftHolder.LandscapeObj.SetActive(true);
                    nftHolder.PotraiteObj.SetActive(false);
                    nftHolder.ratioReferences[ratioId].l_obj.SetActive(true);
                    nftHolder.ratioReferences[ratioId].p_obj.SetActive(false);
                    if (type == PMY_DataType.Video)
                    {
                        nftHolder.ratioReferences[ratioId].l_Loader.SetActive(true);
                        nftHolder.ratioReferences[ratioId].p_Loader.SetActive(false);

                        if (videoType == PMY_VideoTypeRes.islive)
                        {
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = false;
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.enabled = false;
                            nftHolder.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);
                            nftHolder.ratioReferences[ratioId].l_LivePlayer.SetActive(true);

                            nftHolder.ratioReferences[ratioId].l_LivePlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(videoLink, true);
                        }
                        else if (videoType == PMY_VideoTypeRes.prerecorded)
                        {
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                            nftHolder.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(true);
                            nftHolder.ratioReferences[ratioId].l_LivePlayer.SetActive(false);

                            nftHolder.ratioReferences[ratioId].l_PrerecordedPlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(videoLink, false);
                        }
                        else if (videoType == PMY_VideoTypeRes.aws)
                        {
                            if (nftHolder.ratioReferences[ratioId].l_PrerecordedPlayer)
                                nftHolder.ratioReferences[ratioId].l_PrerecordedPlayer.SetActive(false);

                            if (nftHolder.ratioReferences[ratioId].l_LivePlayer)
                                nftHolder.ratioReferences[ratioId].l_LivePlayer.SetActive(false);

                            nftHolder.ratioReferences[ratioId].l_videoPlayer.GetComponent<RawImage>().enabled = true;
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.enabled = true;
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.url = videoLink;
                            nftHolder.ratioReferences[ratioId].l_videoPlayer.Play();
                        }

                        OnVideoEnlargeAction?.Invoke();
                    }
                }
                else // for Potraite
                {
                    nftHolder.LandscapeObj.SetActive(false);
                    nftHolder.PotraiteObj.SetActive(true);
                    nftHolder.ratioReferences[ratioId].l_obj.SetActive(false);
                    nftHolder.ratioReferences[ratioId].p_obj.SetActive(true);
                    if (type == PMY_DataType.Video)
                    {
                        nftHolder.ratioReferences[ratioId].l_Loader.SetActive(false);
                        nftHolder.ratioReferences[ratioId].p_Loader.SetActive(true);

                        if (videoType == PMY_VideoTypeRes.islive)
                        {
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = false;
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.enabled = false;
                            nftHolder.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                            nftHolder.ratioReferences[ratioId].p_LivePlayer.SetActive(true);

                            nftHolder.ratioReferences[ratioId].p_LivePlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(videoLink, true);
                        }
                        else if (videoType == PMY_VideoTypeRes.prerecorded)
                        {
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                            nftHolder.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(true);
                            nftHolder.ratioReferences[ratioId].p_LivePlayer.SetActive(false);

                            nftHolder.ratioReferences[ratioId].p_PrerecordedPlayer.GetComponent<StreamYoutubeVideo>().StreamYtVideo(videoLink, false);
                        }
                        else if (videoType == PMY_VideoTypeRes.aws)
                        {
                            nftHolder.ratioReferences[ratioId].p_PrerecordedPlayer.SetActive(false);
                            nftHolder.ratioReferences[ratioId].p_LivePlayer.SetActive(false);
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.GetComponent<RawImage>().enabled = true;
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.enabled = true;
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.url = videoLink;
                            nftHolder.ratioReferences[ratioId].p_videoPlayer.Play();

                        }

                        OnVideoEnlargeAction.Invoke();
                    }

                }
            }
            if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
            {
                GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
            }
            nftHolder.currentRoom = this;
            #region For firebase analytics
            SendCallAnalytics(nftId, roomType);         // firebase event calling in this method
            clickedNftInd = nftId;
            roomName = roomType.ToString();
            #endregion
        }

        public void SendCallAnalytics(int id = -1, AR_VideoAndImage.RoomType roomType = AR_VideoAndImage.RoomType.Stage)
        {
            // For firebase event
            string eventName = "";
            switch (roomType)
            {
                case AR_VideoAndImage.RoomType.Stage:
                    eventName = FirebaseTrigger.CL_NFT_THA_Stage.ToString() + "_" + (id + 1);
                    break;
                case AR_VideoAndImage.RoomType.FactoryTour:
                    eventName = FirebaseTrigger.CL_NFT_THA_Factory.ToString() + "_" + (id + 1);
                    break;
                case AR_VideoAndImage.RoomType.HomeConsulting:
                    eventName = FirebaseTrigger.CL_NFT_THA_Consult.ToString() + "_" + (id + 1);
                    break;
                case AR_VideoAndImage.RoomType.Architectural:
                    eventName = FirebaseTrigger.CL_NFT_THA_Architec.ToString() + "_" + (id + 1);
                    break;
                case AR_VideoAndImage.RoomType.LandInfo:
                    eventName = FirebaseTrigger.CL_NFT_THA_LandInfo.ToString() + "_" + (id + 1);
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
            nftHolder.ratioReferences[ratioId].l_obj.SetActive(false);
            nftHolder.ratioReferences[ratioId].p_obj.SetActive(false);
            nftHolder.ratioReferences[ratioId].p_Loader.SetActive(false);
            nftHolder.ratioReferences[ratioId].l_Loader.SetActive(false);
            nftHolder.LandscapeObj.SetActive(false);
            nftHolder.PotraiteObj.SetActive(false);
            if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
            {
                GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
            }
        }

        private void ErrorOnVideo(VideoPlayer source, string message)
        {
            if (videoRetry <= RetryChances)
            {
                videoRetry++;
                SetInfo(_Ratio, _Title, _Aurthor, _Des, _URL, _image, _Type, _VideoLink, _videoType, _pdfURL, _quiz_data);
            }
            else
            {
                videoRetry = 0;
                CloseInfoPop();
            }
        }

        private void VideoReady(VideoPlayer source)
        {
            nftHolder.ratioReferences[ratioId].p_Loader.SetActive(false);
            nftHolder.ratioReferences[ratioId].l_Loader.SetActive(false);
            videoRetry = 0;
        }

        public void Enable_PDF_Panel()
        {
            if (!ScreenOrientationManager._instance.isPotrait)
                nftHolder.pdfPanel_L.SetActive(true);
            else
                nftHolder.pdfPanel_P.SetActive(true);

            ReferencesForGamePlay.instance.eventSystemObj.SetActive(false);
            PlayerCameraController.instance.isReturn = true;
        }

        public void EnableControlls()
        {
            ActionOnExitBtn();
            if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
            {
                GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
            }

            ReferencesForGamePlay.instance.eventSystemObj.SetActive(true);
            PlayerCameraController.instance.isReturn = false;
        }

        private void OnDisable()
        {
            if (nftHolder.VideoPlayers.Count > 0)
                foreach (VideoPlayer player in nftHolder.VideoPlayers)
                {
                    player.errorReceived -= ErrorOnVideo;
                    player.prepareCompleted -= VideoReady;
                }
            InRoomSoundHandler.playerInRoom -= UpdateNFTData;
        }

    }

    [Serializable]
    public class PMY_WorldData
    {
        public string[] Title;
        public string[] Aurthor;
        public string[] Des;
        public string url;
        public string pdfURL;
        public QuizData quiz_data;
        public string thumbnail;
        public PMY_DataType Type;
        public Sprite WorldImage;
        public Texture2D Texture;
        public string VideoLink;
        public bool isAWSVideo;
        public bool isLiveVideo;
        public bool isPrerecordedVideo;
        public PMY_Ratio pmyRatio;
        public PMY_VideoTypeRes videoType;
    }
    public enum PMY_DataType
    {
        Image,
        Video,
        PDF
    }
    public enum PMY_VideoTypeRes
    {
        none,
        islive,
        prerecorded,
        aws
    }
    public enum PMY_Ratio
    {
        OneXOneWithDes,
        SixteenXNineWithDes,
        NineXSixteenWithDes,
        FourXThreeWithDes,

        OneXOneWithoutDes,
        SixteenXNineWithoutDes,
        NineXSixteenWithoutDes,
        FourXThreeWithoutDes,
    }

    [Serializable]
    public class RatioRef
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
    public class PMY_Json
    {
        public bool success;
        public List<PMY_Asset> data;
        public string msg;
    }
    public class PMY_Asset
    {
        public int id;
        public int worldId;
        public int index;
        public string asset_link;
        public bool check;
        public string[] authorName;
        public string[] description;
        public string descriptionHyperlink;
        public string[] title;
        public string ratio;
        public string thumbnail;
        public string media_type;
        public string pdf_url;
        public QuizData quiz_data;
        public string user_id;
        public string event_id;
        public string category;
        public bool youtubeUrlCheck;
        public string youtubeUrl;
        public DateTime createdAt;
        public DateTime updatedAt;
        public string event_env_class;
    }
    public class QuizData
    {
        public string question;
        public List<string> answer;
        public string correct;
    }

}