using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Toyota
{
    public class AR_VideoAndImage : MonoBehaviour
    {
        public int id;

        private Texture2D _texture;

        public GameObject imgVideo16x9;
        public GameObject imgVideo9x16;
        public GameObject imgVideo1x1;
        public GameObject imgVideo4x3;

        public GameObject liveVideoPlayer;
        public GameObject preRecordedPlayer;


        public string videoLink;
        public string imageLink;

        public PMY_VideoTypeRes _videoType;
        public PMY_Ratio _imgVideoRatio;
        [SerializeField] bool ApplyImageOnTexture; // If image is not on Sqaure
        [SerializeField] MeshRenderer imageMesh;
        [SerializeField] Material imageMat;

        [SerializeField] bool applyVideoMesh; // If play video on mesh 
        [SerializeField] VideoPlayer videoMesh;

        //public string firebaseEventName = "";
        // Start is called before the first frame update


        public GameObject imgVideoFrame16x9;
        public GameObject imgVideoFrame9x16;
        public GameObject imgVideoFrame1x1;
        public GameObject imgVideoFrame4x3;

        public bool isMultipleScreen = false;
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
        [Space(5)]
        public UnityEvent nftStartAction;
        public UnityEvent<int> enableFrame;

        [SerializeField] AR_Nft_Manager nftMAnager;

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

            //if (nftMAnager.PMY_RoomIdFromXanaConstant)
            //    StartCoroutine(UpdateRoomType()); 
        }

        //IEnumerator UpdateRoomType()
        //{
        //    yield return new WaitForSeconds(1f);

        //    switch (nftMAnager.PMY_RoomId)
        //    {
        //        case 8:
        //            roomType = RoomType.RoomA_1;
        //            break;
        //        case 9:
        //            roomType = RoomType.RoomA_2;
        //            break;
        //    }
        //}

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
            if (liveVideoPlayer)
                liveVideoPlayer.SetActive(false);
            if (preRecordedPlayer)
                preRecordedPlayer.SetActive(false);

            SetThumbail(imageLink);
            if (isCreateFrame)
                CreateFrame();   //create frame
        }
        //void SetQuiz()
        //{
        //    if (imgVideo16x9)
        //        imgVideo16x9.SetActive(false);
        //    if (imgVideo9x16)
        //        imgVideo9x16.SetActive(false);
        //    if (imgVideo1x1)
        //        imgVideo1x1.SetActive(false);
        //    if (imgVideo4x3)
        //        imgVideo4x3.SetActive(false);
        //    if (liveVideoPlayer)
        //        liveVideoPlayer.SetActive(false);
        //    if (preRecordedPlayer)
        //        preRecordedPlayer.SetActive(false);

        //    SetThumbail(imageLink);
        //    if (isCreateFrame)
        //        CreateFrame();   //create frame
        //}
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

            SetThumbail(imageLink);
            if (isCreateFrame)
                CreateFrame();   //create frame
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
                        callback(_texture);
                    }
                }
                www.Dispose();
                nftStartAction.Invoke();
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
                nftMAnager.videoRenderObject = liveVideoPlayer;
                if (liveVideoPlayer)
                    liveVideoPlayer.SetActive(true);
                liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
                liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
                liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
            }
            else if (_videoType == PMY_VideoTypeRes.prerecorded && preRecordedPlayer)
            {
                RenderTexture renderTexture = new RenderTexture(nftMAnager.renderTexture_16x9);
                nftMAnager.videoRenderObject = imgVideo16x9;
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
                SetThumbail(imageLink);
            }

            if (nftMAnager && renderTexture_temp != null)
                nftMAnager.NFTLoadedVideos.Add(renderTexture_temp);

            if (isCreateFrame)
                CreateFrame();   //create frame
        }

        private void CreateFrame()
        {
            GameObject frame = AHFrameManager.instance.ref_PMYObjectPooler.GetPooledObjectFrame(_imgVideoRatio);
            frame.transform.SetParent(this.gameObject.transform);
            frame.transform.position = transform.position;
            frame.SetActive(true);
            frame.transform.localPosition = new Vector3(AHFrameManager.instance.frameLocalPos.x, AHFrameManager.instance.frameLocalPos.y, AHFrameManager.instance.frameLocalPos.z);
            frame.transform.localEulerAngles = AHFrameManager.instance.frameLocalRot;
            frame.transform.localScale = new Vector3(AHFrameManager.instance.frameLocalScale.x, AHFrameManager.instance.frameLocalScale.y, AHFrameManager.instance.frameLocalScale.z);

        }

        public void OpenWorldInfo()
        {
            if (SelfieController.Instance.m_IsSelfieFeatureActive) return;
            if (PlayerControllerNew.isJoystickDragging == true)
                return;

            if (nftMAnager != null && _videoType != PMY_VideoTypeRes.islive)
            {
                if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
                {
                    nftMAnager.SetInfo(_imgVideoRatio, nftMAnager.worldInfos[id].Title[0], nftMAnager.worldInfos[id].Aurthor[0], nftMAnager.worldInfos[id].Des[0], nftMAnager.worldInfos[id].url, _texture, nftMAnager.worldInfos[id].Type, nftMAnager.worldInfos[id].VideoLink, nftMAnager.worldInfos[id].videoType,
                        nftMAnager.worldInfos[id].pdfURL, nftMAnager.worldInfos[id].quiz_data, id, roomType);
                }
                else if (CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
                {
                    nftMAnager.SetInfo(_imgVideoRatio, nftMAnager.worldInfos[id].Title[1], nftMAnager.worldInfos[id].Aurthor[1], nftMAnager.worldInfos[id].Des[1], nftMAnager.worldInfos[id].url, _texture, nftMAnager.worldInfos[id].Type, nftMAnager.worldInfos[id].VideoLink, nftMAnager.worldInfos[id].videoType,
                        nftMAnager.worldInfos[id].pdfURL, nftMAnager.worldInfos[id].quiz_data, id, roomType);

                }
            }
        }
    }
}
