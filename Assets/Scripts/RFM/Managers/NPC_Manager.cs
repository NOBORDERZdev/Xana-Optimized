using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RFM.Character;

namespace RFM
{
    public class NPC_Manager : MonoBehaviour
    {
        private GameObject[] players;
        private List<PhotonView> playersPhotonViews = new();
        private NPCHunter[] npcs;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            // players.Clear();
            playersPhotonViews.Clear();

            npcs = FindObjectsOfType<NPCHunter>()/*.ToList()*/;

            players = GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG);
            
            // players = GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG).ToList();
            foreach (var player in players) playersPhotonViews.Add(player.GetComponent<PhotonView>());
        }

        public void DeactivatePlayer(int creatorID)
        {
            foreach (var player in playersPhotonViews)
            {
                if (player == null) continue;

                if(player.CreatorActorNr == creatorID)
                {
                    player.gameObject.SetActive(false);
                    break;
                }
            }
        }

        public void ActivatePlayer(int creatorID)
        {
            foreach (var player in playersPhotonViews)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.CreatorActorNr == creatorID)
                {
                    player.gameObject.SetActive(true);
                    break;
                }
            }
        }

/*
        public int TotalActivePlayers()
        {
            int count = 0;
            foreach (var p in players)
            {
                if (p.activeInHierarchy) count++;
            }

            return count;
        }
*/
    }
}
