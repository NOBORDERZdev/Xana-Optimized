using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;

public class SpeakerRefrence : MonoBehaviour
{
   
    public AudioSource RangeVolSpeaker;

    private void OnEnable()
    {
        if(ConstantsHolder.xanaConstants.EnviornmentName=="RooftopParty")
        {
            RangeVolSpeaker.maxDistance = 3;
        }

        if(!GetComponent<PhotonView>().IsMine)
            Destroy(GetComponent<AudioListener>());

    }

    //public void MyspeakerSync2D()                  //Added by Ali Hamza
    //{
    //    GetComponent<PhotonView>().RPC(nameof(SyncSpeaker2D), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID);
    //}

    //[PunRPC]
    //void SyncSpeaker2D(int viewId)                       //Added by Ali Hamza
    //{
    //    for (int i = 0; i < MutiplayerController.instance.playerobjects.Count; i++)
    //    {
    //        if (MutiplayerController.instance.playerobjects[i] != null)
    //        {
    //            if (MutiplayerController.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
    //            {
    //                MutiplayerController.instance.playerobjects[i].GetComponent<SpeakerRefrence>().RangeVolSpeaker.spatialBlend = 0;
    //            }
    //        }
    //    }
    //}
    //public void MyspeakerSync3D()                  //Added by Ali Hamza
    //{
    //    GetComponent<PhotonView>().RPC(nameof(SyncSpeaker3D), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID);
    //}

    //[PunRPC]
    //void SyncSpeaker3D(int viewId)                       //Added by Ali Hamza
    //{
    //    for (int i = 0; i < MutiplayerController.instance.playerobjects.Count; i++)
    //    {
    //        if (MutiplayerController.instance.playerobjects[i] != null)
    //        {
    //            if (MutiplayerController.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
    //            {
    //                MutiplayerController.instance.playerobjects[i].GetComponent<SpeakerRefrence>().RangeVolSpeaker.spatialBlend = 1;
    //            }
    //        }
    //    }
    //}
}
