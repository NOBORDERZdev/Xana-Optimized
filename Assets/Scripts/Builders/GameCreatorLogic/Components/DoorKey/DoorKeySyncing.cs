using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using Photon.Pun.Demo.PunBasics;

public class DoorKeySyncing : MonoBehaviourPun
{
    [SerializeField] GameObject _keyCounterObj;
    [SerializeField] GameObject _keyImage;
    [SerializeField] GameObject _wrongKey;
    TMP_Text _keyCounter;
    GameObject _playerObj;

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
        _playerObj = FindPlayerusingPhotonView(photonView);
        if (_playerObj != null)
        {
            yield return new WaitForSeconds(0.5f);
            transform.SetParent(_playerObj.GetComponent<ArrowManager>().nameCanvas.transform);
            transform.localPosition = Vector3.up * 18.5f;
            transform.localEulerAngles = new Vector3(180, 0, -45);
            _keyImage.SetActive(true);
            StartCoroutine(KeyCounterCO());
        }
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
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
            if (_playerObj != null)
                transform.localRotation = _playerObj.GetComponent<ArrowManager>().PhotonUserName.transform.localRotation;
        }
    }

    StringBuilder keyCountStringBuilder = new StringBuilder();
    void KeyCounter()
    {
        keyCountStringBuilder.Clear();
        keyCountStringBuilder.Append("x");
        keyCountStringBuilder.Append(photonView.Owner.CustomProperties["doorKeyCount"]);
        if (_keyCounter == null)
        {
            _keyCounter = _keyCounterObj.AddComponent<TextMeshProUGUI>();
            _keyCounter.fontSize = 120;
            _keyCounter.fontStyle = FontStyles.Bold;
            _keyCounter.verticalAlignment = VerticalAlignmentOptions.Middle;
        }
        _keyCounter.text = keyCountStringBuilder.ToString();
    }
}