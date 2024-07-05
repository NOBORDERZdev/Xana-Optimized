using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

public class JJVideoAndImage : MonoBehaviour
{
    public int id;
   
    private Texture2D _texture;

    public GameObject imgVideo16x9;
    public GameObject imgVideo9x16;
    public GameObject imgVideo1x1;
    public GameObject imgVideo4x3;

    public GameObject liveVideoPlayer;
    //public GameObject preRecordedPlayer;

    public string videoLink;
    public string imageLink;
    [SerializeField] private bool isForceAudioOn = false;

    public VideoTypeRes _videoType;
    public JjRatio _imgVideoRatio;
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
    private StreamYoutubeVideo streamYoutubeVideo;
    private void Start()
    {
        imgVideo16x9.AddComponent<Button>();
        //JjWorldInfo jjWorldInfo=imgVideo16x9.GetComponent<JjWorldInfo>();
        imgVideo16x9.GetComponent<Button>().onClick.AddListener(()=>OpenWorldInfo());

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

    }
    private void OnDisable()
    {
        JJEraseDownloadedData();
        Debug.Log("JJ Data Cleared");
        Resources.UnloadUnusedAssets();
    }

    public void InitData(string imageurl, string videourl, JjRatio imgvideores, DataType dataType,VideoTypeRes videoType)
    {
        imageLink = imageurl;
        videoLink = videourl;
        _imgVideoRatio = imgvideores;
        _videoType = videoType;
        if (dataType == DataType.Image)
            SetImage();
        else if (dataType == DataType.Video)
            SetVideo();

        //if(isCreateFrame)
        //    CreateFrame();   //create frame
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
        //if (preRecordedPlayer)
        //    preRecordedPlayer.SetActive(false);


        StartCoroutine(GetSprite(imageLink, (response) =>
        {
            if (JjInfoManager.Instance && response != null)
                JjInfoManager.Instance.NFTLoadedSprites.Add(response);

            if (ApplyImageOnTexture && imageMesh!= null)
            {
                imageMesh.material= imageMat;
                imageMesh.material.mainTexture=response;
            }
            else if (_imgVideoRatio == JjRatio.SixteenXNineWithDes || _imgVideoRatio == JjRatio.SixteenXNineWithoutDes)
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
                    if(imgVideo16x9.transform.childCount>0)
                    {
                        foreach(Transform g in imgVideo16x9.transform)
                        {
                            g.gameObject.GetComponent<RawImage>().texture = response;
                            g.gameObject.SetActive(true);
                        }
                    }
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
            else if (_imgVideoRatio == JjRatio.OneXOneWithDes || _imgVideoRatio == JjRatio.OneXOneWithoutDes)
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
            else if (_imgVideoRatio == JjRatio.FourXThreeWithDes || _imgVideoRatio == JjRatio.FourXThreeWithoutDes)
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
                //Debug.Log("ERror in loading sprite" + www.error);
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
        //if (preRecordedPlayer)
        //    preRecordedPlayer.SetActive(false);

        if (_videoType==VideoTypeRes.islive && liveVideoPlayer)
        {
            JjInfoManager.Instance.videoRenderObject = liveVideoPlayer;
            if (liveVideoPlayer)
            liveVideoPlayer.SetActive(true);
            //liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
            //liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
            //liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
            if(streamYoutubeVideo!=null)
                streamYoutubeVideo.StreamYtVideo(videoLink, true);
            SoundController.Instance.livePlayerSource = liveVideoPlayer.GetComponent<MediaPlayer>();
            SoundSettings.soundManagerSettings.setNewSliderValues();
        }
        else if(_videoType == VideoTypeRes.prerecorded /*&& preRecordedPlayer*/)
        {
            RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_16x9);
            SoundController.Instance.videoPlayerSource = imgVideo16x9.GetComponent<AudioSource>();
            SoundSettings.soundManagerSettings.videoSource = imgVideo16x9.GetComponent<AudioSource>();
            SoundSettings.soundManagerSettings.setNewSliderValues();
            JjInfoManager.Instance.videoRenderObject = imgVideo16x9;
            renderTexture_temp = renderTexture;
                imgVideo16x9.GetComponent<RawImage>().texture= renderTexture;
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
                    tempVideoPlayer =imgVideo16x9.GetComponent<VideoPlayer>();
                }

            //preRecordedPlayer.SetActive(true);
            //preRecordedPlayer.GetComponent<YoutubeSimplified>().videoPlayer = tempVideoPlayer;
            //preRecordedPlayer.GetComponent<YoutubeSimplified>().player.videoPlayer = tempVideoPlayer;
            //preRecordedPlayer.GetComponent<YoutubeSimplified>().player.audioPlayer = tempVideoPlayer;
            //preRecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
            //preRecordedPlayer.GetComponent<YoutubeSimplified>().Play();
            if(streamYoutubeVideo!=null)
                streamYoutubeVideo.StreamYtVideo(videoLink, false);
            imgVideo16x9.GetComponent<VideoPlayer>().playOnAwake = true;
            imgVideo16x9.SetActive(true);
            if (imgVideoFrame16x9)
            {
                EnableImageVideoFrame(imgVideoFrame16x9);
            }
        }
        else if(_videoType == VideoTypeRes.aws)
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
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_16x9);
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
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_9x16);
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
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_1x1);
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
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_4x3);
                    renderTexture_temp = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
                    imgVideo4x3.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo4x3.GetComponent<VideoPlayer>().Play();
                }
            }
        }

        if (isCreateFrame)
            CreateFrame();   //create frame

        if (JjInfoManager.Instance && renderTexture_temp != null)
            JjInfoManager.Instance.NFTLoadedVideos.Add(renderTexture_temp);
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
        spotLightObj.transform.localPosition = JJFrameManager.instance.spotLightPrefabPos ;
        spotLightObj.transform.localEulerAngles = new Vector3(-22.857f, 180f, 0f);
    }

    public void OpenWorldInfo()
    {
        if (PlayerSelfieController.Instance.m_IsSelfieFeatureActive) return;
        if (PlayerController.isJoystickDragging == true)
            return;
        //JjInfoManager.Instance.firebaseEventName = firebaseEventName;
        if (JjInfoManager.Instance != null && _videoType!=VideoTypeRes.islive)
        {
            if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
            {
                JjInfoManager.Instance.SetInfo(_imgVideoRatio, JjInfoManager.Instance.worldInfos[id].Title[0], JjInfoManager.Instance.worldInfos[id].Aurthor[0], JjInfoManager.Instance.worldInfos[id].Des[0], JjInfoManager.Instance.worldInfos[id].url, _texture, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink, JjInfoManager.Instance.worldInfos[id].videoType, id, museumType, roomNumber);       
            }
            else if (LocalizationManager.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                JjInfoManager.Instance.SetInfo(_imgVideoRatio, JjInfoManager.Instance.worldInfos[id].Title[1], JjInfoManager.Instance.worldInfos[id].Aurthor[1], JjInfoManager.Instance.worldInfos[id].Des[1], JjInfoManager.Instance.worldInfos[id].url, _texture, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink, JjInfoManager.Instance.worldInfos[id].videoType, id, museumType, roomNumber);

            }
        }
    }
    private void JJEraseDownloadedData() {
        if (imgVideo16x9.GetComponent<RawImage>().texture != null)
        {
            DestroyImmediate(imgVideo16x9.GetComponent<RawImage>().texture, true);
            imgVideo16x9.GetComponent<RawImage>().texture = null;
            imgVideo16x9.SetActive(false);
        }
        else if (imgVideo9x16.GetComponent<RawImage>().texture != null)
        {
            DestroyImmediate(imgVideo9x16.GetComponent<RawImage>().texture, true);
            imgVideo9x16.GetComponent<RawImage>().texture = null;
            imgVideo9x16.SetActive(false);
        }
        else if (imgVideo1x1.GetComponent<RawImage>().texture != null)
        {

            DestroyImmediate(imgVideo1x1.GetComponent<RawImage>().texture, true);
            imgVideo1x1.GetComponent<RawImage>().texture = null;
            imgVideo1x1.SetActive(false);
        }
        else if (imgVideo4x3.GetComponent<RawImage>().texture != null)
        {
            DestroyImmediate(imgVideo4x3.GetComponent<RawImage>().texture, true);
            imgVideo4x3.GetComponent<RawImage>().texture = null;
            imgVideo4x3.SetActive(false);
        }
              
        if (this.GetComponent<StreamYoutubeVideo>() != null)
        {
            streamYoutubeVideo = this.GetComponent<StreamYoutubeVideo>();
        }


    }
}
