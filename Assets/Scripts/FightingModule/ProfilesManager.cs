using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ProfilesManager : MonoBehaviourPunCallbacks
{
  //  int ii = 0;
    public void ProfileSelected(int i) {
        ProfileSelector._instance.currentProfile = i;
    //    ii = i;
        //Hashtable customProperties = new Hashtable();
        //customProperties["profileId"] = i; // Replace 123 with your desired integer value.

        //if (PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "profileId", i } });
        //}
        //else
        //{
        //    Debug.Log("Not Connected to photon");
        //}

        //// Set the custom properties for the local player.
        ////PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        //print("Selected");
    }

    public override void OnJoinedRoom()
    {
      //  base.OnJoinedRoom();
        Debug.Log("Joined Room");
     //   PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "profileId", ii } });
    }
}
