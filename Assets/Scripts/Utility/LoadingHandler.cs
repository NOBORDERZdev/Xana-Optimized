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

    [Header("Loading BG Elements")]
    public Image loadingBgImage;
    public Image loadingBgImageAlter;
    public Sprite[] loadingSprites;

    public float fadeTimer;


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
    }

    private void Start()
    {

        StartCoroutine(StartBGChange());

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
                Debug.unityLogger.logEnabled = false;
#endif
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

    public void ShowLoading()
    {
        if (/*XanaConstants.xanaConstants.JjWorldSceneChange*/teleportFeader.gameObject.activeInHierarchy)
        {
            return;
        }
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        loadingPanel.SetActive(true);

        if (gameplayLoadingUIRefreshCo != null)//rik for refresh screen on every 5-7 second.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
        }
        isScreenRefresh = true;
        gameplayLoadingUIRefreshCo = StartCoroutine(IEGameplayLoadingScreenUIRefresh());
    }

    public void HideLoading()
    {
        if (ReferrencesForDynamicMuseum.instance != null)
            ReferrencesForDynamicMuseum.instance.workingCanvas.SetActive(true);
        loadingPanel.SetActive(false);

        if (ChangeOrientation_waqas._instance != null && ChangeOrientation_waqas._instance.isPotrait && !XanaConstants.xanaConstants.JjWorldSceneChange)
            Screen.orientation = ScreenOrientation.Portrait;

        if (gameplayLoadingUIRefreshCo != null)//rik stop refreshing screen coroutine.......
        {
            StopCoroutine(gameplayLoadingUIRefreshCo);
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
        //Debug.LogError("RefreshLoading screen");
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


    public void LoadSceneByIndex(string sceneName)
    {
        UpdateLoadingSlider(.2f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    }

    public IEnumerator TeleportFader(FadeAction action) {
       // teleportFeader.gameObject.SetActive(true);
        switch (action)
        {
            case FadeAction.Out:
                teleportFeader.DOFade(0,0.5f).OnComplete(()=>{
                    teleportFeader.gameObject.SetActive(false);
                    teleportFeaderLandscape.SetActive(false);
                    teleportFeaderPotraite.SetActive(false);
                });
                break;
            case FadeAction.In:
                if (XanaConstants.xanaConstants!=null)
                {
                    teleportFeaderLandscape.SetActive(!XanaConstants.xanaConstants.orientationchanged);
                    teleportFeaderPotraite.SetActive(XanaConstants.xanaConstants.orientationchanged);
                }
                else
                {
                    teleportFeaderLandscape.SetActive(true);
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