using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerHunter : Hunter
    {
        [SerializeField] private GameObject killVFX;

        private void OnEnable()
        {
            RFM.EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += OnGameOver;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= OnGameOver;
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

                other.GetComponent<Collider>().enabled = false; // disable the runner collider on local client to avoid duplicate calls

                return;
            }
            
            var playerRunner = other.GetComponent<PlayerRunner>();
            if (playerRunner != null && playerRunner.enabled)
            {
                killVFX.SetActive(true);

                var runnerViewId = other.GetComponent<PhotonView>().ViewID;
                var myViewId = GetComponent<PhotonView>().ViewID;

                object[] prameters = new object[] { runnerViewId, myViewId };

                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                    prameters,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable);

                // commented because RaiseEvent probably doesn't work when the object is disabled
                //other.gameObject.SetActive(false); // disable the runner on local client to avoid duplicate calls
            }
        }

        private void OnGameOver()
        {
            if (!this.enabled) return;

            PhotonNetwork.Destroy(transform.root.gameObject);
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
}
