using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using RenderHeads.Media.AVProVideo;
using AdvancedInputFieldPlugin;
using System;
//using UnityEngine.XR.ARFoundation;

public class ARFaceModuleManager : MonoBehaviour
{
    public static ARFaceModuleManager Instance;

    public string xanaRoomSavePath;

    //public ARFaceTrackManager aRFaceTrackManager;
    public GameObject testDefaultAvatar;
    public bool isTest;

    [Space]
    [Header("Main Avatar reference")]
    public GameObject mainAvatar;
    public Vector3 MainAvatarDefaultPos;
    public float mainAvatarScale;

    [Header("FadeInOut color")]
    public Color loadToColor = Color.black;

    [Header("Ui reference")]
    public GameObject uiCan;
    public GameObject tempDontClickObj;

    [Header("Capture Screen")]
    public GameObject captureViewScreen;
    public Image displayCatureImage;

    [Header("Screens")]
    public GameObject backGroundSelectionScreen;
    public Transform backGroundScreenContainer;
    public Transform backgroundScreenColorContainer;

    [Space]
    public GameObject filterSelectionScreen;
    public GameObject characterSelectionScreen;
    public Transform characterSelectionContainer;
    public GameObject albumSelectionScreen;
    public GameObject bottomUiPanel;
    public GameObject bottomMainTypePanelScrollView;
    public GameObject topMainPanel;

    [Header("Notification Screen Reference")]
    public GameObject notificationScreen;
    public TextMeshProUGUI notificationText;

    [Header("Other")]
    public Button[] bottomMainPanelButtons;
    public SelectionItemScript selectionItemScript;
    //public bool isImageCapture = false;
    //float imageCaptureTime;
    public Transform addItemParent;
    public List<GameObject> addAvtarItem = new List<GameObject>();

    public string sceneName = "";

    public Vector3 allCharacterItemDefaultScale = Vector3.one;
    public Vector3 allCharacterItemDefaultPos = new Vector3(0f,0f,2f);
    public GameObject[] allCharacterItemList;
    public GameObject EmojiItem;
    public List<Sprite> allEmojiSprite = new List<Sprite>();

    public GameObject videoEditCanvas;

    [Header("Delete item reference")]
    public GameObject currentSelectedItemObj;
    public GameObject deleteItemScreen;
    public DeleteItemWithDragScript deleteItemWithDragScript;

    [Header("Avatar Selection Outline Default Setup")]
    public Color outlineColor = Color.red;

    [SerializeField, Range(0f, 10f)]
    public float outlineWidth = 3;
    public Outline1.Mode outlineMode = Outline1.Mode.OutlineVisible;

    [Header("Main Photo Video UI reference")]
    public bool r_IsCaptureTypeImage;
    public GameObject photoVideoPanel;
    public GameObject captureButtonObj;
    public List<GameObject> photoVideoButtonList = new List<GameObject>();
    public VideoRecordingButton videoCaptureButton;
    public bool r_IsCapturingVideoBack = false;
 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (isTest)
        {
            testDefaultAvatar.SetActive(true);
            mainAvatar = testDefaultAvatar;
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "ARModulePlanDetectionScene")
            {
                mainAvatar.SetActive(true);
            }
            if (ConstantsHolder.xanaConstants.r_MainSceneAvatar != null)
            {
                GameObject mainSceneAvatar = Instantiate(ConstantsHolder.xanaConstants.r_MainSceneAvatar, mainAvatar.transform);
               // mainSceneAvatar.GetComponent<AvatarControllerHome>().UpdateState(true);
                mainSceneAvatar.GetComponent<AvatarController>().SetAvatarForAR();
                mainSceneAvatar.transform.localScale = Vector3.one;
                mainSceneAvatar.transform.localPosition = new Vector3(0, 0, 0);
                mainSceneAvatar.transform.rotation = Quaternion.Euler(0, 180, 0);
                mainSceneAvatar.transform.SetAsFirstSibling();
                Debug.Log("mainSceneAvatar.transform name: " + mainSceneAvatar.transform.name);
                /*GameObject planObj = mainSceneAvatar.transform.Find("Plane").gameObject;

                if (planObj != null)
                {
                    planObj.GetComponent<MeshCollider>().enabled = false;
                }*/
                mainSceneAvatar.SetActive(true);
            }
        }

        xanaRoomSavePath = Path.Combine(Application.persistentDataPath, "XanaRoom");
        if (!Directory.Exists(xanaRoomSavePath))
        {
            Directory.CreateDirectory(xanaRoomSavePath);
        }
    }

    private void OnDisable()
    {
        /*if (ConstantsHolder.xanaConstants.r_MainSceneAvatar != null)
        {
            Destroy(ConstantsHolder.xanaConstants.r_MainSceneAvatar);
            ConstantsHolder.xanaConstants.r_MainSceneAvatar = null;
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        //bottomMainPanelButtons[2].gameObject.SetActive(false);        

        SetDefaultMode();
        Globle.isFade = false;
        OnBottomMainButtonDisable(false);

        PhotoOrVideoButtonUISetup(0);

        StartCoroutine(FalseTempDontClickObj());
    }    

    IEnumerator FalseTempDontClickObj()
    {
        tempDontClickObj.SetActive(true);
        yield return new WaitForSeconds(1f);
        tempDontClickObj.SetActive(false);
    }

    private void Update()
    {
        /*if (isImageCapture)
        {
            imageCaptureTime += Time.deltaTime;
        }*/
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNotificationMsg("Feed Created");
        }*/
        if (!m_CanRotateCharacter || m_isAnyUiScreenActive)
            return;
#if UNITY_EDITOR
        EditorControls();
#endif
#if UNITY_IOS || UNITY_ANDROID
        MobileControls();
#endif
    }

    [Header("For Rotation Avatar")]
    public bool m_CanRotateCharacter;
    public float m_CharacterRotationSpeed = 10;
    public bool m_isAnyUiScreenActive = false;

    void EditorControls()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.y < Screen.height - (Screen.height / 21) && Input.mousePosition.y > (Screen.height / 7))
            {
                if (mainAvatar.activeInHierarchy)
                {
                    mainAvatar.transform.rotation *= Quaternion.Euler(Vector3.up * -Input.GetAxis("Mouse X") * m_CharacterRotationSpeed);
                }
            }
        }
    }

    public void MobileControls()
    {
        if (Input.touchCount > 0)
        {
            Touch l_Touch = Input.GetTouch(0);

            if (Input.mousePosition.y < Screen.height - (Screen.height / 21) && Input.mousePosition.y > (Screen.height / 7))
            {
                if (mainAvatar.activeInHierarchy)
                    mainAvatar.transform.rotation *= Quaternion.Euler(Vector3.up * -l_Touch.deltaPosition.x * Time.deltaTime * m_CharacterRotationSpeed);
            }
        }
    }

    public void OnGoToHomeButtonClick()
    {
        if (videoCaptureButton.pressed)//if video capturing on then canceling video capture and back to main Room Screen.......
        {
            videoCaptureButton.CancelingVideoToBackButtonPress();
            return;
        }
        if (ConstantsHolder.xanaConstants.r_MainSceneAvatar != null)
        {
            Destroy(ConstantsHolder.xanaConstants.r_MainSceneAvatar);
            ConstantsHolder.xanaConstants.r_MainSceneAvatar = null;
        }
        Initiate.Fade("Home", loadToColor, 1.0f);
    }

    public void ARFaceManagerDisable(bool isDisable)
    {
       Debug.Log("ARFaceManagerDisable:" + isDisable);
        //if (aRFaceTrackManager != null)
        //{
        //    aRFaceTrackManager.enabled = isDisable;
        //    if (isDisable)
        //    {
        //        aRFaceTrackManager.ToggleFaceDetection();
        //    }
        //}
    }

    //this method is used to setup ui of photo video button ui.......
    void PhotoOrVideoButtonUISetup(int defaultIndex)
    {
        for (int i = 0; i < photoVideoButtonList.Count; i++)
        {
            if (i == defaultIndex)
            {
                photoVideoButtonList[i].transform.GetChild(0).gameObject.SetActive(true);
                photoVideoButtonList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                photoVideoButtonList[i].transform.GetChild(0).gameObject.SetActive(false);
                photoVideoButtonList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
        }

        if (defaultIndex == 1)
        {
            r_IsCaptureTypeImage = false;//capture type video.......

            captureButtonObj.transform.GetChild(1).gameObject.SetActive(true);
            captureButtonObj.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            r_IsCaptureTypeImage = true;//capture type image.......

            captureButtonObj.transform.GetChild(1).gameObject.SetActive(false);
            captureButtonObj.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    //this method is used to Update Ui of recording time and stop recording re update ui.......
    public void OnActiveOrDisableUiRecordingTime(bool isActive)
    {
        for (int i = 0; i < bottomUiPanel.transform.childCount; i++)
        {
            if (i != 2)
            {
                bottomUiPanel.transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }
        captureButtonObj.transform.GetChild(0).gameObject.SetActive(isActive);
        photoVideoPanel.SetActive(isActive);

        bottomMainTypePanelScrollView.SetActive(isActive);
    }

    //this method is used to on click photo or video button.......
    public void OnClickPhotoOrVideoButton(int index)
    {
        PhotoOrVideoButtonUISetup(index);
    }
     
    public void DisableBottomMainPanel(bool isDisable)
    {
        photoVideoPanel.SetActive(isDisable);
        bottomUiPanel.SetActive(isDisable);
        bottomMainTypePanelScrollView.SetActive(isDisable);

        topMainPanel.SetActive(isDisable);

        ARFaceManagerDisable(isDisable);
    }

    public void SetDefaultMode()
    {
        if (mainAvatar != null && SceneManager.GetActiveScene().name != "ARModulePlanDetectionScene")
        {
            Vector3 endScale = new Vector3(mainAvatarScale, mainAvatarScale, mainAvatarScale);
            mainAvatar.transform.localScale = endScale;

            mainAvatar.transform.position = MainAvatarDefaultPos;
        }
        
        if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
        {
            bottomUiPanel.transform.GetChild(3).GetComponent<Button>().interactable = false;
            bottomUiPanel.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
            bottomUiPanel.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "ARModuleRealitySceneOld")
        {
            bottomUiPanel.transform.GetChild(1).GetComponent<Button>().interactable = false;
            bottomUiPanel.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
            bottomUiPanel.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void SetDefaultAvatarPosition()
    {
        if (mainAvatar != null)
        {
            Vector3 endScale = new Vector3(mainAvatarScale, mainAvatarScale, mainAvatarScale);
            mainAvatar.transform.localScale = endScale;

            mainAvatar.transform.position = MainAvatarDefaultPos;
           Debug.Log("MainAvatarPos:" + mainAvatar.transform.position);
        }
    }
        
    public void OnFaceDetectionModeBtnClick()
    {
        Initiate.Fade("ARModuleFaceTrackingScene", loadToColor, 1.0f);
        OnBottomMainButtonDisable(false);
    }

    public void OnRoomModeBtnClick()
    {
        selectionItemScript.OnSelectedClick(0);
        Initiate.Fade("ARModuleRoomScene", loadToColor, 1.0f);
        OnBottomMainButtonDisable(false);
    }

    public void OnActionModeBtnClick()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("ar_body"))
        {            
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        selectionItemScript.OnSelectedClick(1);
        Initiate.Fade("ARModuleActionScene", loadToColor, 1.0f);
        OnBottomMainButtonDisable(false);
    }

    public void OnRealityModeBtnClick()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("ar_face"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        selectionItemScript.OnSelectedClick(2);
        Initiate.Fade("ARModuleRealityScene", loadToColor, 1.0f);
        OnBottomMainButtonDisable(false);
    }

    public void OnPlanDetectionModeBtnClick()
    {
        if (!UserPassManager.Instance.CheckSpecificItem("ar_ar"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
        selectionItemScript.OnSelectedClick(3);
        Initiate.Fade("ARModulePlanDetectionScene", loadToColor, 1.0f);       
        OnBottomMainButtonDisable(false);        
    }       

    public void OnBottomMainButtonDisable(bool isDesable)
    {
        if (!Globle.isFade)
        {
            Globle.isFade = true;
            for (int i = 0; i < bottomMainPanelButtons.Length; i++)
            {
                bottomMainPanelButtons[i].interactable = isDesable;
            }
            StartCoroutine(OnBottomMainButtonEnable());
        }
    }

    IEnumerator OnBottomMainButtonEnable()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < bottomMainPanelButtons.Length; i++)
        {
            bottomMainPanelButtons[i].interactable = true;
        }
        Globle.isFade = false;
    }

    public void SetUpMainAvatar(bool isActive, Vector3 pos, int rotation, float scale)
    {
        mainAvatar.SetActive(isActive);

        Vector3 endRotation = new Vector3(0, rotation, 0);
        mainAvatar.transform.rotation = Quaternion.Euler(endRotation);

        Vector3 headRotation = Vector3.zero;
        //mainAvatarHead.transform.localRotation = Quaternion.Euler(headRotation);

        mainAvatar.transform.position = pos;  

        Vector3 endScale = new Vector3(scale, scale, scale);
        mainAvatar.transform.localScale = endScale;
    }

    #region Capture screenshot.......
    public int captureId = 0;

    public void OnCaptureButtonPress()
    {
        //isImageCapture = true;
    }

    //this method is used to capture image disply screen back button click.......
    public void OnClickFinalCaptureScreenBackButton()
    {
        lastCapturedByteData = null;
        displayCatureImage.sprite = null;
        Resources.UnloadUnusedAssets();

        /*if (File.Exists(createFeedFilePath))
        {
            File.Delete(createFeedFilePath);
        }*/
    }

    public void OnCaptureButtonUp()
    {
        //isImageCapture = false;
        //if (imageCaptureTime <=1)
        if(r_IsCaptureTypeImage)
        {
            OnCaptureButtonClick(0);
        }
        //imageCaptureTime = 0;
    }

    public void OnCaptureButtonClick(int index = 0)
    {        
        captureId = index;
        uiCan.SetActive(false);
        if (captureId == 1)
        {
            LiveVideoRoomManager.Instance.ImageSelectionAllUIDisable(false);
        }
        if (SNSNotificationHandler.Instance != null)
        {
            SNSNotificationHandler.Instance.ResetAndInstantHideNotificationBar();
        }
        Capture();
    }
     
    private Camera newCam;
    private Texture2D screenshot;
    private RenderTexture screenshotRT;

    public void Capture()
    {
        GameObject g = new GameObject();
        g.transform.parent = Camera.main.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.Euler(Vector3.zero);
        g.SetActive(false);
        newCam = g.AddComponent<Camera>();

        newCam.enabled = false;

        screenshotRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        StartCoroutine(LoadTexture());
    }

    byte[] lastCapturedByteData;
    IEnumerator LoadTexture()
    {
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

       Debug.Log("Texture Size:" + screenshot.texelSize);
        //screenshot.Compress(true);
        uiCan.SetActive(true);
        if (captureId != 1)
        {
            displayCatureImage.sprite = captureSp;
            captureViewScreen.SetActive(true);
        }
        else
        {
            if (SNSNotificationHandler.Instance != null)
            {
                SNSNotificationHandler.Instance.ShowNotificationMsg("Save Photo success");
            }
            else
            {
                ShowNotificationMsg("Save Photo success");
            }
            LiveVideoRoomManager.Instance.ImageSelectionAllUIDisable(true);
        }

        lastCapturedByteData = screenshot.EncodeToPNG();
        string filename = "ArCapture" + System.DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + ".png";
        //NativeGallery.SaveImageToGallery(screenshot, "ArCapture", filename, null);

        string pathStr = Path.Combine(xanaRoomSavePath , filename);
        createFeedFileName = filename;
        createFeedFilePath = pathStr;

        //Debug.Log("path:" + createFeedFilePath);
        //File.WriteAllBytes(createFeedFilePath, lastCapturedByteData);
        //NativeGallery.SaveImageToGallery(imageByte, "ArCapture", filename);
    }
    #endregion

    #region pick Image and Video.......
    public void OnClickPickImageFromGallery(int index)
    {
        PickImage(1024, index);
    }

    private void PickImage(int maxSize, int index)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    return;
                }
                else
                {
                    byte[] pngBytes = File.ReadAllBytes(path);
                    texture.LoadImage(pngBytes);
                    texture.wrapMode = TextureWrapMode.Repeat;
                    texture.filterMode = FilterMode.Bilinear;
                    Sprite bg = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32);
                    if (index == 0)//for bg
                    {
                        albumSelectionScreen.SetActive(false);
                        LiveVideoRoomManager.Instance.BackgroundImage.gameObject.SetActive(true);
                        LiveVideoRoomManager.Instance.BackgroundImage.sprite = bg;
                        LiveVideoRoomManager.Instance.BackgroundImage.color = Color.white;
                    }
                    else if (index == 1)
                    {
                        DisableBottomMainPanel(false);
                        albumSelectionScreen.SetActive(false);
                        LiveVideoRoomManager.Instance.imageSelectionScreen.SetActive(true);
                        LiveVideoRoomManager.Instance.imageSelectionUIScreen.SetActive(true);
                        LiveVideoRoomManager.Instance.GetLastAvatarListCount(); 

                        LiveVideoRoomManager.Instance.GalleryImage.gameObject.SetActive(true);
                        LiveVideoRoomManager.Instance.GalleryImage.sprite = bg;
                        if (mainAvatar != null)
                        {
                          mainAvatar.SetActive(false);
                        }

                        lastCapturedByteData = pngBytes;
                        displayCatureImage.sprite = bg;
                        string filename = "ArRoomImage" + System.DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + ".png";

                        string pathStr = Path.Combine(xanaRoomSavePath, filename);
                        createFeedFileName = filename;
                        createFeedFilePath = pathStr;
                    }
                }
            }
        }, "Select a PNG image", "image/png");
        Debug.Log("Permission result: " + permission);
    }
     
    public void OnClickPickVideoFromGallery()
    {
        PickVideo();
    }

    private void PickVideo()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                /*long fileSizeibBytes = GetFileSize(path);
                long fileSizeibMbs = fileSizeibBytes / (1024 * 1024);
               Debug.Log("File size:" + fileSizeibMbs + " :long:" + fileSizeibBytes);
                if (fileSizeibMbs > 15)//check video file size and upload upto 15MG only.......
                {
                    SNSWarningMessageManager.Instance.ShowWarningMessage("Please upload video upto 15MB");
                    return;
                }*/

                /*if (mainAvatar != null)
                {
                    mainAvatar.gameObject.SetActive(false);
                }

                DisableBottomMainPanel(false);*/

                albumSelectionScreen.SetActive(false);

                ShowLoader(true);
                // Play the selected video
                //Handheld.PlayFullScreenMovie("file://" + path);               //Riken
                LiveVideoRoomManager.Instance.OnStartVideoPlay(path, true, true);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }

    static long GetFileSize(string FilePath)
    {
        if (File.Exists(FilePath))
        {
            return new FileInfo(FilePath).Length;
        }
        return 0;
    }
    #endregion

    #region Screen Transection Click Event.......
    public void OnBackGroundSelectionBtnClick()
    {
        FeedAnimationBGChange.instance.GetAllBackGroundImages();
        backGroundSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        backGroundSelectionScreen.SetActive(true);
        backGroundSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.1f).SetEase(Ease.Linear);

        m_isAnyUiScreenActive = true;//For Stop Avatar Rotation if any UI Screen Active.......
    }

    public void SelectionBorderOnBackgroundImage(int index)
    {
       Debug.Log("index:" + index);
        for (int i = 0; i < backGroundScreenContainer.childCount; i++)
        {
            if (i == index)
            {
                backGroundScreenContainer.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                backGroundScreenContainer.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void SelectionBorderOnBackgroundColor(int index)
    {
       Debug.Log("index:" + index);
        for (int i = 0; i < backgroundScreenColorContainer.childCount; i++)
        {
            if (i == index)
            {
                backgroundScreenColorContainer.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                backgroundScreenColorContainer.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void CharacterSelectionBoarderChange(int index)
    {
        for (int i = 0; i < characterSelectionContainer.childCount; i++)
        {
            if (i == index)
            {
                characterSelectionContainer.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                characterSelectionContainer.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }

            if (!characterSelectionContainer.GetChild(i).gameObject.activeSelf)
            {
                break;
            }
        }
    }

    public void OnFilterSelectionBtnClick()
    {
        print("Filter btn clicked");
        if (!UserPassManager.Instance.CheckSpecificItem("filter button"))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }
          

        filterSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        filterSelectionScreen.SetActive(true);
        filterSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.2f).SetEase(Ease.Linear);

        m_isAnyUiScreenActive = true;//For Stop Avatar Rotation if any UI Screen Active.......
    }

    public void OnFilterBackBtnClick()
    {
        filterSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        StartCoroutine(CloseOpenScreen(filterSelectionScreen));

        m_isAnyUiScreenActive = false;//For Stop Avatar Rotation if any UI Screen Active.......
    }

    public void OnCharacterSelectionBtnClick()
    {
        print("Gesture btn clicked");
        if (!UserPassManager.Instance.CheckSpecificItem("SNS Emote"))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }  
        else
        {
            print("Horayyy you have Access");
        }  


        characterSelectionScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
        characterSelectionScreen.SetActive(true);
        characterSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.2f).SetEase(Ease.Linear);

        characterSelectionContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);

        m_isAnyUiScreenActive = true;//For Stop Avatar Rotation if any UI Screen Active.......
    }

    public void OnAlbumBtnClick()
    {
        albumSelectionScreen.SetActive(true);
    }

    public void OnAlbumeCancelBtnClick()
    {
        albumSelectionScreen.SetActive(false);
    }

    public void OnScreenCloseBtnClick()
    {
        if (backGroundSelectionScreen.activeSelf)
        {
            backGroundSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.2f).SetEase(Ease.Linear);
            StartCoroutine(CloseOpenScreen(backGroundSelectionScreen));
        }
        else if (filterSelectionScreen.activeSelf)
        {
            filterSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.2f).SetEase(Ease.Linear);
            StartCoroutine(CloseOpenScreen(filterSelectionScreen));
        }
        else if (characterSelectionScreen.activeSelf)
        {
            characterSelectionScreen.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.2f).SetEase(Ease.Linear);
            StartCoroutine(CloseOpenScreen(characterSelectionScreen));
        }

        m_isAnyUiScreenActive = false;//For Stop Avatar Rotation if any UI Screen Active.......
    }

    IEnumerator CloseOpenScreen(GameObject openScreen)
    {
        yield return new WaitForSeconds(0.4f);
        openScreen.SetActive(false);
    }

    public void CloseAndResetScreenAfterPostFeed()
    {
        if (captureViewScreen.activeSelf)
        {
            captureViewScreen.SetActive(false);
        }
        else if (LiveVideoRoomManager.Instance.videoPlayerUIScreen.activeSelf)
        {
            LiveVideoRoomManager.Instance.CloseVideoScreenBtnClick();
            createFeedFilePath = "";
        } 
        else if (LiveVideoRoomManager.Instance.imageSelectionUIScreen.activeSelf)
        {
            LiveVideoRoomManager.Instance.CloseImageScreenBtnClick();
        }

        switch (imageOrVideo)
        {
            case "Image":
                if (File.Exists(createFeedFilePath))
                {
                    lastCapturedByteData = null;
                    displayCatureImage.sprite = null;
                    CreatePostImage.sprite = null;
                    Resources.UnloadUnusedAssets();
                    //Caching.ClearCache();
                    //GC.Collect();
                }
                break;
            case "Video":
                if (File.Exists(createFeedFilePath) && !LiveVideoRoomManager.Instance.isPickVideoFromGellary)
                {
                    File.Delete(createFeedFilePath);
                }
                break;
            default:
                break;
        }

        OnClickCreatePostBackButton();
    }
    #endregion

    #region Notification msg reference.......
    public void ShowNotificationMsg(string msg)
    {
        notificationText.text = TextLocalization.GetLocaliseTextByKey(msg);
        //notificationText.GetComponent<TextLocalization>().LocalizeTextText();
        notificationScreen.GetComponent<RectTransform>().DOAnchorPosY(-50, 0.3f).SetEase(Ease.Linear);
        Invoke("NotificationScreenClose", 1f);
    }

    void NotificationScreenClose()
    {
        notificationScreen.GetComponent<RectTransform>().DOAnchorPosY(250, 0.2f).SetEase(Ease.Linear);
    }

    public void OnMainButtomMenuDisableForDelete(bool isDisable)
    {
        if (!LiveVideoRoomManager.Instance.videoPlayScreen.activeSelf && !LiveVideoRoomManager.Instance.imageSelectionScreen.activeSelf)
        {
            bottomUiPanel.SetActive(isDisable);
            bottomMainTypePanelScrollView.SetActive(isDisable);
            topMainPanel.SetActive(isDisable);
        }
    }

    public void OnDeleteScreenActive(bool isActive)
    {
        deleteItemScreen.SetActive(isActive);
        OnMainButtomMenuDisableForDelete(!isActive);
        if (!isActive)
        {
            deleteItemWithDragScript.isPointerEnter = false;
        }
        //Debug.Log("deleteItemScreen:" + deleteItemWithDragScript.isPointerEnter);
    }

    public void OnDeleteItemObject()
    {
        if (deleteItemScreen.activeSelf)
        {
            //Debug.Log("ispointerEnter111111:" + deleteItemWithDragScript.isPointerEnter);
            if (deleteItemWithDragScript.isPointerEnter)
            {
                if (addAvtarItem.Contains(currentSelectedItemObj))
                {
                    GameObject crntObj = currentSelectedItemObj;
                    addAvtarItem.Remove(crntObj);
                    Destroy(crntObj);
                }
                else if (mainAvatar != null)
                {
                    GameObject crntObj = currentSelectedItemObj;
                    Destroy(crntObj);
                    mainAvatar = null;
                    //mainAvatarHead = null;
                }
                else 
                {
                    GameObject crntObj = currentSelectedItemObj;
                    Destroy(crntObj);
                    
                }
                currentSelectedItemObj = null;
            }
        }
    }
    #endregion

    #region API Loader Screen
    public SNSAPILoaderController apiLoaderController;

    public void ShowLoader(bool isActive)
    {
        apiLoaderController.ShowApiLoader(isActive);
    }
    #endregion

    #region Create Post
    [Space]
    [Header("Create Post")]
    public GameObject CreatePostScreen;
    public TMP_InputField createFeedTitle;
    public AdvancedInputField createFeedTitleAdvance;
    public TMP_InputField createFeedDescription;
    public AdvancedInputField createFeedDescriptionAdvance;

    public Image CreatePostImage;

    public MediaPlayer feedMediaPlayer;
    public GameObject videoDisplay;

    [Space]
    public string imageOrVideo = "";
    public string createFeedFilePath;
    public string createFeedFileName;

    public void VideoScreenOrImagePostBtnClick(string fileType)
    {
        print("Post btn clicked");

        if (!UserPassManager.Instance.CheckSpecificItem("post button"))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }    
           
        CreatePostImage.gameObject.SetActive(false);
        feedMediaPlayer.gameObject.SetActive(false);
        videoDisplay.SetActive(false);

        //createFeedTitle.text = "";
        createFeedTitleAdvance.Text = "";
        //createFeedDescription.text = "";
        createFeedDescriptionAdvance.Text = "";

        imageOrVideo = fileType;
        switch (imageOrVideo)
        {
            case "Image":
                File.WriteAllBytes(createFeedFilePath, lastCapturedByteData);               
                CreatePostImage.sprite = displayCatureImage.sprite;
                CreatePostImage.gameObject.SetActive(true);
                break;
            case "Video":

                createFeedFilePath= LiveVideoRoomManager.Instance.videoPath;

                string[] pathArry = createFeedFilePath.Split('/');

                //createFeedFileName = pathArry[pathArry.Length - 1];
                createFeedFileName = Path.GetFileName(createFeedFilePath);

                feedMediaPlayer.gameObject.SetActive(true);
                videoDisplay.SetActive(true);

                feedMediaPlayer.OpenMedia(new MediaPath(createFeedFilePath, MediaPathType.AbsolutePathOrURL), autoPlay: true);

               Debug.Log("media path:"+ feedMediaPlayer.MediaPath.Path);
                break;
            default:
                break;
        }
        CreatePostScreen.SetActive(true);
    }


    //this method is used to create feed post.......
    //public void OnClickCreatePostButton()
    //{
    //    ShowLoader(true);//active loader
    //    string iscompress = "";
    //    if (imageOrVideo == "Image")
    //    {
    //        iscompress = "true";
    //    }
    //    else if(imageOrVideo == "Video")
    //    {
    //        feedMediaPlayer.Pause();
    //    }
    //    AWSHandler.Instance.PostObjectFeed(createFeedFilePath, createFeedFileName, "CreateFeedRoom", iscompress);
    //}


    //public void CreateFeedAPICall(string url, string thumbnail)
    //{
    //   Debug.Log("Room Feed Create:" + url);
    //    switch (imageOrVideo)
    //    {
    //        case "Image":
    //            //string s1 = createFeedTitle.text;
    //            //string s1 = createFeedTitleAdvance.RichText;
    //            string s1 = SNS_APIManager.Instance.userName;
    //            //string s2 = createFeedDescription.text;
    //            string s2 = createFeedDescriptionAdvance.RichText;

    //            if (string.IsNullOrEmpty(s1))
    //            {
    //                s1 = "@new";
    //            }
    //            else
    //            {
    //                s1 = "@" + s1;
    //            }

    //            if (string.IsNullOrEmpty(s2))
    //            {
    //                s2 = "  ";
    //            }

    //            //SNS_APIManager.Instance.RequestCreateFeed(SNS_APIManager.EncodedString(s1), SNS_APIManager.EncodedString(s2), url, "", thumbnail, "true", "", "RoomCreateFeed");
    //            break;
    //        case "Video":
    //            //string s11 = createFeedTitle.text;
    //            //string s11 = createFeedTitleAdvance.RichText;
    //            string s11 = SNS_APIManager.Instance.userName;
    //            //string s22 = createFeedDescription.text;
    //            string s22 = createFeedDescriptionAdvance.RichText;

    //            if (string.IsNullOrEmpty(s11))
    //            {
    //                s11 = "@new";
    //            }
    //            else
    //            {
    //                s11 = "@" + s11;
    //            }

    //            if (string.IsNullOrEmpty(s22))
    //            {
    //                s22 = "  ";
    //            }
    //            SNS_APIManager.Instance.RequestCreateFeed(SNS_APIManager.EncodedString(s11), SNS_APIManager.EncodedString(s22), "", url, thumbnail, "true", "", "RoomCreateFeed");
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public void OnClickCreatePostBackButton()
    {
        switch (imageOrVideo)
        {
            case "Image":
                if (File.Exists(createFeedFilePath))
                {
                    File.Delete(createFeedFilePath);
                }
                break;
            default:
                break;
        }
        CreatePostScreen.SetActive(false);
    }

    public void CreateFeedSuccess()
    {        
        ShowLoader(false);
        if (SNSNotificationHandler.Instance != null)
        {
            SNSNotificationHandler.Instance.ShowNotificationMsg("Feed Created");
        }
        else
        {
            ShowNotificationMsg("Feed Created");
        }

        CloseAndResetScreenAfterPostFeed();//reset all screen after post image 
    }
    #endregion

    public static class Globle
    {
        public static bool isFade = false;
    }
}