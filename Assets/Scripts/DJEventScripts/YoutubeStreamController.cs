using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using LightShaft.Scripts;

public class YoutubeStreamController : MonoBehaviour
{
    [SerializeField]
    public GameObject LiveStreamPlayer;
    [SerializeField]
    private VideoPlayer NormalPlayer;
    [SerializeField]
    private YoutubeAPIHandler APIHandler;
    private YoutubeStreamController Instance;
    public AudioSource videoPlayerAudioSource;
    public AudioSource mediaPlayerAudioSource;

    private string PrevURL;
    private bool IsOldURL = true;
    public static Action playPrercordedVideo;
    public StreamYoutubeVideo streamYoutubeVideo;
    // Start is called before the first frame update
    private void OnEnable()
    {
        PrevURL = "xyz";
        StartCoroutine(SetStreamContinous());
        playPrercordedVideo += PlayPrerecordedVideo;
    }

    private void OnDisable()
    {
        playPrercordedVideo -= PlayPrerecordedVideo;
    }

    public IEnumerator SetStreamContinous()
    {
        while (true)
        {
            StartCoroutine(SetStream());
            yield return new WaitForSeconds(5.0f);
        }
    }

    public void PlayPrerecordedVideo()
    {
        //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();
        NormalPlayer.url = APIHandler.Data.URL;
        NormalPlayer.Play();
    }

    private void Awake()
    {

        Instance = this;
        if (SoundController.Instance)
        {
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().player.GetComponent<YoutubePlayer>().playInAVPRO)
            //{
            //    SoundController.Instance.videoPlayerSource = mediaPlayerAudioSource;
            //    SoundSettings.soundManagerSettings.videoSource = mediaPlayerAudioSource;
            //}
            //else
            //{
                //SoundController.Instance.videoPlayerSource = videoPlayerAudioSource;
                //SoundSettings.soundManagerSettings.videoSource = videoPlayerAudioSource;
            //}
            
            //SoundController.Instance.livePlayerSource = LiveStreamPlayer.GetComponent<MediaPlayer>();
            SoundSettings.soundManagerSettings.setNewSliderValues();
        }
    }

    private void Start()
    {
        if (this.GetComponent<StreamYoutubeVideo>() != null)
        {
            streamYoutubeVideo = this.GetComponent<StreamYoutubeVideo>();
        }
        //if (videoPlayerAudioSource)
        //    videoPlayerAudioSource.gameObject.GetComponent<VideoPlayer>().targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
        //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
        //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
        //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
//#if UNITY_EDITOR && !UNITY_IOS
//        if (!WorldItemView.m_EnvName.Contains("BreakingDown Arena") && !WorldItemView.m_EnvName.Contains("XANA FESTIVAL STAGE in Dubai.") && !WorldItemView.m_EnvName.Contains("DJ Event"))
//        {
//            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
//            scale.y *= -1;
//            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
//        }
//#endif
//#if UNITY_IOS
//        if (WorldItemView.m_EnvName.Contains("DJ Event"))
//        {
//            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
//            scale.y *= -1;
//            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
//        }
//        if (WorldItemView.m_EnvName.Contains("Xana Festival"))
//        {
//            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
//            scale.y *= -1;
//            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
//        }
//        if (WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
//        {
//            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
//            scale.y *= -1;
//            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
//        }
//        if (WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("XANA Lobby"))
//        {
//            Vector3 scale = NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale;
//            scale.y *= -1;
//            NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.transform.localScale = scale;
//        }
//#endif
    }

    private IEnumerator SetStream()
    {
        yield return StartCoroutine(APIHandler.GetStream());

        while (APIHandler.Data == null)
        {
            yield return null;
        }

        if (!PrevURL.Equals(APIHandler.Data.URL) && APIHandler.Data.isPlaying)
        {
            PrevURL = APIHandler.Data.URL;
            SetUpStream();
        }
        else if (!APIHandler.Data.isPlaying)
        {
            LiveStreamPlayer.SetActive(false);
            NormalPlayer.gameObject.SetActive(true);
            //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(57, 57, 57, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);

            //player.OnInternetDisconnect();
            NormalPlayer.Stop();
        }
        else if (APIHandler.Data.isPlaying && APIHandler.Data.IsLive && !LiveStreamPlayer.GetComponent<MediaPlayer>().Info.HasVideo())
        {
            PrevURL = APIHandler.Data.URL;
            SetUpStream();
        }
        else if (APIHandler.Data.isPlaying && APIHandler.Data.IsLive && !LiveStreamPlayer.activeInHierarchy)
        {
            SetUpStream();
        }
        else if (APIHandler.Data.isPlaying && !APIHandler.Data.IsLive && !NormalPlayer.gameObject.activeInHierarchy)
        {
            SetUpStream();
        }
        else
        {
            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().videoPlayer.targetMaterialRenderer.material.color = new Color32(255, 255, 255, 255);
            //if (NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer != null)
            //    NormalPlayer.GetComponent<YoutubeSimplified>().mPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(255, 255, 255, 255);
        }
    }

    private void SetUpStream()
    {
        if (APIHandler.Data.IsLive && APIHandler.Data.isPlaying)
        {
            Debug.Log("Hardik changes check");
            if (GetComponent<AvProLiveVideoSoundEnabler>())
            {
                GetComponent<AvProLiveVideoSoundEnabler>().EnableVideoScreen(true);
            }
            streamYoutubeVideo.mediaPlayer.enabled = true;
            LiveStreamPlayer.SetActive(true);
            NormalPlayer.gameObject.SetActive(false);
            if (GetComponent<AvProDirectionalSound>())
            {
                GetComponent<AvProDirectionalSound>().ActivateDirectionalSoundIfNotYet();
            }


            //YoutubePlayerLivestream player = LiveStreamPlayer.GetComponent<YoutubePlayerLivestream>();
            //if (player)
            //{
            //    player.GetLivestreamUrl(APIHandler.Data.URL);
            //}
            streamYoutubeVideo.StreamYtVideo(APIHandler.Data.URL, APIHandler.Data.IsLive);
            if (!WorldItemView.m_EnvName.Contains("Xana Festival") || !WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
            {
                NormalPlayer.gameObject.SetActive(false);
            }

        }
        else
        {

            //LiveStreamPlayer.GetComponent<ApplyToMesh>().MeshRenderer.sharedMaterial.color = new Color32(57, 57, 57, 255);

            LiveStreamPlayer.SetActive(false);
            NormalPlayer.gameObject.SetActive(true);

            //YoutubeSimplified player = NormalPlayer.GetComponent<YoutubeSimplified>();

            if (NormalPlayer && APIHandler.Data.isPlaying)
            {
                //NormalPlayer.url = APIHandler.Data.URL;
                //NormalPlayer.Play();
                streamYoutubeVideo.StreamYtVideo(APIHandler.Data.URL, APIHandler.Data.IsLive);
            }
            else if (APIHandler.Data.isPlaying == false)
            {
                //player.OnInternetDisconnect();
                NormalPlayer.Stop();
            }

        }
    }


}
