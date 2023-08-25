using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System.Linq;
using Photon.Pun;

namespace RFM
{
    public class NPC_Manager : MonoBehaviour
    {
        private GameObject[]/*List<GameObject>*/ players/* = new List<GameObject>()*/;
        private List<PhotonView> playersPhotonViews = new List<PhotonView>();
        private NPC[]/*List<NPC>*/ npcs/* = new List<NPC>()*/;

        private void OnEnable()
        {
            // EventsManager.onRestarting += ResetAll;
            EventsManager.onGameTimeup += GameOver;
        }
        
        private void OnDisable()
        {
            // EventsManager.onRestarting -= ResetAll;
            EventsManager.onGameTimeup -= GameOver;
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            // players.Clear();
            playersPhotonViews.Clear();

            npcs = FindObjectsOfType<NPC>()/*.ToList()*/;

            players = GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG);
            
            // players = GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG).ToList();
            foreach (var player in players) playersPhotonViews.Add(player.GetComponent<PhotonView>());

            InvokeRepeating(nameof(SetTargetForNPC), 2, 1);
        }

        private void SetTargetForNPC()
        {
            // if (!PhotonNetwork.IsMasterClient) return;
            //
            // foreach(var npc in npcs)
            // {
            //     if (npc == null) continue;
            //
            //     var target = players[Random.Range(0, players.Length/*Count*/)];
            //     
            //     // GameObject playerInZone = players.Where(p => p.gameObject.activeInHierarchy)
            //     //     .Where(p => Vector2.Angle(new Vector2(p.transform.position.x,
            //     //                                   p.transform.position.z) -
            //     //                               new Vector2(npc.transform.position.x,
            //     //                                   npc.transform.position.z),
            //     //                     new Vector2(npc.transform.forward.x,
            //     //                         npc.transform.forward.z)) <
            //     //                 45)
            //     //     .FirstOrDefault(p => Vector3.Distance(p.transform.position,
            //     //                              npc.transform.position) ==
            //     //                          players.Min(p => Vector3.Distance(p.transform.position,
            //     //                              npc.transform.position)));
            //     
            //     // npc.FollowTarget(playerInZone == null ? target.transform.position : playerInZone.transform.position);
            //     
            //     npc.FollowTarget(target.transform.position);
            // }
        }

        private void GameOver()
        {
            CancelInvoke(nameof(SetTargetForNPC));
        }

        // private void ResetAll()
        // {
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         foreach (var npc in npcs)
        //         {
        //             PhotonNetwork.Destroy(npc.gameObject);
        //         }
        //     }
        //
        //     // npcs.Clear();
        // }

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

        public int TotalActivePlayers()
        {
            int count = 0;
            foreach (var p in players)
            {
                if (p.activeInHierarchy) count++;
            }

            return count;
        }
    }
}
