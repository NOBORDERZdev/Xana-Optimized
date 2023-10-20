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

    public ManualRoomController manualRoomController;
    public StreamingLoadingText streamingLoading;

    public float currentValue = 0;
    private float timer = 0;
    public bool isLoadingComplete = false;
    public float randCurrentValue = 0f;
    private float sliderFinalValue = 0;

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

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
                Debug.unityLogger.filterLogType = LogType.Error;
#endif
    }

    private void Start()
    {
        sliderFinalValue = Random.Range(80f, 95f);
        StartCoroutine(StartBGChange());
        //#if UNITY_EDITOR
        //        Debug.unityLogger.logEnabled = true;
        //#else
        //                        Debug.unityLogger.logEnabled = false;
        //#endif
    }

    //private void Update()
    //{
    //    if(percentComplete!=downloadProgressScript.downloadprogressOutput)
    //}
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

        /*if (loadingSlider.fillAmount < 0.5f)//rik for Refresh screen on every 5-7 second.......
        {
            ChangeHelpScreenUI(true);
        }
        else
        {
            ChangeHelpScreenUI(false);
        }*/
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
        if (teleportFeader.gameObject.activeInHierarchy) // XanaConstants.xanaConstants.JjWorldSceneChange
        {
            return;
        }
        ResetLoadingValues();
        //Debug.LogError(Screen.orientation + " ~~~~~~~  Activated Loading ~~~~~~~ " + oriantation);
        //bool isFedderActive = false;
        //if (!XanaConstants.xanaConstants.isFromXanaLobby)
        {
            //isFedderActive = true;
            Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
            blackScreen.DOFade(1, 0.1f).OnComplete(delegate
            {
                //Debug.LogError("7 ~~~~~~~~~~~~~~~~ LandscapeLeft");
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                //Debug.LogError(" ~~~~~~~  Oriantation Change Called ~~~~~~~ " );
            });
        }

        //StartCoroutine(CustomLoading());
        CustomLoading();
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



        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.2f).SetDelay(1f);
        loadingPanel.SetActive(true);
      

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
        if (LoadFromFile.instance)
        {
            LoadFromFile.instance.isEnvLoaded = false;
        }
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

        if (!XanaConstants.xanaConstants.isFromXanaLobby)
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
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
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

        //StartCoroutine(CustomHideLoading());
        CustomHideLoading();
    }
    void CustomHideLoading()
    {
        //if (needWait)
        //{
        //    Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        //    blackScreen.DOFade(0, 0.5f).SetDelay(0.5f);
        //}

        loadingPanel.SetActive(false);
        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
        blackScreen.DOFade(0, 0.5f).SetDelay(0.5f);


        if (ReferrencesForDynamicMuseum.instance != null)
            ReferrencesForDynamicMuseum.instance.workingCanvas.SetActive(true);
        //loadingPanel.SetActive(false);

        //if (ChangeOrientation_waqas._instance != null && ChangeOrientation_waqas._instance.isPotrait && !XanaConstants.xanaConstants.JjWorldSceneChange)
        //{
        //    // Debug.LogError("~~~~~ Waqas_ LoadingHandler ~~~~~~~~~~~");
        //    //Screen.orientation = ScreenOrientation.Portrait;
        //}

        if (gameplayLoadingUIRefreshCo != null)//rik stop refreshing screen coroutine.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
        }

        //if (XanaConstants.xanaConstants.isBackFromWorld)
        //    HideFadderAfterOriantationChanged(1.5f);
    }


    //bool orientationchanged = false;
    //    public void ShowFadderWhileOriantationChanged(ScreenOrientation oriantation)
    //    {
    //        Debug.LogError("~~~~~~~  Activated Fadder ~~~~~~~ " + oriantation);
    //        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
    //        blackScreen.DOKill();
    //#if !UNITY_EDITOR

    //           // Removing Delay Time 
    //            blackScreen.DOFade(1, 0f);
    //            Screen.orientation = oriantation;
    //            orientationchanged = false;
    //            StartCoroutine(Check_Orientation(oriantation));


    // //blackScreen.DOFade(1, 0.15f).OnComplete(delegate 
    // //       {
    // //           Screen.orientation = oriantation;
    // //           orientationchanged = false;
    // //           StartCoroutine(Check_Orientation(oriantation));
    // //            });
    //#else

    //        Screen.orientation = oriantation;
    //#endif

    //        //Invoke(nameof(HideFadderAfterOriantationChanged), 2f);
    //    }
    //    public void HideFadderAfterOriantationChanged(float delay = 0)
    //    {
    //       // Debug.LogError("~~~~~~~  Fadder Out ~~~~~~~ " );
    //        Image blackScreen = Loading_WhiteScreen.GetComponent<Image>();
    //        blackScreen.DOFade(0, 0.5f).SetDelay(delay);
    //        XanaConstants.xanaConstants.isBackFromWorld = false;
    //    }

    //private IEnumerator Check_Orientation(ScreenOrientation oriantation)
    //{
    //CheckAgain:
    //    //  Debug.LogError(Screen.orientation + " ~~~~~~~ Oriantation Checking ~~~~~~~ " + oriantation);
    //    yield return new WaitForSeconds(.2f);
    //    if (Screen.orientation == oriantation || XanaConstants.xanaConstants.JjWorldSceneChange)
    //    {
    //        //if(!XanaConstants.xanaConstants.isBackFromWorld)
    //        //    HideFadderAfterOriantationChanged();
    //    }
    //    else
    //    {
    //        Screen.orientation = oriantation;
    //        goto CheckAgain;
    //    }

    //}




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
        while (currentValue < 100)
        {
            timer += Time.deltaTime;
            currentValue = Mathf.Lerp(0, sliderFinalValue, timer / speed);
            if (XanaConstants.xanaConstants.isFromXanaLobby || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld))
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
            if (isLoadingComplete)
            {
                currentValue = 100;
                if (XanaConstants.xanaConstants.isFromXanaLobby || (JjInfoManager.Instance != null && JjInfoManager.Instance.IsJjWorld))
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

                teleportFeader.gameObject.SetActive(true);
                teleportFeader.DOFade(1, 0.5f);
                break;
            default:
                break;
        }
        yield return null;
    }



}


public enum FadeAction
{
    Out,
    In
}