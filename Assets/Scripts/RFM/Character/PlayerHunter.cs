using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerHunter : Hunter
    {
        // [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        //[SerializeField] private Animator npcAnim;
        //[SerializeField] private string velocityNameX, velocityNameY;

        //private List<GameObject> _players;
        //private Transform _target;

        // public Transform cameraTarget/* => cameraPosition*/;

        private void Start()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!this.enabled) return; // This function is called even if the script is disabled
            
            if (RFM.Globals.gameState != RFM.Globals.GameState.Gameplay) // Only catch players in gameplay state
            {
                return;
            }

            // Should only catch runners if this is local player
            //if (!GetComponent<PhotonView>().IsMine)
            //{
            //    return;
            //}
            

            //if (other.CompareTag(Globals.RUNNER_NPC_TAG))
            if (other.GetComponentInParent<NPCRunner>())
            {
                // Cannot directly call AIRunnerCaught() because NPCRunners are owned by the master client
                // other.transform.parent.GetComponent<NPCRunner>().AIRunnerCaught();
                
                killVFX.SetActive(true);
                var runnerViewId = other.GetComponent<PhotonView>().ViewID;
                var myViewId = GetComponent<PhotonView>().ViewID;

                object[] prameters = new object[] { runnerViewId, myViewId };

                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                    prameters,
                    new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                    SendOptions.SendReliable);
                return;
            }
            
            //else if (other.CompareTag(Globals.PLAYER_TAG))
            var playerRunner = other.GetComponent<PlayerRunner>();
            if (playerRunner != null && playerRunner.enabled)
            {
                killVFX.SetActive(true);

                // other.GetComponent<PlayerRunner>()?.PlayerRunnerCaughtByPlayer(this);

                var runnerViewId = other.GetComponent<PhotonView>().ViewID;
                var myViewId = GetComponent<PhotonView>().ViewID;

                object[] prameters = new object[] { runnerViewId, myViewId };

                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                    prameters,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable);
            }
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
}
