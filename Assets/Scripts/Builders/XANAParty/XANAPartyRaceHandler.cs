using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class XANAPartyRaceHandler : MonoBehaviourPunCallbacks
{
    public bool raceStart;
    //new void OnEnable()
    //{

    //}
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.LocalPlayer.IsMasterClient)
    //    {
    //        BuilderEventManager.XANAPartyRaceStart?.Invoke();
    //    }
    //}

    private void Update()
    {
        if (raceStart)
        {
            BuilderEventManager.XANAPartyRaceStart?.Invoke();
            raceStart = false;
        }
    }


}