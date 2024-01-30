using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using RFM.Managers;
using System;
using System.Collections;
using UnityEngine;

public class RFMCharacter : MonoBehaviour
{
    public PhotonView photonView;
    public PhotonVoiceView voiceView;
    //public RFMPlayerClass RFMPlayer;
    public bool isHunter;
    public GameObject rearViewCamera;
    public bool isMinimized;
    //public static Action gameStartAction;


    private void OnEnable()
    {
        RFM.EventsManager.onTakePositionTimeStart += OnTakePositionStart;
        RFM.EventsManager.onGameStart += GameStart;
        //if (photonView.IsMine)
        //RFM.EventsManager.OnShowRearViewMirror(true);
    }

    private void OnDisable()
    {
        RFM.EventsManager.onTakePositionTimeStart -= OnTakePositionStart;
        RFM.EventsManager.onGameStart -= GameStart;
        //if (photonView.IsMine)
        //RFM.EventsManager.OnShowRearViewMirror(false);
    }

    void Start()
    {
        voiceView.SpeakerInUse.GetComponent<AudioSource>().spatialBlend = 0;
        if (!photonView.IsMine)
        {
            Destroy(rearViewCamera);
        }
    }

    [PunRPC]
    public void TimerRPC(float time)
    {
        Debug.Log($"RFM {photonView.Owner.NickName} + TimerRPC: {time}");
        RFMManager.Instance.timer._elapsedSeconds = time;
    }

    private void OnTakePositionStart()
    {
        isHunter = bool.Parse(photonView.Owner.CustomProperties["isHunter"].ToString());

        if (isHunter)
        {
            GetComponent<RFM.Character.PlayerRunner>().enabled = false;
            GetComponent<RFM.Character.PlayerHunter>().enabled = true;
        }
        else
        {
            GetComponent<RFM.Character.PlayerHunter>().enabled = false;
            GetComponent<RFM.Character.PlayerRunner>().enabled = true;
        }
    }




    public void GameStart()
    {
        Debug.Log($"RFM {photonView.Owner.NickName} + player is hunter: {photonView.Owner.CustomProperties["isHunter"]}");
        //isHunter = bool.Parse(photonView.Owner.CustomProperties["isHunter"].ToString());

        if (RFMManager.Instance.isPlayerHunter)
        {
            voiceView.SpeakerInUse.gameObject.SetActive(isHunter);
        }
        else
        {
            voiceView.SpeakerInUse.gameObject.SetActive(!isHunter);
        }
    }
    public Coroutine CheckConditionToCloseConnectionCoroutine;
    [PunRPC]
    public void MinimizeRPC(int viewID)
    {
        Debug.LogError("MinimizedRPC called: " + viewID + "  " + photonView.ViewID);
        if (photonView.ViewID == viewID)
        {
            Debug.LogError("MinimizedRPC viewid matched" + viewID);
            isMinimized = true;
            if (CheckConditionToCloseConnectionCoroutine != null)
            {
                StopCoroutine(CheckConditionToCloseConnectionCoroutine);
            }
            CheckConditionToCloseConnectionCoroutine = StartCoroutine(CheckConditionToCloseConnection(viewID, 5));
        }
    }
    public IEnumerator CheckConditionToCloseConnection(int PVid, int timeout)
    {
        while (timeout > 0)
        {
            Debug.LogError("CheckConditionToCloseConnection timeout: " + timeout + photonView.Owner.NickName);
            yield return new WaitForSeconds(1);
            timeout--;
            if (photonView.ViewID == PVid && !isMinimized)
            {
                StopCoroutine(CheckConditionToCloseConnectionCoroutine);
            }
        }
        if (photonView.ViewID == PVid && isMinimized)
        {
            Player p = photonView.Owner;
            Destroy(transform.parent.gameObject);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("before CheckConditionToCloseConnection CloseConnection: " + p.NickName);
                PhotonNetwork.CloseConnection(p);
                Debug.LogError("after CheckConditionToCloseConnection CloseConnection: " + p.NickName);
            }
        }
    }

    [PunRPC]
    public void MaximizeRPC(int viewID)
    {
        Debug.LogError("MinimizedRPC called: " + viewID + "  " + photonView.ViewID);
        if (photonView.ViewID == viewID)
        {
            isMinimized = false;
            Debug.LogError("MinimizedRPC viewid matched" + viewID);
            if (CheckConditionToCloseConnectionCoroutine != null)
            {
                StopCoroutine(CheckConditionToCloseConnectionCoroutine);
            }
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (isMinimized)
            {
                if (photonView.IsMine)
                {
                    isMinimized = false;
                    /*if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
                    {
                        PhotonNetwork.Reconnect();
                    }*/
                    photonView.RPC(nameof(MaximizeRPC), RpcTarget.AllBuffered, photonView.ViewID);
                }
            }
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isMinimized = pause;
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(MinimizeRPC), RpcTarget.AllBuffered, photonView.ViewID);
                //PhotonNetwork.Disconnect();
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
    }
}
