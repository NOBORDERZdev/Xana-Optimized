using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace RFM.Managers
{
    public class NPCManager : MonoBehaviour
    {
        // private GameObject[] players;
        // private List<PhotonView> _playersPhotonViews = new();
        // private NPCHunter[] npcs;

        // private void Start()
        // {
        //     Init();
        // }
        //
        // private void Init()
        // {
        //     _playersPhotonViews.Clear();
        //     // npcs = FindObjectsOfType<NPCHunter>()/*.ToList()*/;
        //     players = GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG);
        //     
        //     foreach (var player in players)
        //     {
        //         _playersPhotonViews.Add(player.GetComponent<PhotonView>());
        //     }
        // }

        // public void DeactivatePlayer(int creatorID)
        // {
        //     foreach (var player in playersPhotonViews)
        //     {
        //         if (player == null) continue;
        //
        //         if(player.CreatorActorNr == creatorID)
        //         {
        //             player.gameObject.SetActive(false);
        //             break;
        //         }
        //     }
        // }

        // public void ActivatePlayer(int creatorID)
        // {
        //     foreach (var player in playersPhotonViews)
        //     {
        //         if (player == null)
        //         {
        //             continue;
        //         }
        //
        //         if (player.CreatorActorNr == creatorID)
        //         {
        //             player.gameObject.SetActive(true);
        //             break;
        //         }
        //     }
        // }
    }
}
