using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;

    [Header("Loading UI Elements")]
    public GameObject loadingPanel;

    public Image loadingSlider;
    public Text loadingText;
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

    [Header("fader For Villa")]
    public Image fader;

    [Header("Loader For Event")]
    public GameObject EventLoaderCanvas;


    [Header("JJ WORLD TELEPORT")]
    public CanvasGroup teleportFeader;
    public GameObject teleportFeaderLandscape, teleportFeaderPotraite;

    [Header("Store Loading")]
    public GameObject storeLoadingScreen;

    public ManualRoomController manualRoomController;
    public StreamingLoadingText streamingLoading;

    public float currentValue = 0;
    private float timer = 0;
    public bool isLoadingComplete = false;
    public float randCurrentValue = 0f;
    private float sliderFinalValue = 0;
    private float sliderCompleteValue = 0f;

    public GameObject SearchLoadingCanvas;
    private CanvasGroup canvasGroup;
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
//#if UNITY_EDITOR
//        Debug.unityLogger.logEnabled = true;
//#else
//                        Debug.unityLogger.filterLogType = LogType.Error;
//#endif
    }

    private void Start()
    {
        sliderFinalValue = Random.Range(80f, 95f);
        sliderCompleteValue = Random.Range(96f, 99f);
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
        loadingText.text = message;
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
        loadingPercentageText.text = ((int)(value * 100f)).ToString() + "%";

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
        //Debug.LogError("Show Loading Method");
        //Debug.LogError("TeleportFeader: " + teleportFeader.gameObject.activeInHierarchy + " ~~~~~~~  Activated Loading ~~~~~~~ ");
        if (teleportFeader.gameObject.activeInHierarchy) // XanaConstants.xanaConstants.JjWorldSceneChange
        {
            loadingPanel.SetActive(false);
            //Debug.LogError("Off Loading");
            return;
        }
        ResetLoadingValues();
        //Debug.LogError(Screen.orientation + " ~~~~~~~  Activated Loading ~~~~~~~ " + oriantation);
        //bool isFedderActive = false;
        //if (!XanaConstants.xanaConstants.isFromXanaLobby)
        //{
        //isFedderActive = true;
        if (XanaConstants.xanaConstants.isBackFromWorld)
        {
            if (ChangeOrientation_waqas._instance != null && ChangeOrientation_waqas._instance.isPotrait)
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

        //}

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
        if (!XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            //Debug.LogError("On Loading");
            loadingPanel.SetActive(true);
        }
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.2f).SetDelay(0f);


        if (gameplayLoadingUIRefreshCo != null)//rik for refresh screen on every 5-7 second.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
        }
        isScreenRefresh = true;
        gameplayLoadingUIRefreshCo = StartCoroutine(IEGameplayLoadingScreenUIRefresh());

        if (XanaConstants.xanaConstants.needToClearMemory)
            AddressableDownloader.Instance.MemoryManager.RemoveAllAddressables();
        else
            XanaConstants.xanaConstants.needToClearMemory = true;
    }

    public void ResetLoadingValues()
    {
        //if (LoadFromFile.instance)
        //{
        //    LoadFromFile.instance.isEnvLoaded = false;
        //}
        currentValue = 0;
        isLoadingComplete = false;
        timer = 0;
        loadingSlider.fillAmount = 0f;
        loadingPercentageText.text = "0%".ToString();
        JJLoadingSlider.fillAmount = 0f;
        JJLoadingPercentageText.text = "0%".ToString();
    }

    public void HideLoading()
    {
        //Debug.LogError("TeleportFeader: " + teleportFeader.gameObject.activeInHierarchy + "  isFromXanaLobby: " +  XanaConstants.xanaConstants.isFromXanaLobby +  " ~~~~~~~  Deactivated Loading ~~~~~~~ ");

        if (isFirstTime || teleportFeader.gameObject.activeInHierarchy) //XanaConstants.xanaConstants.JjWorldSceneChange
        {
            isFirstTime = false;
            XanaConstants.xanaConstants.isBackFromWorld = false;
            return;
        }

        if (!loadingPanel.activeInHierarchy)
            return;

        if (!XanaConstants.xanaConstants.isFromXanaLobby && XanaConstants.xanaConstants.isBackFromWorld)
        {
            Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
            blackScreen.DOKill();
            blackScreen.DOFade(1, 0.2f).OnComplete(delegate
            {
                if (XanaConstants.xanaConstants.isBackFromWorld)
                {
                    //Debug.LogError(" ~~~~~~~  BackFromWOrld: Portrait  ~~~~~~~ ");
                    //Debug.LogError("8 ~~~~~~~~~~~~~~~~ Portrait");
                    Screen.orientation = ScreenOrientation.Portrait;
                }
                else
                {
                    //Debug.LogError(" ~~~~~~~  Simple: LandscapeLeft  ~~~~~~~ ");
                    //Debug.LogError("9 ~~~~~~~~~~~~~~~~ LandscapeLeft");
                    //Screen.orientation = ScreenOrientation.LandscapeLeft;
                }

                XanaConstants.xanaConstants.isBackFromWorld = false;


                //if (ChangeOrientation_waqas._instance != null && ChangeOrientation_waqas._instance.isPotrait && !XanaConstants.xanaConstants.JjWorldSceneChange)
                //{
                //    // Debug.LogError("~~~~~ Waqas_ LoadingHandler ~~~~~~~~~~~");
                //    Screen.orientation = ScreenOrientation.Portrait;
                //}
                //else
                //{
                //    Screen.orientation = oriantation;
                //}
                //Debug.LogError(" ~~~~~~~  Oriantation Change Called ~~~~~~~ ");
            });
        }
        CustomHideLoading();
    }
    void CustomHideLoading()
    {
        //if (needWait)
        //{
        //    Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        //    blackScreen.DOFade(0, 0.5f).SetDelay(0.5f);
        //}
        UpdateLoadingStatusText("");
        loadingPanel.SetActive(false);
        //Debug.LogError("Off Loading");
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.5f).SetDelay(0.5f);

        if (ReferrencesForDynamicMuseum.instance != null)
            ReferrencesForDynamicMuseum.instance.workingCanvas.SetActive(true);

        if (gameplayLoadingUIRefreshCo != null)//rik stop refreshing screen coroutine.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
        }
    }

    //private IEnumerator ApplyDelay()
    //{
    //    if (XanaConstants.xanaConstants.isBackFromWorld)
    //    {
    //        HideLoadingManually();
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(2f);
    //        LoadFromFile.instance.SetPlayerPos();
    //        yield return new WaitForSeconds(0.1f);
    //        HideLoadingManually();
    //    }
    //}
    //private void HideLoadingManually()
    //{
    //    UpdateLoadingStatusText("");      
    //    loadingPanel.SetActive(false);
    //    //Debug.LogError("Off Loading");
    //    Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
    //    blackScreen.DOFade(0, 0.5f).SetDelay(0.5f);
    //}


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
        XanaConstants.xanaConstants.isBackFromWorld = false;
    }

    private IEnumerator Check_Orientation(ScreenOrientation oriantation)
    {
    CheckAgain:
        //  Debug.LogError(Screen.orientation + " ~~~~~~~ Oriantation Checking ~~~~~~~ " + oriantation);
        yield return new WaitForSeconds(.2f);
        if (Screen.orientation == oriantation || XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            //if(!XanaConstants.xanaConstants.isBackFromWorld)
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
        if (canvasGroup.alpha == 0)
            canvasGroup.alpha = 1;
        characterLoading.SetActive(true);
        yield return new WaitForSeconds(delay);
        characterLoading.SetActive(false);
    }


    public IEnumerator FadeIn()
    {
        fader.gameObject.SetActive(true);
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
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


    public void LoadSceneByIndex(string sceneName, bool isBuilder = false)
    {
        //UpdateLoadingSlider(.2f);
        if (XanaConstants.xanaConstants.JjWorldSceneChange)
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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    }

    public IEnumerator IncrementSliderValue(float speed, bool loadMainScene = false)
    {
        while (currentValue < sliderCompleteValue)
        {
            timer += Time.deltaTime;
            currentValue = Mathf.Lerp(0, sliderFinalValue, timer / speed);
            if ((XanaConstants.xanaConstants.isBackFromPMY || XanaConstants.xanaConstants.isFromXanaLobby
                || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld)) &&
                teleportFeader.gameObject.activeInHierarchy)
            {
                JJLoadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                JJLoadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
            }
            else
            {
                loadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                loadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
            }


            if (LoadFromFile.instance && !loadMainScene)
            {
                if (LoadFromFile.instance.isEnvLoaded)
                {
                    isLoadingComplete = true;
                }
                else if (currentValue > 75f)
                {
                    isLoadingComplete = true;
                }
            }
            else if (loadMainScene)
            {
                if (currentValue > 35f)
                {
                    isLoadingComplete = true;
                }
            }
            if (isLoadingComplete)
            {
                currentValue = sliderCompleteValue;
                if ((XanaConstants.xanaConstants.isFromPMYLobby || XanaConstants.xanaConstants.isFromXanaLobby
                    || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld)) &&
                    teleportFeader.gameObject.activeInHierarchy)
                {
                    JJLoadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                    JJLoadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
                    // yield return new WaitForSeconds(1f);
                    //HideLoading(ScreenOrientation.Portrait);
                }
                else
                {
                    loadingSlider.DOFillAmount((currentValue / 100), 0.15f);
                    loadingPercentageText.text = ((int)(currentValue)).ToString() + "%";
                    //yield return new WaitForSeconds(1f);
                    //HideLoading(ScreenOrientation.Portrait);
                }
            }
            yield return null;
        }
    }

    public IEnumerator TeleportFader(FadeAction action)
    {
        if (action.Equals(FadeAction.Out) && XanaConstants.xanaConstants.isFromPMYLobby)
        {
            JJLoadingSlider.fillAmount = 1f;
            JJLoadingPercentageText.text = "100%".ToString();
        }
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
                if (XanaConstants.xanaConstants != null)
                {
                    teleportFeaderLandscape.SetActive(!XanaConstants.xanaConstants.orientationchanged);
                    teleportFeaderPotraite.SetActive(XanaConstants.xanaConstants.orientationchanged);
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
                //Debug.LogError("On Teleport Fader");
                teleportFeader.gameObject.SetActive(true);
                teleportFeader.DOFade(1, 0.5f);
                yield return new WaitForSecondsRealtime(0.1f);
                loadingPanel.SetActive(false);
                //Debug.LogError("loading false");
                break;
            default:
                break;
        }
    }



}


public enum FadeAction
{
    Out,
    In
}