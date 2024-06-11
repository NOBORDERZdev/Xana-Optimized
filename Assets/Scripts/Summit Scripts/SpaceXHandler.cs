using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SpaceXHandler : MonoBehaviour
{
    public GameObject planetOptions;
    public VideoPlayer videoPlayer;
    public string[] planetNames;

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

    void StartVideoPlayer(VideoClip videoClip,Vector3 _returnPlayerPos)
    {
        returnPlayerPos = _returnPlayerPos;
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip=videoClip; 
        videoPlayer.Play();
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
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
        LoadingHandler.Instance.FadeIn();
        Debug.LogError("fade in"+System.DateTime.Now);
        string sceneName = planetNames[x];
        summitSceneLoading.LoadingNewScene(sceneName,returnPlayerPos);
        Destroy(videoPlayer.clip);
        DisableVideoPlayer();
        DisablePlanetOptionScreen();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        Debug.LogError("fade out" + System.DateTime.Now);
        //LoadingHandler.Instance.FadeOut();
    }

}
