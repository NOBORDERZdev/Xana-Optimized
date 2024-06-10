using UnityEngine;
using Photon.Pun;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TriggerSpaceX : MonoBehaviour
{

    private VideoClip vClip;
    private void OnEnable()
    {
        AsyncOperationHandle AsyncOp=Addressables.LoadAssetAsync<VideoClip>("SpaceX");
        AsyncOp.Completed += AsyncOp_Completed;
    }

    private void AsyncOp_Completed(AsyncOperationHandle obj)
    {
        vClip = (VideoClip)obj.Result;
        throw new System.NotImplementedException();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.spaceXActivated?.Invoke(vClip);
        }
    }
}
