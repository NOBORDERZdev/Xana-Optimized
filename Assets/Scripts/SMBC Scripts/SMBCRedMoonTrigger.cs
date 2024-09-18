using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

public class SMBCRedMoonTrigger : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    private VideoClip _vClip;

    private void Start()
    {
        gameObject.SetActive(SMBCManager.Instance.CheckAllRocketPartIsCollected());
    }

    private void OnEnable()
    {
        AsyncOperationHandle AsyncOp = Addressables.LoadAssetAsync<VideoClip>("SpaceX");
        AsyncOp.Completed += AsyncOp_Completed;
    }


    private void AsyncOp_Completed(AsyncOperationHandle obj)
    {
        AddressableDownloader.bundleAsyncOperationHandle.Add(obj);
        _vClip = (VideoClip)obj.Result;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && _vClip != null)
        {
            XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
            StartVideoPlayer(_vClip);
        }
    }
    private void StartVideoPlayer(VideoClip videoClip)
    {
        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.targetTexture.Release();
        _videoPlayer.clip = videoClip;
        _videoPlayer.Play();
        _videoPlayer.loopPointReached += OnVideoEnd;
    }
    private void OnVideoEnd(VideoPlayer source)
    {
        _videoPlayer.transform.parent.gameObject.SetActive(false);
        LoadingHandler.Instance.EnterDome();
    }
}
