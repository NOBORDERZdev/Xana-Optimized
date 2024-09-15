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

    public bool spaceXactivated;

    private bool _WaitForRestart;
    private Vector3 _ReturnPlayerPos;

    private void OnEnable()
    {
        BuilderEventManager.spaceXActivated += StartVideoPlayer;
        BuilderEventManager.spaceXDeactivated += OnspaceXDeactivated;
    }
    private void OnDisable()
    {
        BuilderEventManager.spaceXActivated -= StartVideoPlayer;
        BuilderEventManager.spaceXDeactivated -= OnspaceXDeactivated;
    }

    async void StartVideoPlayer(VideoClip VideoClip, Vector3 ReturnPlayerPos)
    {
        spaceXactivated = true;
        if (_WaitForRestart)
            return;
        await ShowCounter();
        if (!spaceXactivated)
        {
            return;
        }
        _ReturnPlayerPos = ReturnPlayerPos;
        VideoPlayer.gameObject.SetActive(true);
        VideoPlayer.targetTexture.Release();
        VideoPlayer.clip = VideoClip;
        VideoPlayer.Play();
        VideoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    async Task ShowCounter()
    {
        LaunchCounter.GetComponent<Animator>().enabled = true;
        _WaitForRestart = true;
        int x = 10;
        LaunchCounter.gameObject.SetActive(true);
        while (x > 0)
        {
            if (!spaceXactivated)
            {
                break;
            }
            LaunchCounter.text = x.ToString();
            await Task.Delay(1000);
            x--;
        }
        LaunchCounter.GetComponent<Animator>().enabled = false;
        LaunchCounter.gameObject.SetActive(false);
        await Task.Delay(1000);
        //launchCountingAudioSource.clip=audioClip;
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

    public void OnspaceXDeactivated()
    {
        _WaitForRestart = spaceXactivated = false;
        VideoPlayer.gameObject.SetActive(false);
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
        Destroy(VideoPlayer.clip);
        DisableVideoPlayer();
        DisablePlanetOptionScreen();
        _WaitForRestart = false;
        // SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    //private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    //{
    //    StartCoroutine(LoadingHandler.Instance.FadeOut());
    //}

}
