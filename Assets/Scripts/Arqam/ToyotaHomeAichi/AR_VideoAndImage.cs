using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace Toyota
{
    public class AR_VideoAndImage : MonoBehaviour
    {
        public enum NFT_Type { TwoD_View, MainScreen }
        public NFT_Type NFT;
        //public bool isAddBtnCompOnScreen = true;
        [Space(5)]
        public int id;

        private Texture2D _texture;

        public GameObject imgVideo16x9;
        public GameObject imgVideo9x16;
        public GameObject imgVideo1x1;
        public GameObject imgVideo4x3;

        //public GameObject liveVideoPlayer;
        public GameObject videoParent;
        public string videoLink;
        public string imageLink;

        public PMY_VideoTypeRes _videoType;
        public PMY_Ratio _imgVideoRatio;
        [SerializeField] bool ApplyImageOnTexture; // If image is not on Sqaure
        [SerializeField] MeshRenderer imageMesh;
        [SerializeField] Material imageMat;

        [SerializeField] bool applyVideoMesh; // If play video on mesh 
        [SerializeField] VideoPlayer videoMesh;

        public GameObject imgVideoFrame16x9;
        public GameObject imgVideoFrame9x16;
        public GameObject imgVideoFrame1x1;
        public GameObject imgVideoFrame4x3;

        //public bool isMultipleScreen = false;

        public enum RoomType
        {
            Stage,
            FactoryTour,
            HomeConsulting,
            Architectural,
            LandInfo
        }

        [Space(5)]
        [Header("For Firebase Enum")]
        public RoomType roomType;
        [Space(5)]
        public UnityEvent nftStartAction;
        public UnityEvent<int> enableFrame;
        public UnityEvent disableFrame;
        public AR_Nft_Manager nftMAnager;
        [Range(1, 5)]
        public int roomNumber = 1;
        private StreamYoutubeVideo streamYoutubeVideo;

        private void Awake()
        {
            imgVideo16x9.gameObject.SetActive(false);
            imgVideo9x16.gameObject.SetActive(false);
            imgVideo1x1.gameObject.SetActive(false);
            imgVideo4x3.gameObject.SetActive(false);
        }
        private void Start()
        {
            imgVideo16x9.AddComponent<Button>();
            imgVideo16x9.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo9x16.AddComponent<Button>();
            imgVideo9x16.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo1x1.AddComponent<Button>();
            imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            imgVideo4x3.AddComponent<Button>();
            imgVideo4x3.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

            if (this.GetComponent<StreamYoutubeVideo>() != null)
            {
                streamYoutubeVideo = this.GetComponent<StreamYoutubeVideo>();
            }
        }
        private void OnDisable()
        {
            try
            {
                DestroyImmediate(imgVideo16x9.GetComponent<RawImage>().texture);
            }
            catch (Exception ex)
            {
                Debug.Log("<color=red>" + ex.Message + "</color>");
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
            else if (dataType == PMY_DataType.PDF)
                SetPDF();
            //else if (dataType == PMY_DataType.Quiz)
            //    SetQuiz();
        }

        void SetPDF()
        {
            if (imgVideo16x9)
                imgVideo16x9.SetActive(false);
            if (imgVideo9x16)
                imgVideo9x16.SetActive(false);
            if (imgVideo1x1)
                imgVideo1x1.SetActive(false);
            if (imgVideo4x3)
                imgVideo4x3.SetActive(false);
            //if (liveVideoPlayer)
            //    liveVideoPlayer.SetActive(false);
            //if (preRecordedPlayer)
            //    preRecordedPlayer.SetActive(false);

            SetThumbail(imageLink);
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
            //if (liveVideoPlayer)
            //    liveVideoPlayer.SetActive(false);
            //if (preRecordedPlayer)
            //    preRecordedPlayer.SetActive(false);

            SetThumbail(imageLink);
        }

        void SetThumbail(string _imageLink)
        {
            StartCoroutine(GetSprite(_imageLink, (response) =>
            {
                if (nftMAnager && response != null)
                    nftMAnager.NFTLoadedSprites.Add(response);

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

                        enableFrame.Invoke(0);
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

                        enableFrame.Invoke(1);
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
                        enableFrame.Invoke(2);
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
                        enableFrame.Invoke(3);
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
            //liveVideoPlayer.SetActive(false);
            //preRecordedPlayer.SetActive(false);
            imgVideo16x9.SetActive(false);
            imgVideo9x16.SetActive(false);
            imgVideo1x1.SetActive(false);
            imgVideo4x3.SetActive(false);
            //liveVideoPlayer.SetActive(false);
            //preRecordedPlayer.SetActive(false);
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
                        callback(_texture);
                    }
                }
                www.Dispose();
                nftStartAction.Invoke();
            }
        }

        private RenderTexture renderTexture_temp;
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
            //if (liveVideoPlayer)
            //    liveVideoPlayer.SetActive(false);

            if (NFT.Equals(NFT_Type.MainScreen))
            {
                if (_videoType == PMY_VideoTypeRes.islive) //&& liveVideoPlayer
                {
                    //nftMAnager.videoRenderObject = liveVideoPlayer;
                    //if (liveVideoPlayer)
                    //    liveVideoPlayer.SetActive(true);

                    //if (streamYoutubeVideo != null)
                    //    streamYoutubeVideo.StreamYtVideo(videoLink, true);
                    videoParent.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, true);
                }
                else if (_videoType == PMY_VideoTypeRes.prerecorded)
                {
                    SoundController.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
                    SoundSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
                    SoundSettings.soundManagerSettings.setNewSliderValues();

                    RenderTexture renderTexture = new RenderTexture(NFT_Holder_Manager.instance.renderTexture_16x9);
                    //nftMAnager.videoRenderObject = imgVideo16x9;
                    //renderTexture_temp = renderTexture;
                    imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    videoParent.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, false);
                    //if (isMultipleScreen)
                    //{
                    //    for (int i = 0; i < imgVideo16x9.transform.childCount; i++)
                    //    {
                    //        imgVideo16x9.transform.GetChild(i).GetComponent<RawImage>().texture = renderTexture;
                    //        imgVideo16x9.transform.GetChild(i).GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    //    }
                    //}

                    //if (streamYoutubeVideo != null)
                    //    streamYoutubeVideo.StreamYtVideo(videoLink, false);
                    //imgVideo16x9.GetComponent<VideoPlayer>().playOnAwake = true;
                    //imgVideo16x9.SetActive(true);
                    //if (imgVideoFrame16x9)
                    //{
                    //    EnableImageVideoFrame(imgVideoFrame16x9);
                    //}
                    //if (!isAddBtnCompOnScreen)
                    //    imgVideo16x9.GetComponent<Button>().enabled = false;
                }
                else if (_videoType == PMY_VideoTypeRes.aws)
                {
                    SoundController.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
                    SoundSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
                    SoundSettings.soundManagerSettings.setNewSliderValues();

                    videoParent.SetActive(false);
                    imgVideo16x9.SetActive(true);
                    RenderTexture renderTexture = new RenderTexture(NFT_Holder_Manager.instance.renderTexture_16x9);
                    imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo16x9.GetComponent<VideoPlayer>().isLooping = true;
                    imgVideo16x9.GetComponent<VideoPlayer>().Play();
                }
            }
            else if (NFT.Equals(NFT_Type.TwoD_View))
            {
                SetThumbail(imageLink);
            }
        }


        public void OpenWorldInfo()
        {
            if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;
            if (PlayerController.isJoystickDragging == true)
                return;

            if (nftMAnager != null) //&& _videoType != PMY_VideoTypeRes.islive
            {
                if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
                {
                    nftMAnager.SetInfo(_imgVideoRatio, nftMAnager.worldInfos[id].Title[0], nftMAnager.worldInfos[id].Aurthor[0], nftMAnager.worldInfos[id].Des[0], nftMAnager.worldInfos[id].url, _texture, nftMAnager.worldInfos[id].Type, nftMAnager.worldInfos[id].VideoLink, nftMAnager.worldInfos[id].videoType,
                        nftMAnager.worldInfos[id].pdfURL, nftMAnager.worldInfos[id].quiz_data, id, roomType, roomNumber);
                }
                else if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
                {
                    nftMAnager.SetInfo(_imgVideoRatio, nftMAnager.worldInfos[id].Title[1], nftMAnager.worldInfos[id].Aurthor[1], nftMAnager.worldInfos[id].Des[1], nftMAnager.worldInfos[id].url, _texture, nftMAnager.worldInfos[id].Type, nftMAnager.worldInfos[id].VideoLink, nftMAnager.worldInfos[id].videoType,
                        nftMAnager.worldInfos[id].pdfURL, nftMAnager.worldInfos[id].quiz_data, id, roomType, roomNumber);
                }
            }
        }


        public void EraseDownloadedData()
        {
            if (imgVideo16x9.GetComponent<RawImage>().texture != null)
            {
                DestroyImmediate(imgVideo16x9.GetComponent<RawImage>().texture, true);
                imgVideo16x9.SetActive(false);
            }
            else if (imgVideo9x16.GetComponent<RawImage>().texture != null)
            {
                DestroyImmediate(imgVideo9x16.GetComponent<RawImage>().texture, true);
                imgVideo9x16.SetActive(false);
            }
            else if (imgVideo1x1.GetComponent<RawImage>().texture != null)
            {

                DestroyImmediate(imgVideo1x1.GetComponent<RawImage>().texture, true);
                imgVideo1x1.SetActive(false);
            }
            else if (imgVideo4x3.GetComponent<RawImage>().texture != null)
            {
                DestroyImmediate(imgVideo4x3.GetComponent<RawImage>().texture, true);
                imgVideo4x3.SetActive(false);
            }
            //if (liveVideoPlayer.activeSelf)
            //    liveVideoPlayer.SetActive(false);
            disableFrame.Invoke();
        }


    }
}
