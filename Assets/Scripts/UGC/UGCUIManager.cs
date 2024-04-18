using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

public class UGCUIManager : MonoBehaviour
{
    public bool isPressed;
    public bool isPhoto;
    public bool isVideo;
    public string snapSavePath;
    public float VideoRecordTimer;
    //public float holdTime;
    public bool isRecording;
    //public float holdTimeForPhoto;

    public AvatarController UGCCharacter;
    public VideoPlayer videoPlayer;
    public RenderTexture characterRT;
    public Camera characterRenderCamera;
    public List<GameObject> screenUI = new List<GameObject>();
    public GameObject videoImageResultScreen;
    public GameObject savePopup;
    public GameObject recordScreen;
    public GameObject photoScreen;
    public GameObject videoPlayScreen;
    public GameObject loadingScreen;
    public Image recordButton;
    public Image photoButton;

    //public Renderer BG;
    //public Texture texture;

    public TextMeshProUGUI videoRecordingTimerText;
    public UGCRecordVideoBehaviour ugcRecordVideoBehaviour;

    //new changes
    public UGCDataManager ugcDataManager;
    [Header("Background Panel")]
    public GameObject bgScreenPanel;
    public Renderer bgMat;
    public Texture defaultTexture;
    public string bgDefaultTextureKey = "";

    public GameObject tagsPrefab;
    public Transform tagsPrefabParent;
    public Transform bgPrefabParent;
    public List<GameObject> tagsObjects;
    public List<GameObject> tagsbuttons;
    bool saveVideo = false;
    public GameObject loadingTexture;
    SavingCharacterDataClass _CharacterData;

    //public Color normalColor, highlightedColor;
    //public Color normalTextColor, highlightedTextColor;
    public GameObject ItemPrefab;


    void Start()
    {
        CharacterHandler.instance.ActivateAvatarByGender(SaveCharacterProperties.instance.SaveItemList.gender);
        UGCCharacter = CharacterHandler.instance.GetActiveAvatarData().avatar_parent.GetComponent<AvatarController>();

        ConstantsHolder.xanaConstants.returnedFromGamePlay = true;
        DisableLoadingPanel();
        isPhoto = false;
        isVideo = true;
        // BGMat = new Material(BG.material);
    }


    public void DisableLoadingPanel()
    {
        StartCoroutine(IEHandleLoadingPanel());
    }
    public IEnumerator IEHandleLoadingPanel()
    {
        loadingScreen.SetActive(true);
        while (!UGCCharacter.isClothLoaded)
        {
            yield return new WaitForSeconds(.5f);
        }
        if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        {
            _CharacterData = new SavingCharacterDataClass();
            _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));
            if (_CharacterData.isBgApply)
            {
                StartCoroutine(ugcDataManager.DownloadBgAddressableTexture(_CharacterData.bgKeyValue));
                bgDefaultTextureKey = _CharacterData.bgKeyValue;
            }
            else if (bgDefaultTextureKey != null && bgDefaultTextureKey != "")
            {
                StartCoroutine(ugcDataManager.DownloadBgAddressableTexture(bgDefaultTextureKey));
            }
            else
            {
                ApplyDefaultTexture();
            }
        }

        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
    }
    public IEnumerator IEVideoButtonDown()
    {
        while (isPressed && !isRecording)
        {
           // holdTime += Time.deltaTime;
           // if (holdTime > holdTimeForPhoto)
           // {
                StartRecording();
           // }
            yield return null;
        }
        yield return null;
    }

    public void ActiveUI(bool enable)
    {
        for (int i = 0; i < screenUI.Count; i++)
        {
            screenUI[i].SetActive(enable);
        }
    }
    public void OnVideoButtonDown()
    {
        isPressed = true;
        VideoButtonDownCoroutine = StartCoroutine(IEVideoButtonDown());
    }
    public void OnTapaRecordingButton()
    {
        isPressed = true;
        if (isVideo)
        {
            if (isPressed && !isRecording) 
            {
                VideoButtonDownCoroutine = StartCoroutine(IEVideoButtonDown());
            }
            else 
            {
                StopRecording();
               // holdTime = 0;
                isPressed = false;
            }
        }
        else if (isPhoto)
        {
            TakeAPhoto();
        }
    }
    public void OnTapVideoButton()
    {
        isPhoto = false;
        isVideo = true;
    }
    public void OnTapphotoButton()
    {
        isPhoto = true;
        isVideo = false;
    }
    Coroutine VideoButtonDownCoroutine;
    public void OnVideoButtonUp()
    {
        //StopCoroutine(VideoButtonDownCoroutine);
        //if (holdTime < holdTimeForPhoto)
        //{
        //    TakeAPhoto();
        //    holdTime = 0;
        //    isPressed = false;
        //}
        //else
        //{
        //    if (isPressed && isRecording)
        //    {
        //        StopRecording();
        //        holdTime = 0;
        //        isPressed = false;
        //    }
        //}
    }

    #region TakeSnap

    private Camera newCam;
    private Texture2D screenshot;
    private RenderTexture screenshotRT;
    public void TakeAPhoto()
    {
        //isPhoto = true;
        //isVideo = false;
        GameObject g = new GameObject();
        g.transform.parent = Camera.main.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.Euler(Vector3.zero);
        g.SetActive(false);
        newCam = g.AddComponent<Camera>();

        newCam.enabled = false;

        screenshotRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        StartCoroutine(IETakePhoto());
    }

    public IEnumerator IETakePhoto()
    {
        ActiveUI(false);
        yield return new WaitForSeconds(0.1f);
        newCam.aspect = Camera.main.aspect;
        newCam.fieldOfView = Camera.main.fieldOfView;
        newCam.orthographic = Camera.main.orthographic;
        newCam.orthographicSize = Camera.main.orthographicSize;
        newCam.backgroundColor = Camera.main.backgroundColor;
        newCam.cullingMask = 1 << 8;
        newCam.cullingMask = ~newCam.cullingMask;
        newCam.depth = 1;

        yield return new WaitForEndOfFrame();

        newCam.targetTexture = screenshotRT;
        newCam.Render();
        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();
        newCam.targetTexture = null;
        Sprite captureSp = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.FullRect);
        photoScreen.GetComponent<Image>().sprite = captureSp;
        byte[] bytes = screenshot.EncodeToPNG();

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "UGCSnap")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "UGCSnap"));
        }

        snapSavePath = Path.Combine(Application.persistentDataPath + "/UGCSnap", "Image" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");
        File.WriteAllBytes(snapSavePath, bytes);
        Destroy(newCam);
        recordScreen.SetActive(false);
        photoScreen.SetActive(true);
        videoPlayScreen.SetActive(false);
        videoImageResultScreen.SetActive(true);
        ActiveUI(true);
    }
    #endregion


    #region VideoRecording

    public void StartRecording()
    {
        recordButton.gameObject.SetActive(true);
        photoButton.gameObject.SetActive(false);
        videoRecordingTimerText.gameObject.SetActive(true);
        isRecording = true;
        recordtimerCoroutine = StartCoroutine(IEStartVideoTimer());
        ugcRecordVideoBehaviour.StartRecording();
    }

    public void StopRecording()
    {
        isRecording = false;
       // isPhoto = false;
       // isVideo = true;
        recordButton.gameObject.SetActive(false);
        photoButton.gameObject.SetActive(true);
        ugcRecordVideoBehaviour.StopRecording();
        StartCoroutine(PlayRecordedVideo());
        StopCoroutine(recordtimerCoroutine);
    }

    Coroutine recordtimerCoroutine;
    public IEnumerator IEStartVideoTimer()
    {
        VideoRecordTimer = 0;
        while (isRecording)
        {
            VideoRecordTimer += Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(VideoRecordTimer);

            string answer = string.Format("{0:D2}.{1:D2}.{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
            videoRecordingTimerText.text = answer.ToString();
            yield return null;
        }
        recordtimerCoroutine = null;
    }
    #endregion

    public void BackToRecordScreen()
    {
        videoRecordingTimerText.gameObject.SetActive(false);
        recordScreen.SetActive(true);
        videoImageResultScreen.SetActive(false);
        if (!saveVideo && isVideo)
        {
            File.Delete(ugcRecordVideoBehaviour.videoRecordingPath);
            ugcRecordVideoBehaviour.videoRecordingPath = "";
            saveVideo = false;
        }
        else if (!saveVideo && isPhoto)
        {
            File.Delete(snapSavePath);
            snapSavePath = "";
            saveVideo = false;
        }
        else
        {
            snapSavePath = "";
            ugcRecordVideoBehaviour.videoRecordingPath = "";
            saveVideo = false;
        }

    }

    public void BackToHomeScreen()
    {
        if (bgScreenPanel.activeInHierarchy)
        {
            bgScreenPanel.SetActive(false);
            screenUI[1].SetActive(true);
            if (bgDefaultTextureKey != null && bgDefaultTextureKey != "")
            {
                StartCoroutine(ugcDataManager.DownloadBgAddressableTexture(bgDefaultTextureKey));
            }
            else
            {
                ApplyDefaultTexture();
            }
        }
        else
        {
            SceneManager.LoadScene("Home");
        }

    }
    public void CancelVideoSreen()
    {
        if (!saveVideo && isVideo)
        {
            File.Delete(ugcRecordVideoBehaviour.videoRecordingPath);
            ugcRecordVideoBehaviour.videoRecordingPath = "";
            saveVideo = false;
        }
        else if (!saveVideo && isPhoto)
        {
            File.Delete(snapSavePath);
            snapSavePath = "";
            saveVideo = false;
        }
        else
        {
            snapSavePath = "";
            ugcRecordVideoBehaviour.videoRecordingPath = "";
            saveVideo = false;
        }
    }
    //public void OnClickTags(GameObject _gameObject, string _category)
    //{
    //    for (int i = 0; i < tagsPrefabParent.childCount; i++)
    //    {
    //        tagsbuttons[i].GetComponent<Image>().color = normalColor;
    //        tagsbuttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = normalTextColor;
    //    }
    //    _gameObject.GetComponent<Image>().color = highlightedColor;
    //    _gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = highlightedTextColor;
    //    for (int i = 0; i < tagsObjects.Count; i++)
    //    {
    //        if (tagsObjects[i].GetComponent<BgTagsView>().tagName == _category)
    //        {
    //            tagsObjects[i].SetActive(true);
    //        }
    //        else
    //        {
    //            tagsObjects[i].SetActive(false);
    //        }
    //    }
    //}
    public IEnumerator PlayRecordedVideo()
    {
        loadingScreen.SetActive(true);
        while (string.IsNullOrEmpty(ugcRecordVideoBehaviour.videoRecordingPath))
        {
            yield return new WaitForSeconds(.1f);
        }
        recordScreen.SetActive(false);
        photoScreen.SetActive(false);
        videoImageResultScreen.SetActive(true);
        videoPlayScreen.SetActive(true);
        var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "" : "";
        videoPlayer.url = $"{prefix}{ugcRecordVideoBehaviour.videoRecordingPath}";
        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(.1f);
        }
        loadingScreen.SetActive(false);
        videoPlayer.Play();
    }

    public void OnTapOnARButton()
    {

    }

    public void OnTapBackGroundButton()
    {
        loadingTexture.SetActive(true);
        ugcDataManager.GetAllBackGroundCategory();
        bgScreenPanel.SetActive(true);
        screenUI[1].SetActive(false);

    }
    Material BGMat;
    public void ChangeBG()
    {
        //BGMat.mainTexture = texture;
        //BG.material = BGMat;
    }
    public void OnTapSaveButton()
    {
        if (isPhoto)
        {
            NativeGallery.SaveImageToGallery(screenshot, "Xana", "Image" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");
        }
        if (isVideo)
        {
            FileInfo file = new FileInfo(ugcRecordVideoBehaviour.videoRecordingPath);
            NativeGallery.SaveVideoToGallery(ugcRecordVideoBehaviour.videoRecordingPath, "Xana", file.Name.Replace(".mp4", "") + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }
        saveVideo = true;
        savePopup.SetActive(true);
    }

    public void OnSavePopUpOkButton()
    {
        savePopup.SetActive(false);
    }
    public void OnTapShareButton()
    {
        if (isPhoto)
        {
            NativeShare SharePost = new NativeShare();
            SharePost.AddFile(snapSavePath).Share();
        }
        if (isVideo)
        {
            NativeShare shareVideo = new NativeShare();
            shareVideo.AddFile(ugcRecordVideoBehaviour.videoRecordingPath).Share();

        }
    }
    public void OnClickSaveBackgroundButton()
    {
        bgScreenPanel.SetActive(false);
        screenUI[1].SetActive(true);
        bgDefaultTextureKey = _CharacterData.bgKeyValue;
    }
    public void ApplyBgTexture(Texture _texture, string _key)
    {
        //bgMat.mainTexture = texture;
        bgMat.material.mainTexture = _texture;
        _CharacterData.isBgApply = true;
        _CharacterData.bgKeyValue = _key;
        string bodyJson = JsonUtility.ToJson(_CharacterData);
        File.WriteAllText(GameManager.Instance.GetStringFolderPath(), bodyJson);
        ServerSideUserDataHandler.Instance.CreateUserOccupiedAsset(() =>
        {
        });
    }
    public void ApplyDefaultTexture()
    {
        //bgMat.mainTexture = texture;
        bgMat.material.mainTexture = defaultTexture;
    }
    public void OnClickSelectBackgroundButton(GameObject _gameObject, string key)
    {
        loadingTexture.SetActive(true);
        for (int i=0;i<tagsObjects.Count;i++) 
        {
            tagsObjects[i].transform.GetChild(1).gameObject.SetActive(false);
        }
        _gameObject.transform.GetChild(1).gameObject.SetActive(true);
        key = Regex.Replace(key, @"\s", "");
        key = key.ToLower();
        key = "bg_" + key;
        StartCoroutine(ugcDataManager.DownloadBgAddressableTexture(key));
    }

}
