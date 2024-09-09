using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Video;
using System.Threading.Tasks;
using SuperStar.Helpers;
using Unity.VisualScripting;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;
    public static bool StopLoader;

    [Header("Loading UI Elements")]
    public GameObject loadingPanel;

    public Image loadingSlider;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingPercentageText;

    public Image JJLoadingSlider;
    public TextMeshProUGUI JJLoadingPercentageText;

    [Header("Loading BG Elements")]
    public Image loadingBgImage;
    public Image loadingBgImageAlter;
    public Sprite[] loadingSprites;

    public float fadeTimer;
    bool isFirstTime = true;
    
    /// <summary>
    /// Help Screen Arrays for 2 scenarios.
    /// If loading percentage is less than 50 only display helpScreenOne items
    /// else loading percentage is greater or equal to 50 display helpScreenTwo items
    /// </summary>
    [Header("Loading Help Screens UI")]
    public GameObject[] helpScreensOne;
    public GameObject[] helpScreensTwo;

    public GameObject Loading_WhiteScreen;
    public GameObject nftvideoloader;
    private int currentBgIndex = 0;
    private int aheadBgIndex = 1;

    [Header("Character Loading")]
    public GameObject characterLoading;
    // Added by Waqas
    public GameObject presetCharacterLoading;

    [Header("World Loading")]
    public GameObject worldLoadingScreen;

    [Header("Loader While NFT Loading in BG")]
    public GameObject nftLoadingScreen;
    public GameObject LoadingScreenSummit;
    [Header("fader For Villa")]
    public Image fader;

    [Header("Loader For Event")]
    public GameObject EventLoaderCanvas;


    [Header("JJ WORLD TELEPORT")]
    public CanvasGroup teleportFeader;
    public GameObject teleportFeaderLandscape, teleportFeaderPotraite;

    [Header("Store Loading")]
    public GameObject storeLoadingScreen;

    [Header("Dome Loading")]
    public GameObject VideoLoading;
    public RenderTexture Texture16x9;
    public RenderTexture Texture9x16;

    public GameObject DomeLoading;
    public GameObject DomeLodingUI;
    public GameObject ApprovalUI;
    public Image DomeThumbnail;
    public TextMeshProUGUI DomeName;
    public TextMeshProUGUI DomeDescription;
    public TextMeshProUGUI DomeProgress;
    public TextMeshProUGUI DomeCreator;
    public TextMeshProUGUI DomeType;
    public TextMeshProUGUI DomeCategory;
    public TextMeshProUGUI DomeEstimateTime;
    public TextMeshProUGUI DomeID;
    public RectTransform LoadingStatus;

    public ManualRoomController manualRoomController;
    public StreamingLoadingText streamingLoading;
    public string Aalternate;
    public bool enter = false, WaitForInput = false;
    public float currentValue = 0;
    private float timer = 0;
    public bool isLoadingComplete = false;
    public float randCurrentValue = 0f;
    private float sliderFinalValue = 0;
    private float sliderCompleteValue = 0f;
    private float originalWidth;

    public GameObject SearchLoadingCanvas;
    private CanvasGroup canvasGroup;
    bool Autostartslider = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);

        loadingText.text = "";
        manualRoomController = gameObject.GetComponent<ManualRoomController>();
      //  Debug.unityLogger.logEnabled = true;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
                        Debug.unityLogger.filterLogType = LogType.Error;
#endif

        if (LoadingStatus != null)
        {
            originalWidth = LoadingStatus.sizeDelta.x;
           
        }
    }

    private void Start()
    {
        sliderFinalValue = Random.Range(80f, 95f);
        sliderCompleteValue = 100;// Random.Range(96f, 99f);
        StartCoroutine(StartBGChange());
        canvasGroup = GetComponent<CanvasGroup>();
    }

    IEnumerator StartBGChange()
    {
        loadingBgImage.sprite = loadingSprites[currentBgIndex];

        loadingBgImage.DOFade(1, 0);
        loadingBgImageAlter.DOFade(0, 0);

        yield return new WaitForSeconds(2.0f + fadeTimer);

        loadingBgImageAlter.sprite = loadingSprites[aheadBgIndex];

        loadingBgImage.DOFade(0, fadeTimer);
        loadingBgImageAlter.DOFade(1, fadeTimer);

        yield return new WaitForSeconds(fadeTimer * 2);

        currentBgIndex += 1;
        aheadBgIndex += 1;

        if (currentBgIndex >= loadingSprites.Length)
        {
            currentBgIndex = 0;
        }

        if (aheadBgIndex >= loadingSprites.Length)
        {
            aheadBgIndex = 0;
        }

        StartCoroutine(StartBGChange());
    }

    public void UpdateLoadingStatusText(string message)
    {
        var text = message;
        text = text.Replace("a", Aalternate);
        if (!string.IsNullOrEmpty(text) && !text.Contains("..."))
        {
            text += "...  ";
        }
       
        loadingText.text = text.ToUpper();
        loadingText.GetComponent<TextLocalization>().LocalizeTextText(message);
        loadingText.GetComponent<TextLocalization>().LocalizeTextText();
    }

    public void UpdateLoadingSlider(float value, bool doLerp = false)
    {
        if (doLerp)
        {
            loadingSlider.DOFillAmount(value, 0.15f);
        }
        else
        {
            loadingSlider.fillAmount = value;
        }
        loadingPercentageText.text =" "+ ((int)(value * 100f)).ToString() + "%";

    }
    public void UpdateLoadingSliderForJJ(float value, float fillSpeed, bool doLerp = false)
    {
        value = value * 100;
        value = value - (value % 5f);
        value = value / 100;
        if (doLerp)
        {
            JJLoadingSlider.DOFillAmount(value, fillSpeed);
        }
        else
        {
            JJLoadingSlider.fillAmount = value;
        }
        JJLoadingPercentageText.text = ((int)(value * 100f)).ToString() + "%";
    }

    public void ShowLoading()
    {
        //Debug.LogError("TeleportFeader: " + teleportFeader.gameObject.activeInHierarchy + " ~~~~~~~  Activated Loading ~~~~~~~ ");
        if (teleportFeader.gameObject.activeInHierarchy) // ConstantsHolder.xanaConstants.JjWorldSceneChange
        {
            return;
        }
        ResetLoadingValues();
        //Debug.LogError(Screen.orientation + " ~~~~~~~  Activated Loading ~~~~~~~ " + oriantation);
        //bool isFedderActive = false;
        //if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            //isFedderActive = true;
            if (ConstantsHolder.xanaConstants.isBackFromWorld)
            {
                if (ScreenOrientationManager._instance != null && ScreenOrientationManager._instance.isPotrait)
                {
                    ActivateFadder_AtLoadingStart();
                }
                else
                {
                    CustomLoading();
                }
            }
            else
            {
                ActivateFadder_AtLoadingStart();
            }
           
        }

        //StartCoroutine(CustomLoading());
    }

    void ActivateFadder_AtLoadingStart()
    {
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOKill();
        blackScreen.color = new Color(1, 1, 1, 1);
        // Adding Delay Time
        blackScreen.DOFade(1, 0.5f).OnComplete(delegate
        {
            //Debug.LogError("7 ~~~~~~~~~~~~~~~~ LandscapeLeft");
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            CustomLoading();
            //Debug.LogError(" ~~~~~~~  Oriantation Change Called ~~~~~~~ " );
        });
    }
   
    void CustomLoading()
    {
        //if (needWait)
        //{
        //    Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        //    yield return new WaitForSeconds(1f);
        //    blackScreen.DOFade(0, 0.2f).SetDelay(1f);
        //}
        //if (!loadingPanel.activeInHierarchy)
        //{
        //    loadingPanel.SetActive(true);
        //}

       
        loadingPanel.SetActive(true);
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.2f).SetDelay(0f);
      

        if (gameplayLoadingUIRefreshCo != null)//rik for refresh screen on every 5-7 second.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
        }
        isScreenRefresh = true;
        gameplayLoadingUIRefreshCo = StartCoroutine(IEGameplayLoadingScreenUIRefresh());

        if (ConstantsHolder.xanaConstants.needToClearMemory)
            AddressableDownloader.Instance.MemoryManager.RemoveAllAddressables();
        else
            ConstantsHolder.xanaConstants.needToClearMemory = true;
    }
   
    public void ResetLoadingValues()
    {
        //if (GameplayEntityLoader.instance)
        //{
        //    GameplayEntityLoader.instance.isEnvLoaded = false;
        //}
        currentValue = 0;
        isLoadingComplete = false;
        timer = 0;
        loadingSlider.fillAmount = 0f;
        loadingPercentageText.text = " 0%".ToString();
        JJLoadingSlider.fillAmount = 0f;
        JJLoadingPercentageText.text = " 0%".ToString();
        LoadingStatus.anchorMax = new Vector2(0, LoadingStatus.anchorMax.y);
        DomeProgress.text = "00";
    }

    public void HideLoading()
    {
        if (isFirstTime || teleportFeader.gameObject.activeInHierarchy)
        {
            isFirstTime = false;
            ConstantsHolder.xanaConstants.isBackFromWorld = false;
            return;
        }

        if (!loadingPanel.activeInHierarchy)
            return;

        if (!ConstantsHolder.xanaConstants.isFromXanaLobby && ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            
            Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
            blackScreen.DOKill();
            blackScreen.DOFade(1f, 0.01f).OnComplete(async () =>
            {
                loadingPanel.SetActive(false);
                await Task.Delay(1000);
                if (ConstantsHolder.xanaConstants.isBackFromWorld)
                    Screen.orientation = ScreenOrientation.Portrait;

                ConstantsHolder.xanaConstants.isBackFromWorld = false;
                CustomHideLoading();
            });
        }
        else
        {
            CustomHideLoading();
        }


    }

    void CustomHideLoading()
    {
        StartCoroutine(ApplyDelay());

        if (ReferencesForGamePlay.instance != null)
            ReferencesForGamePlay.instance.workingCanvas.SetActive(true);

        if (gameplayLoadingUIRefreshCo != null)
            StopCoroutine(gameplayLoadingUIRefreshCo);
    }

    private IEnumerator ApplyDelay()
    {
        if (ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            yield return null; // Wait for one frame (to ensure synchronization)
            HideLoadingManually();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            if (GameplayEntityLoader.instance)
            {
                GameplayEntityLoader.instance.SetPlayerPos();
            }
            HideLoadingManually();
        }
    }

    private void HideLoadingManually()
    {
        LoadingHandler.Instance.UpdateLoadingStatusText("");
        loadingPanel.SetActive(false);
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.1f).SetDelay(0.5f);
    }


    bool orientationchanged = false;
    public void ShowFadderWhileOriantationChanged(ScreenOrientation oriantation)
    {
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOKill();
#if !UNITY_EDITOR

               // Removing Delay Time 
                blackScreen.DOFade(1, 0f);
                Screen.orientation = oriantation;
                orientationchanged = false;
                StartCoroutine(Check_Orientation(oriantation));


     //blackScreen.DOFade(1, 0.15f).OnComplete(delegate 
     //       {
     //           Screen.orientation = oriantation;
     //           orientationchanged = false;
     //           StartCoroutine(Check_Orientation(oriantation));
     //            });
#else

        Screen.orientation = oriantation;
#endif

        //Invoke(nameof(HideFadderAfterOriantationChanged), 2f);
    }
    public void HideFadderAfterOriantationChanged(float delay = 0)
    {
        // Debug.LogError("~~~~~~~  Fadder Out ~~~~~~~ " );
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.5f).SetDelay(delay);
        ConstantsHolder.xanaConstants.isBackFromWorld = false;
    }

    private IEnumerator Check_Orientation(ScreenOrientation oriantation)
    {
    CheckAgain:
        //  Debug.LogError(Screen.orientation + " ~~~~~~~ Oriantation Checking ~~~~~~~ " + oriantation);
        yield return new WaitForSeconds(.2f);
        if (Screen.orientation == oriantation || ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            //if(!ConstantsHolder.xanaConstants.isBackFromWorld)
            //    HideFadderAfterOriantationChanged();
        }
        else
        {
            Screen.orientation = oriantation;
            goto CheckAgain;
        }

    }




    public bool GetLoadingStatus()
    {
        return loadingPanel.activeInHierarchy;
    }

    //rik create coroutine for env loading screen refresh every 5-7 second.......
    Coroutine gameplayLoadingUIRefreshCo;
    bool isScreenRefresh = false;
    IEnumerator IEGameplayLoadingScreenUIRefresh()
    {
        //Debug.Log("RefreshLoading screen");
        ChangeHelpScreenUI(isScreenRefresh);
        isScreenRefresh = !isScreenRefresh;
        yield return new WaitForSeconds(UnityEngine.Random.Range(5, 7));
        gameplayLoadingUIRefreshCo = StartCoroutine(IEGameplayLoadingScreenUIRefresh());
    }

    /// <summary>
    /// Switch between HelpDialogs
    /// </summary>
    /// <param name="isFirst"> 
    /// if isFirst is TRUE then helpScreenOne items will be displayed
    /// and helpScreenTwo items will be hidden </param>
    public void ChangeHelpScreenUI(bool isFirst)
    {
        if (isFirst)
        {
            foreach (GameObject _helpDialog in helpScreensOne)
            {
                _helpDialog.SetActive(true);
            }
            foreach (GameObject _helpDialog in helpScreensTwo)
            {
                _helpDialog.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject _helpDialog in helpScreensOne)
            {
                _helpDialog.SetActive(false);
            }
            foreach (GameObject _helpDialog in helpScreensTwo)
            {
                _helpDialog.SetActive(true);
            }
        }

    }
    public void OnCloasenft()
    {
        nftvideoloader.SetActive(false);
    }
    public void OnLoadnft()
    {
        nftvideoloader.SetActive(true);
    }


    public IEnumerator ShowLoadingForCharacterUpdation(float delay)
    {
        if(canvasGroup.alpha == 0)
            canvasGroup.alpha = 1;
        characterLoading.SetActive(true);
        yield return new WaitForSeconds(delay);
        characterLoading.SetActive(false);
    }


    public IEnumerator FadeIn()
    {
        fader.gameObject.SetActive(true);
        // loop over 1 second
        for (float i = 0; i <= 1; i += (Time.deltaTime*5))
        {
            // set color with i as alpha
            fader.color = new Color(0, 0, 0, i);
            yield return null;
        }

    }

    public IEnumerator FadeOut()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fader.color = new Color(0, 0, 0, i);
            yield return null;
        }
        fader.gameObject.SetActive(false);
    }


    public AsyncOperation LoadSceneByIndex(string sceneName, bool isBuilder = false,LoadSceneMode mode = LoadSceneMode.Single)
    {
        //UpdateLoadingSlider(.2f);
        if (ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            StartCoroutine(IncrementSliderValue((randCurrentValue > 0) ? randCurrentValue : Random.Range(6f, 10f)));
        }
        else
        {
            if (isBuilder)
            {
                StartCoroutine(IncrementSliderValue((randCurrentValue > 0) ? randCurrentValue : Random.Range(25f, 30f)));
            }
            else
                StartCoroutine(IncrementSliderValue(Random.Range(10f, 13f)));
        }
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName,mode);
        return asyncOperation;
    }

    public IEnumerator IncrementSliderValue(float speed, bool loadMainScene = false)
    {
      
     
        while (currentValue < sliderCompleteValue)  
        {
            while(StopLoader && currentValue>30)
            {
                yield return null;
            }
            timer += Time.deltaTime;
            currentValue = Mathf.Lerp(0, sliderFinalValue, timer / speed);
            if ((ConstantsHolder.xanaConstants.isFromXanaLobby || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld)) &&
                teleportFeader.gameObject.activeInHierarchy || ConstantsHolder.xanaConstants.isFromTottoriWorld)
            {
                JJLoadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                JJLoadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
            }
            else if (ConstantsHolder.isFromXANASummit) {
            
              LoadingStatus.DOAnchorMax(new Vector2(currentValue / 100, LoadingStatus.anchorMax.y), 0.15f); ;
                DomeProgress.text = ((int)(currentValue)).ToString();
            }
            else
            {
                loadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                loadingPercentageText.text = " " + ((int)(currentValue)).ToString() + "%";
               
               
            }


            if (GameplayEntityLoader.instance && !loadMainScene)
            {
                if (GameplayEntityLoader.instance.isEnvLoaded)
                {
                    isLoadingComplete = true;
                }
                else if (currentValue > 75f)
                {
                    isLoadingComplete = true;
                }
            }
            else if(loadMainScene)
            {
                if (currentValue > 35f)
                {
                    isLoadingComplete = true;
                }
            }
            if (isLoadingComplete)
            {
                currentValue = sliderCompleteValue;
                if ((ConstantsHolder.xanaConstants.isFromXanaLobby || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld)) &&
                    teleportFeader.gameObject.activeInHierarchy || ConstantsHolder.xanaConstants.isFromTottoriWorld)
                {
                    JJLoadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                    JJLoadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
                   // yield return new WaitForSeconds(1f);
                    //HideLoading(ScreenOrientation.Portrait);
                }
                if (ConstantsHolder.isFromXANASummit)
                {
                    LoadingStatus.DOAnchorMax(new Vector2(currentValue / 100, LoadingStatus.anchorMax.y), 0.15f); ;
                    DomeProgress.text = ((int)(currentValue)).ToString();
                }
                else
                {
                    loadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                    loadingPercentageText.text = " " + ((int)(currentValue)).ToString() + "%";
                    //yield return new WaitForSeconds(1f);
                 
                    //HideLoading(ScreenOrientation.Portrait);
                }
            }
            yield return null;
        }
    }

    public IEnumerator TeleportFader(FadeAction action)
    {
        // teleportFeader.gameObject.SetActive(true);
        switch (action)
        {
            case FadeAction.Out:
                teleportFeader.DOFade(0, 0.5f).OnComplete(() =>
                {
                    teleportFeader.gameObject.SetActive(false);
                    teleportFeaderLandscape.SetActive(false);
                    teleportFeaderPotraite.SetActive(false);
                });
                break;
            case FadeAction.In:
                if (ConstantsHolder.xanaConstants != null)
                {
                    teleportFeaderLandscape.SetActive(!ConstantsHolder.xanaConstants.orientationchanged);
                    teleportFeaderPotraite.SetActive(ConstantsHolder.xanaConstants.orientationchanged);
                }
                else
                {
                    teleportFeaderLandscape.SetActive(true);
                }
                if (!teleportFeader.gameObject.activeInHierarchy)
                {
                    currentValue = 0;
                    isLoadingComplete = false;
                    timer = 0;
                    JJLoadingSlider.fillAmount = 0f;
                    JJLoadingPercentageText.text = "0%".ToString();
                }

                teleportFeader.gameObject.SetActive(true);
                teleportFeader.DOFade(1, 0.5f);
                break;
            default:
                break;
        }
        yield return null;
    }

    public void ShowVideoLoading()
    {
        if(ScreenOrientationManager._instance != null)
        {
            if (ScreenOrientationManager._instance.isPotrait)
                VideoLoading.GetComponent<VideoPlayer>().targetTexture = Texture9x16;
            else
                VideoLoading.GetComponent<VideoPlayer>().targetTexture = Texture16x9;

            VideoLoading.GetComponent<RawImage>().texture = VideoLoading.GetComponent<VideoPlayer>().targetTexture;
        }
        VideoLoading.SetActive(true);
    }

    public void showDomeLoading(XANASummitDataContainer.StackInfoWorld info)
    {
        if (!string.IsNullOrEmpty(info.thumbnail))
        {
            DomeThumbnail.gameObject.SetActive(true);
            if (AssetCache.Instance.HasFile(info.thumbnail))
            {
                AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, info.thumbnail);

            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(info.thumbnail, info.thumbnail, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, info.thumbnail, changeAspectRatio: true);

                    }
                });
            }
        }
        else { DomeThumbnail.gameObject.SetActive(false); }
        ResetLoadingValues();
        DomeLoading.SetActive(true);
        DomeName.text = info.name;
        DomeDescription.text = info.description;
        DomeCreator.text = info.creator;
      
        Debug.Log("Dome id " + info.domeId);
        if (info.domeId > 0 && info.domeId < 9)
        {
            DomeCategory.text = "Center";
            DomeID.text = "CA-" + info.domeId;
        }

        if (info.domeId > 8 && info.domeId < 39)
        {
            DomeCategory.text = "Business";
            DomeID.text = "BA-" + info.domeId;
        }

        if (info.domeId > 38 && info.domeId < 69)
        {
            DomeCategory.text = "Web 3";
            DomeID.text = "WA-" + info.domeId;
        }

        if (info.domeId > 68 && info.domeId < 99)
        {
            DomeCategory.text = "Game";
            DomeID.text = "GA-" + info.domeId;
        }
        if (info.domeId > 98 && info.domeId < 129)
        {
            DomeCategory.text = "Entertainmnent";
            DomeID.text = "EA-" + info.domeId;
        }
        if (info.domeId > 128 && info.domeId < 161)
        {
            DomeCategory.text = "Entertainmnent";
            DomeID.text = "MD   -" + info.domeId;
        }
        DomeEstimateTime.text = "1 min.";
        ApprovalUI.SetActive(false);
        DomeLodingUI.SetActive(true);
        StartCoroutine(IncrementSliderValue(Random.Range(0f, 5f)));
    }

    public void showApprovaldomeloading(XANASummitDataContainer.DomeGeneralData info)
    {
        WaitForInput = true;
        if (!string.IsNullOrEmpty(info.world360Image))
        {
            DomeThumbnail.gameObject.SetActive(true);
            if (AssetCache.Instance.HasFile(info.world360Image))
            {
                AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, info.world360Image);

            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(info.world360Image, info.world360Image, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, info.world360Image, changeAspectRatio: true);

                    }
                });
            }
        }else { DomeThumbnail.gameObject.SetActive(false);}
        ResetLoadingValues();
        DomeLoading.SetActive(true);
        DomeName.text = info.name;
        DomeDescription.text = info.description;
        DomeCreator.text = info.creatorName;
        DomeType.text = info.experienceType;
        Debug.Log("Dome id " + info.id);
        if(info.id>0 && info.id < 9)
        {
            DomeCategory.text = "Center";
            DomeID.text = "CA-"+ info.id;
        }

        if (info.id > 8 && info.id < 39)
        {
            DomeCategory.text = "Business";
            DomeID.text = "BA-" + info.id;
        }

        if (info.id > 38 && info.id < 69)
        {
            DomeCategory.text = "Web 3";
            DomeID.text = "WA-" + info.id;
        }

        if (info.id > 68 && info.id < 99)
        {
            DomeCategory.text = "Game";
            DomeID.text = "GA-" + info.id;
        }
        if (info.id > 98 && info.id < 129)
        {
            DomeCategory.text = "Entertainmnent";
            DomeID.text = "EA-" + info.id;
        }
        if (info.id > 128 && info.id < 161)
        {
            DomeCategory.text = "Entertainmnent";
            DomeID.text = "MD   -" + info.id;
        }
        DomeEstimateTime.text = "1 min.";
        ApprovalUI.SetActive(true);
        DomeLodingUI.SetActive(false);
        if (info.worldType) { Autostartslider = false; } else { Autostartslider = true; }
    }
    public void showApprovaldomeloading(XANASummitSceneLoading.SingleWorldInfo info, XANASummitDataContainer.OfficialWorldDetails selectedWold)
    {
        WaitForInput = true;
        if (!string.IsNullOrEmpty(selectedWold.icon))
        {
            DomeThumbnail.gameObject.SetActive(true);
            if (AssetCache.Instance.HasFile(selectedWold.icon))
            {
                AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, selectedWold.icon);

            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(selectedWold.icon, selectedWold.icon, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(DomeThumbnail, selectedWold.icon, changeAspectRatio: true);

                    }
                });
            }
        }
        else { DomeThumbnail.gameObject.SetActive(false); }
        ResetLoadingValues();
        DomeLoading.SetActive(true);
        DomeName.text = info.data.name;
        DomeDescription.text = selectedWold.description;
        DomeCreator.text = selectedWold.creatorName;
        DomeType.text = selectedWold.subWorldType;
        DomeCategory.text = selectedWold.subWorldCategory;
        ApprovalUI.SetActive(true);
        DomeLodingUI.SetActive(false);
        if (info.data.entityType == "USER_WORLD") { Autostartslider = false; } else { Autostartslider = true; }
    }
    public async  void EnterDome()
    {
        enter = true;
        WaitForInput = false;
        ApprovalUI.SetActive(false);
        DomeLodingUI.SetActive(true);
        await Task.Delay(500);
        StartCoroutine(IncrementSliderValue(Random.Range(0f, 5f)));

    }
    public void ReturnDome()
    {
        enter = false;
        WaitForInput = false;
        DomeLoading.SetActive(false);
    }

    public void DisableVideoLoading()
    {
        VideoLoading.SetActive(false);
    }

    internal void DisableDomeLoading()
    {
        DomeLoading.SetActive(false);
    }

    public void DomeLoadingProgess(float progress)
    {
        Debug.Log("Loading progress...");
        LoadingStatus.DOAnchorMax(new Vector2(currentValue / 100, LoadingStatus.anchorMax.y), 0.15f); ;
        DomeProgress.text = ((int)progress).ToString("D2");
    }
}


public enum FadeAction
{
    Out,
    In
}