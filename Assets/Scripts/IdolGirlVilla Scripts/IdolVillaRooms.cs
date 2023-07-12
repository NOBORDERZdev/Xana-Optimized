using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class IdolVillaRooms : MonoBehaviour
{

    public static IdolVillaRooms instance;
    public GameObject outSideSpaceRoot;
    public GameObject[] villaRoomsRoot;
    public GameObject[] villaRooms;
    public GameObject outSideSpace;

    private GameObject m_Player;
    // Start is called before the first frame update

    private void Awake()
    { 
        instance = this;
    }


    public void ResetVilla()
    {
        outSideSpaceRoot.SetActive(true);
        foreach(GameObject g in villaRoomsRoot)
        {
            g.SetActive(false);
        }
    }



    //public void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    Debug.LogError("jay shree ram");
    //    if (m_Player == null)
    //        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponentInChildren<PhotonView>().gameObject;

    //    if (m_Player.GetComponent<PhotonView>())
    //        m_Player.GetComponent<PhotonView>().RPC("ChangePlayerParentOnStart", RpcTarget.All);
    //}

    //public void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //   // throw new System.NotImplementedException();
    //}

    //public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    //{
    //    //throw new System.NotImplementedException();
    //}

    //public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //   // throw new System.NotImplementedException();
    //}

    //public void OnMasterClientSwitched(Player newMasterClient)
    //{
    //   // throw new System.NotImplementedException();
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    photonEvent.
    //}
}
