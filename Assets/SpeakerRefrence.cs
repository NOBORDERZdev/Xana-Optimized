using Metaverse;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;

public class SpeakerRefrence : MonoBehaviour
{
   
    public AudioSource RangeVolSpeaker;

    public void MyspeakerSync2D()                  //Added by Ali Hamza
    {
        GetComponent<PhotonView>().RPC(nameof(SyncSpeaker2D), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void SyncSpeaker2D(int viewId)                       //Added by Ali Hamza
    {
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            if (Launcher.instance.playerobjects[i] != null)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().RangeVolSpeaker.spatialBlend = 0;
                }
            }
        }
    }
    public void MyspeakerSync3D()                  //Added by Ali Hamza
    {
        GetComponent<PhotonView>().RPC(nameof(SyncSpeaker3D), RpcTarget.AllBuffered, GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void SyncSpeaker3D(int viewId)                       //Added by Ali Hamza
    {
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            if (Launcher.instance.playerobjects[i] != null)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().RangeVolSpeaker.spatialBlend = 1;
                }
            }
        }
    }
}
