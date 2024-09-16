using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SpaceXHandler : MonoBehaviour
{
    public GameObject PlanetOptions;
    public VideoPlayer VideoPlayer;
    public TMPro.TextMeshProUGUI LaunchCounter;
    public string[] PlanetWorldId_Testnet;
    public string[] PlanetWorldId_Mainnet;
    public XANASummitSceneLoading SummitSceneLoading;
    //public AudioSource launchCountingAudioSource;
    //public AudioClip countingAudioClip;


    private bool _WaitForRestart;
    private Vector3 _ReturnPlayerPos;
    private CancellationTokenSource _cancellationTokenSource; // Add a cancellation token source

    private void OnEnable()
    {
        BuilderEventManager.spaceXActivated += StartVideoPlayer;
        BuilderEventManager.spaceXDeactivated += SpaceXDeactivated;
    }
    private void OnDisable()
    {
        BuilderEventManager.spaceXActivated -= StartVideoPlayer;
        BuilderEventManager.spaceXDeactivated -= SpaceXDeactivated;
    }

    async void StartVideoPlayer(VideoClip VideoClip, Vector3 ReturnPlayerPos)
    {
        if (_WaitForRestart)
            return;

        _cancellationTokenSource = new CancellationTokenSource(); // Initialize the cancellation token source

        try
        {
            await ShowCounter(_cancellationTokenSource.Token); // Pass the cancellation token
        }
        catch (TaskCanceledException)
        {
            //Debug.Log("ShowCounter was cancelled.");
            return;
        }

        _ReturnPlayerPos = ReturnPlayerPos;
        VideoPlayer.gameObject.SetActive(true);
        VideoPlayer.targetTexture.Release();
        VideoPlayer.clip = VideoClip;
        VideoPlayer.Play();
        VideoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    async Task ShowCounter(CancellationToken token)
    {
        LaunchCounter.GetComponent<Animator>().enabled = true;
        _WaitForRestart = true;
        int x = 10;
        LaunchCounter.gameObject.SetActive(true);
        while (x > 0)
        {
            if (token.IsCancellationRequested)
            {
                break; // Exit the loop if cancellation is requested
            }

            LaunchCounter.text = x.ToString();
            await Task.Delay(1000, token); // Pass the cancellation token to Task.Delay
            x--;
        }
        LaunchCounter.GetComponent<Animator>().enabled = false;
        LaunchCounter.gameObject.SetActive(false);
        //launchCountingAudioSource.clip=audioClip;
        await Task.Delay(1000, token); // Delay before proceeding, also cancellable
    }


    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        PlanetOptions.SetActive(true);
    }

    void DisableVideoPlayer()
    {
        VideoPlayer.gameObject.SetActive(false);
    }

    void DisablePlanetOptionScreen()
    {
        PlanetOptions.SetActive(false);
    }

    public void LoadPlanetScene(int x)
    {
        string SceneId;
        //StartCoroutine(LoadingHandler.Instance.FadeIn());
        //LoadingHandler.Instance.ShowVideoLoading();
        if (APIBasepointManager.instance.IsXanaLive)
            SceneId = PlanetWorldId_Mainnet[x];
        else
            SceneId = PlanetWorldId_Testnet[x];

        ConstantsHolder.isFromXANASummit = true;
        ReferencesForGamePlay.instance.ChangeExitBtnImage(false);
        SummitSceneLoading.LoadingSceneByIDOrName(SceneId, _ReturnPlayerPos);
        DisableObjects();
        // SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    //private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    //{
    //    StartCoroutine(LoadingHandler.Instance.FadeOut());
    //}


    // Method to cancel the async task
    void SpaceXDeactivated()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel(); // Cancel the async task
            DisableObjects();
        }
    }

    void DisableObjects()
    {
        Destroy(VideoPlayer.clip);
        DisableVideoPlayer();
        DisablePlanetOptionScreen();
        LaunchCounter.GetComponent<Animator>().enabled = false;
        LaunchCounter.gameObject.SetActive(false);
        _WaitForRestart = false;
    }
}
