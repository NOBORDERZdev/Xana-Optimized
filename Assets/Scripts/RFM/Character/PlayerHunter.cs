using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerHunter : MonoBehaviour
    {
        // [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        //[SerializeField] private Animator npcAnim;
        //[SerializeField] private string velocityNameX, velocityNameY;

        //private List<GameObject> _players;
        //private Transform _target;

        public Transform cameraTarget/* => cameraPosition*/;

        private void Start()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (RFM.Globals.gameState != RFM.Globals.GameState.Gameplay) // Only catch players in gameplay state
            {
                return;
            }

            //if (other.CompareTag(Globals.RUNNER_NPC_TAG))
            if (other.GetComponentInParent<NPCRunner>())
            {
                //_players.Remove(other.gameObject);
                //_target = null;
                killVFX.SetActive(true);
                // other.transform.parent.GetComponent<NPCRunner>().AIRunnerCaught();

                var viewId = other.GetComponent<PhotonView>().ViewID;

                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                    viewId,
                    new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                    SendOptions.SendReliable);

                return;
            }
            
            //else if (other.CompareTag(Globals.PLAYER_TAG))
            var playerRunner = other.GetComponent<PlayerRunner>();
            if (playerRunner != null && playerRunner.enabled)
            {
                Debug.LogError("PlayerHunter: OnTriggerEnter: PlayerRunner");
                //_players.Remove(other.gameObject);
                //_target = null;
                killVFX.SetActive(true);

                // other.GetComponent<PlayerRunner>()?.PlayerRunnerCaughtByPlayer(this);

                var viewId = other.GetComponent<PhotonView>().ViewID;

                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                    viewId,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable);
            }
        }
    }
}
