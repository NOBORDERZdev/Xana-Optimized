using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using System;
//using System.Diagnostics.Eventing.Reader;

namespace PMY
{
    public class PMY_VideoAndImage_Extension : MonoBehaviour
    {
        public enum NFT_Type { TwoD_View, MainScreen }
        public NFT_Type NFT;
        public bool isAddBtnComponent = false;
        public int id;

        private Texture2D _texture;

        public GameObject imgVideo16x9;
        public GameObject imgVideo9x16;
        public GameObject imgVideo1x1;
        public GameObject imgVideo4x3;

        public GameObject videoParent;
        //public GameObject liveVideoPlayer;
        //public GameObject preRecordedPlayer;

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

        //public bool isMultipleScreen = false;
        public bool isCreateFrame = true;

        public enum RoomType
        {
            PMYLobby,
            RoomA_1,
            RoomA_2,
            Gallery
        }
        [Space(5)]
        [Header("For Firebase Enum")]
        public RoomType roomType;

        //private StreamYoutubeVideo streamYoutubeVideo;

        private void Start()
        {
            if (isAddBtnComponent)
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

            if (PMY_Nft_Manager_Extension.Instance.PMY_RoomIdFromXanaConstant)
                StartCoroutine(UpdateRoomType());
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

        IEnumerator UpdateRoomType()
        {
            yield return new WaitForSeconds(1f);

            switch (PMY_Nft_Manager_Extension.Instance.PMY_RoomId)
            {
                case 8:
                    roomType = RoomType.RoomA_1;
                    break;
                case 9:
                    roomType = RoomType.RoomA_2;
                    break;
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
            else if (dataType == PMY_DataType.Quiz)
                SetQuiz();
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
            if (isCreateFrame)
                CreateFrame();   //create frame
        }
        void SetQuiz()
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
            if (isCreateFrame)
                CreateFrame();   //create frame
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
            if (isCreateFrame)
                CreateFrame();   //create frame
        }

        void SetThumbail(string _imageLink)
        {
            StartCoroutine(GetSprite(_imageLink, (response) =>
            {
                if (PMY_Nft_Manager_Extension.Instance && response != null)
                    PMY_Nft_Manager_Extension.Instance.NFTLoadedSprites.Add(response);

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
            if (imgVideo16x9)
                imgVideo16x9.SetActive(false);
            if (imgVideo9x16)
                imgVideo9x16.SetActive(false);
            if (imgVideo1x1)
                imgVideo1x1.SetActive(false);
            if (imgVideo4x3)
                imgVideo4x3.SetActive(false);

            if (imgVideo16x9.GetComponent<VideoPlayer>().targetTexture != null)
            {
                imgVideo16x9.GetComponent<VideoPlayer>().targetTexture.Release();
                imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = null;
            }
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
            }
        }

        //RenderTexture renderTexture_temp;
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
            //if (preRecordedPlayer)
            //    preRecordedPlayer.SetActive(false);

            if (NFT.Equals(NFT_Type.MainScreen))
            {
                if (_videoType == PMY_VideoTypeRes.islive) //&& liveVideoPlayer
                {
                    //PMY_Nft_Manager.Instance.videoRenderObject = liveVideoPlayer;
                    //if (liveVideoPlayer)
                    //    liveVideoPlayer.SetActive(true);

                    videoParent.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, true);
                    //if (streamYoutubeVideo != null)
                    //    streamYoutubeVideo.StreamYtVideo(videoLink, true);
                }
                else if (_videoType == PMY_VideoTypeRes.prerecorded)
                {
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_16x9);
                    //PMY_Nft_Manager.Instance.videoRenderObject = imgVideo16x9;
                    //renderTexture_temp = renderTexture;
                    imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
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

                    videoParent.GetComponent<AdvancedYoutubePlayer>().StreamYtVideo(videoLink, false);
                    //imgVideo16x9.SetActive(true);
                    //if (imgVideoFrame16x9)
                    //{
                    //    EnableImageVideoFrame(imgVideoFrame16x9);
                    //}
                }
                else if (_videoType == PMY_VideoTypeRes.aws)
                {
                    videoParent.SetActive(false);
                    imgVideo16x9.SetActive(true);
                    RenderTexture renderTexture = new RenderTexture(PMY_Nft_Manager.Instance.renderTexture_16x9);
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

            if (isCreateFrame)
                CreateFrame();   //create frame
        }

        private void CreateFrame()
        {
            GameObject frame = PMYFrameManager.instance.ref_PMYObjectPooler.GetPooledObjectFrame(_imgVideoRatio);
            frame.transform.SetParent(this.gameObject.transform);
            frame.transform.position = transform.position;
            frame.SetActive(true);
            frame.transform.localPosition = new Vector3(PMYFrameManager.instance.frameLocalPos.x, PMYFrameManager.instance.frameLocalPos.y, PMYFrameManager.instance.frameLocalPos.z);
            frame.transform.localEulerAngles = PMYFrameManager.instance.frameLocalRot;
            frame.transform.localScale = new Vector3(PMYFrameManager.instance.frameLocalScale.x, PMYFrameManager.instance.frameLocalScale.y, PMYFrameManager.instance.frameLocalScale.z);

        }

        public void OpenWorldInfo()
        {
            if (SelfieController.Instance.m_IsSelfieFeatureActive) return;
            if (PlayerControllerNew.isJoystickDragging == true)
                return;

            if (PMY_Nft_Manager_Extension.Instance != null && _videoType != PMY_VideoTypeRes.islive)
            {
                if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
                {
                    PMY_Nft_Manager_Extension.Instance.SetInfo(_imgVideoRatio, PMY_Nft_Manager_Extension.Instance.worldInfos[id].Title[0], PMY_Nft_Manager_Extension.Instance.worldInfos[id].Aurthor[0], PMY_Nft_Manager_Extension.Instance.worldInfos[id].Des[0], PMY_Nft_Manager_Extension.Instance.worldInfos[id].url, _texture, PMY_Nft_Manager_Extension.Instance.worldInfos[id].Type, PMY_Nft_Manager_Extension.Instance.worldInfos[id].VideoLink, PMY_Nft_Manager_Extension.Instance.worldInfos[id].videoType,
                        PMY_Nft_Manager_Extension.Instance.worldInfos[id].pdfURL, PMY_Nft_Manager_Extension.Instance.worldInfos[id].quiz_data, id, roomType);
                }
                else if (CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
                {
                    PMY_Nft_Manager_Extension.Instance.SetInfo(_imgVideoRatio, PMY_Nft_Manager_Extension.Instance.worldInfos[id].Title[1], PMY_Nft_Manager_Extension.Instance.worldInfos[id].Aurthor[1], PMY_Nft_Manager_Extension.Instance.worldInfos[id].Des[1], PMY_Nft_Manager_Extension.Instance.worldInfos[id].url, _texture, PMY_Nft_Manager_Extension.Instance.worldInfos[id].Type, PMY_Nft_Manager_Extension.Instance.worldInfos[id].VideoLink, PMY_Nft_Manager_Extension.Instance.worldInfos[id].videoType,
                        PMY_Nft_Manager_Extension.Instance.worldInfos[id].pdfURL, PMY_Nft_Manager_Extension.Instance.worldInfos[id].quiz_data, id, roomType);

                }
            }
        }

    }
}
