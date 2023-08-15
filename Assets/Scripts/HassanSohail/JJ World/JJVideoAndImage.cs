using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;

public class JJVideoAndImage : MonoBehaviour
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

    public VideoTypeRes _videoType;
    public JjRatio _imgVideoRatio;
    [SerializeField] bool ApplyImageOnTexture; // If image is not on Sqaure
    [SerializeField] MeshRenderer imageMesh;
    [SerializeField] Material imageMat;

     [SerializeField] bool applyVideoMesh; // If play video on mesh 
    [SerializeField] VideoPlayer videoMesh;
    // Start is called before the first frame update

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
        if (preRecordedPlayer)
            preRecordedPlayer.SetActive(false);


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
        if (preRecordedPlayer)
            preRecordedPlayer.SetActive(false);

        if (_videoType==VideoTypeRes.islive && liveVideoPlayer)
        {
            if (liveVideoPlayer)
            liveVideoPlayer.SetActive(true);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>()._livestreamUrl = videoLink;
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().GetLivestreamUrl(videoLink);
            liveVideoPlayer.GetComponent<YoutubePlayerLivestream>().mPlayer.Play();
        }
        else if(_videoType == VideoTypeRes.prerecorded && preRecordedPlayer)
        {
            RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_16x9);
            renderTexture_temp = renderTexture;

                imgVideo16x9.GetComponent<RawImage>().texture= renderTexture;
                imgVideo16x9.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                preRecordedPlayer.GetComponent<YoutubeSimplified>().player.showThumbnailBeforeVideoLoad = false;
                VideoPlayer tempVideoPlayer;
                if (applyVideoMesh)
                {
                    tempVideoPlayer = videoMesh;
                }
                else
                {
                    tempVideoPlayer =imgVideo16x9.GetComponent<VideoPlayer>();
                }

                preRecordedPlayer.GetComponent<YoutubeSimplified>().videoPlayer = tempVideoPlayer;
                preRecordedPlayer.GetComponent<YoutubeSimplified>().player.videoPlayer = tempVideoPlayer;
                preRecordedPlayer.GetComponent<YoutubeSimplified>().player.audioPlayer = tempVideoPlayer;
                preRecordedPlayer.SetActive(true);
                preRecordedPlayer.GetComponent<YoutubeSimplified>().url = videoLink;
                preRecordedPlayer.GetComponent<YoutubeSimplified>().Play();
                imgVideo16x9.GetComponent<VideoPlayer>().playOnAwake = true;
                imgVideo16x9.SetActive(true);
            
        }
        else if(_videoType == VideoTypeRes.aws)
        {
            if (_imgVideoRatio == JjRatio.SixteenXNineWithDes || _imgVideoRatio == JjRatio.SixteenXNineWithoutDes)
            {
                if (imgVideo16x9)
                {
                    imgVideo16x9.SetActive(true);
                    imgVideo16x9.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo16x9.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_16x9);
                    renderTexture_temp = renderTexture;
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
                    imgVideo9x16.SetActive(true);
                    imgVideo9x16.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo9x16.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_9x16);
                    renderTexture_temp = renderTexture;
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
                    imgVideo1x1.SetActive(true);
                    imgVideo1x1.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo1x1.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_1x1);
                    renderTexture_temp = renderTexture;
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
                    imgVideo4x3.SetActive(true);
                    imgVideo4x3.GetComponent<VideoPlayer>().enabled = true;
                    //imgVideo4x3.GetComponent<RawImage>().texture = imgVideo16x9.GetComponent<VideoPlayer>().targetTexture;
                    RenderTexture renderTexture = new RenderTexture(JjInfoManager.Instance.renderTexture_4x3);
                    renderTexture_temp = renderTexture;
                    imgVideo4x3.GetComponent<RawImage>().texture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().targetTexture = renderTexture;
                    imgVideo4x3.GetComponent<VideoPlayer>().url = videoLink;
                    imgVideo4x3.GetComponent<VideoPlayer>().Play();
                }
            }
        }
        if (JjInfoManager.Instance && renderTexture_temp != null)
            JjInfoManager.Instance.NFTLoadedVideos.Add(renderTexture_temp);
    }

    public void OpenWorldInfo()
    {
        if (SelfieController.Instance.m_IsSelfieFeatureActive) return;

        if (JjInfoManager.Instance != null && _videoType!=VideoTypeRes.islive)
        {
            if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
            {
                JjInfoManager.Instance.SetInfo(_imgVideoRatio, JjInfoManager.Instance.worldInfos[id].Title[0], JjInfoManager.Instance.worldInfos[id].Aurthor[0], JjInfoManager.Instance.worldInfos[id].Des[0], _texture, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink, JjInfoManager.Instance.worldInfos[id].videoType, id);
            }
            else if (CustomLocalization.forceJapanese || GameManager.currentLanguage.Equals("ja"))
            {
                if (!JjInfoManager.Instance.worldInfos[id].Title[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Aurthor[1].IsNullOrEmpty() && !JjInfoManager.Instance.worldInfos[id].Des[1].IsNullOrEmpty())
                {
                    JjInfoManager.Instance.SetInfo(_imgVideoRatio, JjInfoManager.Instance.worldInfos[id].Title[1], JjInfoManager.Instance.worldInfos[id].Aurthor[1], JjInfoManager.Instance.worldInfos[id].Des[1], _texture, JjInfoManager.Instance.worldInfos[id].Type, JjInfoManager.Instance.worldInfos[id].VideoLink, JjInfoManager.Instance.worldInfos[id].videoType, id);
                }
            }
        }
    }
}
