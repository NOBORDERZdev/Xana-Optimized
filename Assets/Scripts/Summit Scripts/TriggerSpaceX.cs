using UnityEngine;
using Photon.Pun;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TriggerSpaceX : MonoBehaviour
{

    private VideoClip vClip;
    public Transform playerPos;
    private void OnEnable()
    {
        gameObject.transform.localScale = Vector3.zero;
        AsyncOperationHandle AsyncOp = Addressables.LoadAssetAsync<VideoClip>("SpaceX");
        AsyncOp.Completed += AsyncOp_Completed;
    }


    private void AsyncOp_Completed(AsyncOperationHandle obj)
    {
        gameObject.transform.localScale = Vector3.one;
        AddressableDownloader.bundleAsyncOperationHandle.Add(obj);
        vClip = (VideoClip)obj.Result;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine && vClip!=null)
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                XANASummitSceneLoading.OnJoinSubItem?.Invoke(false);
                BuilderEventManager.spaceXActivated?.Invoke(vClip, playerPos.position);
            }
        }
    }
}
