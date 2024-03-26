using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
//using UnityEngine.XR.ARFoundation;

public class VideoRoomHandler : MonoBehaviour
{
	public static VideoRoomHandler Instance;

    //public ARPoseDriver _aRPoseDriver;
    public ARAvatarController _avatarScript; 

    public Image BackgroundImage;
	
	//public List<PostProcessProfile> filterVolumeProfile = new List<PostProcessProfile>();
	//public PostProcessVolume mainVolume;
    public Volume filterMainVolume; 
	public GameObject videoPlayer;
	public GameObject videoPlayScreen;
    public GameObject videoPlayerUIScreen;
	public GameObject VideoRawImage;
    public GameObject EmojiSelectionScreen;

    [Header("Image Selection Screen Reference")]
    public GameObject imageSelectionScreen;
    public GameObject imageSelectionUIScreen;
    public Image GalleryImage;

    public List<GameObject> imageSelectionUIObjList = new List<GameObject>();
    public List<GameObject> videoRecordingUIObjList = new List<GameObject>();

    public AudioSource audioSource;

    public  bool isRecoring = false;
    double videoTime;

    public int lastAvatarListCount;

    public bool IsVideoScreenImageScreenAvtive = false;

    private void Awake()
    {
        if (Instance == null)
        {
			Instance = this;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            _avatarScript = ARFaceModuleHandler.Instance.mainAvatar.GetComponent<ARAvatarController>();
        }
        BackgroundImage.GetComponent<AspectRatioFitter>().aspectRatio = 0.57f;
    }

    private void Update()
    {
        if (isRecoring)
        {
            videoTime -= Time.deltaTime;
            if (videoTime <= 0)
            {
                isRecoring = false;
                OnSaveVideoTOGallary(videoPath);
            }
        }
    }

    public void OnStartVideoPlay(string url, bool isFromGallery, bool isPickVideo = false)
    {
       Debug.Log("OnStartVideoPlay:" + url);
        // videoPlayer.GetComponent<VideoPlayer>().Prepare();

        //videoPlayer.GetComponent<VideoPlayer>().url = url;
        isPickVideoFromGellary = isFromGallery;
        VideoRawImage.SetActive(false);
        StartCoroutine(playVideo(url, isPickVideo));
        //videoPlayScreen.SetActive(true);
    }
    IEnumerator playVideo(string url, bool isPickVideo)
    {
        //Add VideoPlayer to the GameObject
        VideoPlayer currentVideoPlayer = videoPlayer.GetComponent<VideoPlayer>();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        currentVideoPlayer.targetTexture = renderTexture;

        //Assign the Texture from Video to RawImage to be displayed
        VideoRawImage.GetComponent<RawImage>().texture = renderTexture;

        //Disable Play on Awake for both Video and Audio
        currentVideoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        //We want to play from video clip not from url
        currentVideoPlayer.source = VideoSource.VideoClip;

        //Set Audio Output to AudioSource
        currentVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        currentVideoPlayer.EnableAudioTrack(0, true);
        currentVideoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        currentVideoPlayer.url = url;
        currentVideoPlayer.Prepare();
        //Wait until video is prepared

        double videoLength = 0;
#if UNITY_EDITOR
        while (!currentVideoPlayer.isPrepared)
        {
            Debug.Log("Preparing Video");
            yield return null;
        }
        videoLength = currentVideoPlayer.length;
        Debug.Log("Done Preparing Video" + currentVideoPlayer.length);
#else
        videoLength = NativeGallery.VideoLength(url)/1000;
#endif
        if (videoLength > 15 && isPickVideo)
        {
            ARFaceModuleHandler.Instance.ShowLoader(false);
            SNSWarningsHandler.Instance.ShowWarningMessage("Please upload video upto 15 seconds");
        }
        else
        {
            if (isPickVideo)//if pick video button comes then disable this ui.......
            {
                ARFaceModuleHandler.Instance.ShowLoader(false);
                if (ARFaceModuleHandler.Instance.mainAvatar != null)
                {
                    ARFaceModuleHandler.Instance.mainAvatar.gameObject.SetActive(false);
                }
                ARFaceModuleHandler.Instance.DisableBottomMainPanel(false);
            }

            videoPath = url;//pick video path store.......        

            filterMainVolume.gameObject.SetActive(false);
            videoPlayScreen.SetActive(true);
            videoPlayerUIScreen.SetActive(true);
            GetLastAvatarListCount();

            //Play Video
            currentVideoPlayer.Play();
            yield return new WaitForSeconds(0.5f);
            VideoRawImage.SetActive(true);
            //Play Sound
            audioSource.Play();

            Debug.Log("Playing Video");
            while (currentVideoPlayer.isPlaying)
            {
                //Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.GetComponent<VideoPlayer>().time));
                yield return null;
            }

            Debug.Log("Done Playing Video");
        }
    }

    public bool isPickVideoFromGellary = false;
    public void CloseVideoScreenBtnClick()
    {
        filterMainVolume.gameObject.SetActive(true);
        videoPlayer.GetComponent<VideoPlayer>().Stop();
        videoPlayScreen.SetActive(false);
        videoPlayerUIScreen.SetActive(false);

        if (File.Exists(videoPath) && !isPickVideoFromGellary)
        {
            File.Delete(videoPath);
        }
        ResetLastAvatarListCount();
        isPickVideoFromGellary = false;
    }

    public void CloseImageScreenBtnClick()
    {
        imageSelectionScreen.SetActive(false);
        imageSelectionUIScreen.SetActive(false);

        ResetLastAvatarListCount();
    }

    public void ImageSelectionAllUIDisable(bool isDisable)
    {
        for (int i = 0; i < imageSelectionUIObjList.Count; i++)
        {
            imageSelectionUIObjList[i].SetActive(isDisable);
        }
    }

    public void OnSaveImageSelectionBtnClick(int id)
    {
        ARFaceModuleHandler.Instance.OnCaptureButtonClick(id);
    }

    public void OnClickSaveVideoButton()
    {
        VideoRecordingController.instance.StartRecording();
        videoTime = videoPlayer.GetComponent<VideoPlayer>().length;
       Debug.Log("videoTime" + videoTime); 
        isRecoring = true;        
    }

    public string videoPath;
    public void OnSaveVideoTOGallary(string path)
    {
        path = videoPath;
        if (!isRecoring)
        {
            OnStartVideoPlay(path, true);
            NativeGallery.SaveVideoToGallery(path, "XanaVideo", "Video");
            VideoRecordingAllUIDisable(true);
        }
    }

    public void OnClickSelectAvtarBtnOnVideoEditScreen()
    {
        ARFaceModuleHandler.Instance.OnCharacterSelectionBtnClick();
    }

    public void OnEmojiSelectionBtnClick()
    {
        EmojiSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        EmojiSelectionScreen.SetActive(true);
        EmojiSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.2f).SetEase(Ease.Linear);
    }

    public void OnScreenCloseBtnClick()
    {
        if (EmojiSelectionScreen.activeSelf)
        {
            EmojiSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.2f).SetEase(Ease.Linear); 
            StartCoroutine(CloseOpenScreen(EmojiSelectionScreen));
        }       
    }

    public void VideoRecordingAllUIDisable(bool isDisable)
    {
        for (int i = 0; i < videoRecordingUIObjList.Count; i++)
        {
            //videoRecordingUIObjList[i].SetActive(isDisable);//temp cmnt
        }
    }
    IEnumerator CloseOpenScreen(GameObject openScreen)
    {
        yield return new WaitForSeconds(0.4f);
        openScreen.SetActive(false);
    }

    public void GetLastAvatarListCount()
    {
        IsVideoScreenImageScreenAvtive = true;

        //ARFaceModuleHandler.Instance.DisableBottomMainPanel(false);
        if (ARFaceModuleHandler.Instance.mainAvatar != null)
        {
           Debug.Log("GetLastAvatarListCount..... main avatar false");
            ARFaceModuleHandler.Instance.mainAvatar.SetActive(false);
        }
        if (ARFaceModuleHandler.Instance.addAvtarItem.Count != 0)
        {
            for (int i = 0; i < ARFaceModuleHandler.Instance.addAvtarItem.Count; i++)
            {
                ARFaceModuleHandler.Instance.addAvtarItem[i].gameObject.SetActive(false);
            }
        }
        lastAvatarListCount = ARFaceModuleHandler.Instance.addAvtarItem.Count;

        if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
        {
            //_aRPoseDriver.enabled = false;
        }
        else if (SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            //ARFacePoseTrackingManager.Instance.isVideoOpen =true;
        }
    }

    public void ResetLastAvatarListCount()
    {
        IsVideoScreenImageScreenAvtive = false;

        ARFaceModuleHandler.Instance.DisableBottomMainPanel(true);
               
        if (ARFaceModuleHandler.Instance.addAvtarItem.Count != 0)
        {
            for (int i = 0; i < ARFaceModuleHandler.Instance.addAvtarItem.Count; i++)
            {
               Debug.Log("Condition:" + i + " :lastAvatarListCount:" + lastAvatarListCount);
                if (i < lastAvatarListCount)
                {
                    ARFaceModuleHandler.Instance.addAvtarItem[i].gameObject.SetActive(true);
                }
                else
                {
                   Debug.Log("DeletePlayer:" + i);
                    GameObject crntObj = ARFaceModuleHandler.Instance.addAvtarItem[i];
                    ARFaceModuleHandler.Instance.addAvtarItem.Remove(crntObj);
                    Destroy(crntObj);
                    i--;
                }
            }
        }

        if (ARFaceModuleHandler.Instance.videoEditCanvas.transform.childCount != 0)
        {
            for (int i = 0; i < ARFaceModuleHandler.Instance.videoEditCanvas.transform.childCount; i++)
            {
                Destroy(ARFaceModuleHandler.Instance.videoEditCanvas.transform.GetChild(i).gameObject);
            }
        }

        if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
        {
            //_aRPoseDriver.enabled = true;
        }
        else if(SceneManager.GetActiveScene().name == "ARModuleRealityScene")
        {
            if (_avatarScript != null)
            {
                _avatarScript.SetDefaultAvatarHipsPos();
            }
            ARFaceModuleHandler.Instance.SetDefaultAvatarPosition();
            //ARFacePoseTrackingManager.Instance.SetDefaultMoveTargetObjPos();
        }
       Debug.Log("ResetLastAvatarListCount.......");
        if (ARFaceModuleHandler.Instance.mainAvatar != null && SceneManager.GetActiveScene().name != "ARModulePlanDetectionScene")
        {
            ARFaceModuleHandler.Instance.mainAvatar.gameObject.SetActive(true);
        }
    }
} 