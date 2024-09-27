using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using SuperStar.Helpers;

public class SummitVideoAndImageController : MonoBehaviour
{
    public int id;
    public GameObject imgVideo16x9;
    public GameObject imgVideo9x16;
    public GameObject imgVideo1x1;
    public GameObject imgVideo4x3;
    public GameObject liveVideoPlayer;
    public string videoLink;
    public string imageLink;
    public VideoTypeRes _videoType;
    public JjRatio _imgVideoRatio;
    public string firebaseEventName = "";
    public GameObject imgVideoFrame16x9;
    public GameObject imgVideoFrame9x16;
    public GameObject imgVideoFrame1x1;
    public GameObject imgVideoFrame4x3;
    public bool isMultipleScreen = false;
    public bool isCreateFrame = true;
    public enum MuseumType
    {
        AtomMuseum,
        RentalSpace
    }
    [Space(5)]
    [Header("For Firebase Enum")]
    public MuseumType museumType;
    [Space(5)]
    [Header("For Firebase roomNumber")]
    [Range(0, 12)]
    public int roomNumber = 1;
    [SerializeField] private bool isForceAudioOn = false;
    [SerializeField] bool ApplyImageOnTexture; // If image is not on Sqaure
    [SerializeField] MeshRenderer imageMesh;
    [SerializeField] Material imageMat;
    [SerializeField] bool applyVideoMesh; // If play video on mesh 
    [SerializeField] VideoPlayer videoMesh;
    [SerializeField]
    private Texture _texture;
    private StreamYoutubeVideo streamYoutubeVideo;
    RenderTexture renderTexture_temp;

    private void Start()
    {
        imgVideo16x9.AddComponent<Button>();
        //JjWorldInfo jjWorldInfo=imgVideo16x9.GetComponent<JjWorldInfo>();
        imgVideo16x9.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

        imgVideo9x16.AddComponent<Button>();
        //JjWorldInfo jjWorldInfo1 = imgVideo9x16.GetComponent<JjWorldInfo>();
        imgVideo9x16.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

        imgVideo1x1.AddComponent<Button>();
        //JjWorldInfo jjWorldInfo2 = imgVideo1x1.GetComponent<JjWorldInfo>();
        imgVideo1x1.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());

        imgVideo4x3.AddComponent<Button>();
        //JjWorldInfo jjWorldInfo3 = imgVideo4x3.GetComponent<JjWorldInfo>();
        imgVideo4x3.GetComponent<Button>().onClick.AddListener(() => OpenWorldInfo());
        if (this.GetComponent<StreamYoutubeVideo>() != null)
        {
            streamYoutubeVideo = this.GetComponent<StreamYoutubeVideo>();
        }
        _texture = new Texture2D(2, 2);
    }

    public void InitData(string imageurl, string videourl, JjRatio imgvideores, DataType dataType, VideoTypeRes videoType)
    {
        imageLink = imageurl;
        videoLink = videourl;
        _imgVideoRatio = imgvideores;
        _videoType = videoType;
        if (dataType == DataType.Image)
            SetImage();
        else if (dataType == DataType.Video)
            SetVideo();
    }
    public void TurnOffAllImageAndVideo()
    {
        imgVideo16x9.SetActive(false);
        imgVideo9x16.SetActive(false);
        imgVideo1x1.SetActive(false);
        imgVideo4x3.SetActive(false);
        liveVideoPlayer.SetActive(false);
        //preRecordedPlayer.SetActive(false);
        imgVideo16x9.SetActive(false);
        imgVideo9x16.SetActive(false);
        imgVideo1x1.SetActive(false);
        imgVideo4x3.SetActive(false);
        liveVideoPlayer.SetActive(false);
        //preRecordedPlayer.SetActive(false);
    }
    public void OpenWorldInfo()
    {
        if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;
        if (PlayerController.isJoystickDragging == true)
            return;
        //SummitDomeNFTDataController.Instance.firebaseEventName = firebaseEventName;
        if (SummitDomeNFTDataController.Instance != null /*&& _videoType != VideoTypeRes.islive*/)
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                SummitDomeNFTDataController.Instance.SetInfo(_imgVideoRatio, SummitDomeNFTDataController.Instance.worldInfos[id].Title[0], SummitDomeNFTDataController.Instance.worldInfos[id].Aurthor[0], SummitDomeNFTDataController.Instance.worldInfos[id].Des[0], SummitDomeNFTDataController.Instance.worldInfos[id].url, _texture, SummitDomeNFTDataController.Instance.worldInfos[id].Type, SummitDomeNFTDataController.Instance.worldInfos[id].VideoLink, SummitDomeNFTDataController.Instance.worldInfos[id].videoType, id, museumType, roomNumber);
            }
            else if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                SummitDomeNFTDataController.Instance.SetInfo(_imgVideoRatio, SummitDomeNFTDataController.Instance.worldInfos[id].Title[0], SummitDomeNFTDataController.Instance.worldInfos[id].Aurthor[0], SummitDomeNFTDataController.Instance.worldInfos[id].Des[0], SummitDomeNFTDataController.Instance.worldInfos[id].url, _texture, SummitDomeNFTDataController.Instance.worldInfos[id].Type, SummitDomeNFTDataController.Instance.worldInfos[id].VideoLink, SummitDomeNFTDataController.Instance.worldInfos[id].videoType, id, museumType, roomNumber);

            }
        }
        if (SoundSettings.soundManagerSettings.bgmSource && !(string.IsNullOrEmpty(videoLink)))
        {
            SoundSettings.soundManagerSettings.bgmSource.mute = true;
        }
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

        if (_imgVideoRatio == JjRatio.SixteenXNineWithDes || _imgVideoRatio == JjRatio.SixteenXNineWithoutDes)
        {
            if (imgVideo16x9)
            {
                SetSprite(imageLink, imgVideo16x9.GetComponent<RawImage>());
            }
        }
        else if (_imgVideoRatio == JjRatio.NineXSixteenWithDes || _imgVideoRatio == JjRatio.NineXSixteenWithoutDes)
        {
            if (imgVideo9x16)
            {
                SetSprite(imageLink, imgVideo9x16.GetComponent<RawImage>());
            }
        }
        else if (_imgVideoRatio == JjRatio.OneXOneWithDes || _imgVideoRatio == JjRatio.OneXOneWithoutDes)
        {
            if (imgVideo1x1)
            {
                SetSprite(imageLink, imgVideo1x1.GetComponent<RawImage>());
            }
        }
        else if (_imgVideoRatio == JjRatio.FourXThreeWithDes || _imgVideoRatio == JjRatio.FourXThreeWithoutDes)
        {
            if (imgVideo4x3)
            {
                SetSprite(imageLink, imgVideo4x3.GetComponent<RawImage>());
            }
        }

        if (isCreateFrame)
            CreateFrame();   //create frame
    }

    void EnableImageVideoFrame(GameObject frameToEnable)
    {
        imgVideoFrame16x9.SetActive(false);
        imgVideoFrame9x16.SetActive(false);
        imgVideoFrame1x1.SetActive(false);
        imgVideoFrame4x3.SetActive(false);

        frameToEnable.SetActive(true);
    }

    private void SetSprite(string path, RawImage img)
    {
        path = ModifyImageUrl(path, 200);
        img.enabled = true;
        if (AssetCache.Instance.HasFile(path))
        {
            AssetCache.Instance.LoadTexture2DIntoRawImage(img, path, changeAspectRatio: true);
            img.gameObject.SetActive(true);
            _texture = img.texture;
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(path, path, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadTexture2DIntoRawImage(img, path, changeAspectRatio: true);
                    img.gameObject.SetActive(true);
                    _texture = img.texture;
                }
            });
        }

        if(_texture != null)
        {
            if (SummitDomeNFTDataController.Instance && img != null)
                SummitDomeNFTDataController.Instance.NFTLoadedSprites.Add(img.texture);

            if (ApplyImageOnTexture && imageMesh != null)
            {
                imageMesh.material = imageMat;
                imageMesh.material.mainTexture = img.texture;
            }
            else if (_imgVideoRatio == JjRatio.SixteenXNineWithDes || _imgVideoRatio == JjRatio.SixteenXNineWithoutDes)
            {
                if (imgVideoFrame16x9)
                {
                    EnableImageVideoFrame(imgVideoFrame16x9);
                }
                imgVideo16x9.SetActive(true);
                imgVideo16x9.GetComponent<RawImage>().texture = img.texture;
                imgVideo16x9.GetComponent<VideoPlayer>().enabled = false;
            }
            else if (_imgVideoRatio == JjRatio.NineXSixteenWithDes || _imgVideoRatio == JjRatio.NineXSixteenWithoutDes)
            {
                if (imgVideoFrame9x16)
                {
                    EnableImageVideoFrame(imgVideoFrame9x16);
                }
                imgVideo9x16.SetActive(true);
                imgVideo9x16.GetComponent<RawImage>().texture = img.texture;
                imgVideo9x16.GetComponent<VideoPlayer>().enabled = false;
            }
            else if (_imgVideoRatio == JjRatio.OneXOneWithDes || _imgVideoRatio == JjRatio.OneXOneWithoutDes)
            {
                if (imgVideoFrame1x1)
                {
                    EnableImageVideoFrame(imgVideoFrame1x1);
                }
                imgVideo1x1.SetActive(true);
                imgVideo1x1.GetComponent<RawImage>().texture = img.texture;
                imgVideo1x1.GetComponent<VideoPlayer>().enabled = false;
            }
            else if (_imgVideoRatio == JjRatio.FourXThreeWithDes || _imgVideoRatio == JjRatio.FourXThreeWithoutDes)
            {
                if (imgVideoFrame4x3)
                {
                    EnableImageVideoFrame(imgVideoFrame4x3);
                }
                imgVideo4x3.SetActive(true);
                imgVideo4x3.GetComponent<RawImage>().texture = img.texture;
                imgVideo4x3.GetComponent<VideoPlayer>().enabled = false;
            }
        }
    }

    string ModifyImageUrl(string url, int newWidth)
    {
        string baseUrl = url.Split('?')[0]; // Get everything before the '?'
        string parameters = $"width={newWidth}";

        // Return the modified URL
        return $"{baseUrl}?{parameters}";
    }

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

        if (_videoType == VideoTypeRes.islive && liveVideoPlayer)
        {
            if (string.IsNullOrEmpty(imageLink))
            {
                SummitDomeNFTDataController.Instance.videoRenderObject = liveVideoPlayer;
                if (liveVideoPlayer)
                    liveVideoPlayer.SetActive(true);

                if (streamYoutubeVideo != null)
                    streamYoutubeVideo.StreamYtVideo(videoLink, true);
                SoundController.Instance.livePlayerSource = liveVideoPlayer.GetComponent<MediaPlayer>();
                SoundSettings.soundManagerSettings.setNewSliderValues();
            }
            else
            {
                SetImage();
            }
        }
        else if (_videoType == VideoTypeRes.prerecorded /*&& preRecordedPlayer*/)
        {
            if (string.IsNullOrEmpty(imageLink))
            {
                RenderTexture renderTexture = new RenderTexture(SummitDomeNFTDataController.Instance.renderTexture_16x9);
                SoundController.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
                SoundSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
                SoundSettings.soundManagerSettings.setNewSliderValues();
                SummitDomeNFTDataController.Instance.videoRenderObject = imgVideo16x9;
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
                //preRecordedPlayer.GetComponent<YoutubeSimplified>().player.showThumbnailBeforeVideoLoad = false;
                VideoPlayer tempVideoPlayer;
                if (applyVideoMesh)
                {
                    tempVideoPlayer = videoMesh;
                }
                else
                {
                    tempVideoPlayer = imgVideo16x9.GetComponent<VideoPlayer>();
                }

                if (streamYoutubeVideo != null)
                    streamYoutubeVideo.StreamYtVideo(videoLink, false);
                imgVideo16x9.GetComponent<VideoPlayer>().playOnAwake = true;
                imgVideo16x9.SetActive(true);
                if (imgVideoFrame16x9)
                {
                    EnableImageVideoFrame(imgVideoFrame16x9);
                }
            }
            else
            {
                SetImage();
            }
        }
        else if (_videoType == VideoTypeRes.aws)
        {
            if (string.IsNullOrEmpty(imageLink))
            {
                if (_imgVideoRatio == JjRatio.SixteenXNineWithDes || _imgVideoRatio == JjRatio.SixteenXNineWithoutDes)
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
                        RenderTexture renderTexture = new RenderTexture(SummitDomeNFTDataController.Instance.renderTexture_16x9);
                        renderTexture_temp = renderTexture;
                        if (!isForceAudioOn)
                        {
                            imgVideo16x9.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;

                        }
                        else
                        {

                            SoundController.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
                            SoundSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
                            SoundSettings.soundManagerSettings.setNewSliderValues();
                        }
                        imgVideo16x9.GetComponent<RawImage>().texture = renderTexture;
                        imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                        imgVideo16x9.GetComponent<VideoPlayer>().url = videoLink;
                        imgVideo16x9.GetComponent<VideoPlayer>().Play();
                    }
                }
                else if (_imgVideoRatio == JjRatio.NineXSixteenWithDes || _imgVideoRatio == JjRatio.NineXSixteenWithoutDes)
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
                        RenderTexture renderTexture = new RenderTexture(SummitDomeNFTDataController.Instance.renderTexture_9x16);
                        renderTexture_temp = renderTexture;
                        imgVideo9x16.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                        imgVideo9x16.GetComponent<RawImage>().texture = renderTexture;
                        imgVideo9x16.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                        imgVideo9x16.GetComponent<VideoPlayer>().url = videoLink;
                        imgVideo9x16.GetComponent<VideoPlayer>().Play();
                    }
                }
                else if (_imgVideoRatio == JjRatio.OneXOneWithDes || _imgVideoRatio == JjRatio.OneXOneWithoutDes)
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
                        RenderTexture renderTexture = new RenderTexture(SummitDomeNFTDataController.Instance.renderTexture_1x1);
                        renderTexture_temp = renderTexture;
                        imgVideo1x1.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                        imgVideo1x1.GetComponent<RawImage>().texture = renderTexture;
                        imgVideo1x1.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                        imgVideo1x1.GetComponent<VideoPlayer>().url = videoLink;
                        imgVideo1x1.GetComponent<VideoPlayer>().Play();
                    }
                }
                else if (_imgVideoRatio == JjRatio.FourXThreeWithDes || _imgVideoRatio == JjRatio.FourXThreeWithoutDes)
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
                        RenderTexture renderTexture = new RenderTexture(SummitDomeNFTDataController.Instance.renderTexture_4x3);
                        renderTexture_temp = renderTexture;
                        imgVideo4x3.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                        imgVideo4x3.GetComponent<RawImage>().texture = renderTexture;
                        imgVideo4x3.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                        imgVideo4x3.GetComponent<VideoPlayer>().url = videoLink;
                        imgVideo4x3.GetComponent<VideoPlayer>().Play();
                    }
                }
            }
            else
            {
                SetImage();
            }
        }

        if (isCreateFrame)
            CreateFrame();   //create frame

        if (SummitDomeNFTDataController.Instance && renderTexture_temp != null)
            SummitDomeNFTDataController.Instance.NFTLoadedVideos.Add(renderTexture_temp);
    }

    private void CreateFrame()
    {
        GameObject frame = JJFrameManager.instance.ref_JJObjectPooler.GetPooledObjectFrame();
        frame.transform.SetParent(this.gameObject.transform);
        frame.transform.position = transform.position;
        frame.SetActive(true);
        frame.transform.localPosition = new Vector3(JJFrameManager.instance.frameLocalPos.x, JJFrameManager.instance.frameLocalPos.y, JJFrameManager.instance.frameLocalPos.z);
        frame.transform.localEulerAngles = new Vector3(90, -180.0f, 0);
        frame.transform.localScale = new Vector3(JJFrameManager.instance.frameLocalScale.x, JJFrameManager.instance.frameLocalScale.y, JJFrameManager.instance.frameLocalScale.z);

        GameObject spotLightObj = JJFrameManager.instance.ref_JJObjectPooler.GetPooledObjectSpotLight();
        spotLightObj.transform.SetParent(this.gameObject.transform);
        spotLightObj.transform.position = transform.position;
        spotLightObj.SetActive(true);
        spotLightObj.transform.localScale = new Vector3(JJFrameManager.instance.spotLightScale.x, JJFrameManager.instance.spotLightScale.y, JJFrameManager.instance.spotLightScale.z);
        spotLightObj.transform.localPosition = JJFrameManager.instance.spotLightPrefabPos;
        spotLightObj.transform.localEulerAngles = new Vector3(-22.857f, 180f, 0f);
    }

}
