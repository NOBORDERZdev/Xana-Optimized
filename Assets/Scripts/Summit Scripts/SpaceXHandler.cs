using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SpaceXHandler : MonoBehaviour
{
    public GameObject planetOptions;
    public VideoPlayer videoPlayer;
    public string[] planetNames;
    private bool waitForRestart;
    //public AudioSource launchCountingAudioSource;
    //public AudioClip countingAudioClip;
    public TMPro.TextMeshProUGUI launchCounter;

    public XANASummitSceneLoading summitSceneLoading;

    private Vector3 returnPlayerPos;

    private void OnEnable()
    {
        BuilderEventManager.spaceXActivated += StartVideoPlayer;   
    }
    private void OnDisable()
    {
        BuilderEventManager.spaceXActivated -= StartVideoPlayer;
    }

    async void StartVideoPlayer(VideoClip videoClip,Vector3 _returnPlayerPos)
    {
        if (waitForRestart)
            return;
        await ShowCounter();
        returnPlayerPos = _returnPlayerPos;
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip=videoClip; 
        videoPlayer.Play();
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    async Task ShowCounter()
    {

        //AudioClip audioClip = launchCountingAudioSource.clip;
        //launchCountingAudioSource.clip = countingAudioClip;
        //launchCountingAudioSource.volume = 1;
        //launchCountingAudioSource.Play();
        //await Task.Delay(4000);
        launchCounter.GetComponent<Animator>().enabled = true;
        waitForRestart = true;
        int x = 10;
        launchCounter.gameObject.SetActive(true);
        while (x> 0)
        {
            launchCounter.text=x.ToString();
            await Task.Delay(1000);
            x--;
        }
        launchCounter.GetComponent<Animator>().enabled=false;
        await Task.Delay(1000);
        //launchCountingAudioSource.clip=audioClip;
        launchCounter.gameObject.SetActive(false);
    }


    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        planetOptions.SetActive(true);
    }

    void DisableVideoPlayer()
    {
        videoPlayer.gameObject.SetActive(false);
    }

    void DisablePlanetOptionScreen()
    {
        planetOptions.SetActive(false);
    }

    public void LoadPlanetScene(int x)
    {
        StartCoroutine(LoadingHandler.Instance.FadeIn());
        string sceneName = planetNames[x];
        summitSceneLoading.LoadingSceneByIDOrName(sceneName,returnPlayerPos);
        Destroy(videoPlayer.clip);
        DisableVideoPlayer();
        DisablePlanetOptionScreen();
        waitForRestart = false;
       // SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    //private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    //{
    //    StartCoroutine(LoadingHandler.Instance.FadeOut());
    //}

}
