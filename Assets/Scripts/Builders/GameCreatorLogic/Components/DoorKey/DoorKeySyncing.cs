using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class DoorKeySyncing : MonoBehaviourPun
{
    [SerializeField] GameObject keyImage;
    [SerializeField] GameObject wrongKey;
    public TextMeshPro keyCounter;
    GameObject playerObj;

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        StartCoroutine(SyncingCoroutin());
    }

    private IEnumerator SyncingCoroutin()
    {
        yield return new WaitForSeconds(0.5f);
        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            yield return new WaitForSeconds(0.5f);
            transform.SetParent(playerObj.GetComponent<ArrowManager>().nameCanvas.transform);
            transform.localPosition = Vector3.up * 18.5f;
            transform.eulerAngles = Vector3.zero;
            keyImage.SetActive(true);
            InvokeRepeating(nameof(KeyCounter), 0.5f, 1f);
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView photonView in photonViews)
        {
            if (photonView.Owner == player && photonView.GetComponent<AvatarController>())
            {
                return photonView.gameObject;
            }
        }
        return null;
    }

    void KeyCounter()
    {
        string keyCount = photonView.Owner.CustomProperties["doorKeyCount"].ToString();
        keyCounter.text = "x" + keyCount;
    }
}