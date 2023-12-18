using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DoorKeySyncing : MonoBehaviourPun
{
    [SerializeField] GameObject keyImage;
    [SerializeField] GameObject wrongKey;
    public TMP_Text keyCounter;
    GameObject playerObj;
    private List<PhotonView> allPhotonViews = new List<PhotonView>();

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        allPhotonViews.Clear();
        allPhotonViews.Add(GetComponent<PhotonView>());
        StartCoroutine(SyncingCoroutin());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
            StartCoroutine(KeyCounterCO());
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (PhotonView photonView in allPhotonViews)
        {
            if (photonView.Owner == player && photonView.GetComponent<AvatarController>())
            {
                return photonView.gameObject;
            }
        }
        return null;
    }

    IEnumerator KeyCounterCO()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            KeyCounter();
            yield return new WaitForSeconds(1f);
        }
    }

    StringBuilder keyCountStringBuilder = new StringBuilder();
    void KeyCounter()
    {
        keyCountStringBuilder.Clear();
        keyCountStringBuilder.Append("x");
        keyCountStringBuilder.Append(photonView.Owner.CustomProperties["doorKeyCount"]);
        keyCounter.text = keyCountStringBuilder.ToString();
    }
}