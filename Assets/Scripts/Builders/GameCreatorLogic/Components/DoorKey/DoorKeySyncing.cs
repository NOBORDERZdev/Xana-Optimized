using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using Photon.Pun.Demo.PunBasics;

public class DoorKeySyncing : MonoBehaviourPun
{
    [SerializeField] GameObject keyImage;
    [SerializeField] GameObject wrongKey;
    public TMP_Text keyCounter;
    GameObject playerObj;

    private void OnEnable()
    {
        if (!GamificationComponentData.instance.withMultiplayer || photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
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
            transform.localEulerAngles = new Vector3(180, 0, -45);
            keyImage.SetActive(true);
            StartCoroutine(KeyCounterCO());
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in Launcher.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                return playerObject;
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
            if (playerObj != null)
                transform.localRotation = playerObj.GetComponent<ArrowManager>().PhotonUserName.transform.localRotation;
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