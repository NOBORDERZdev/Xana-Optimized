using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerHunter : MonoBehaviour
    {
        // [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;

        private List<GameObject> _players;
        private Transform _target;

        public Transform cameraTarget/* => cameraPosition*/;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Globals.RUNNER_NPC_TAG))
            {
                _players.Remove(other.gameObject);
                _target = null;
                killVFX.SetActive(true);
                other.transform.parent.GetComponent<NPCRunner>().AIRunnerCaught();
            }
            
            else if (other.CompareTag(Globals.PLAYER_TAG))
            {
                _players.Remove(other.gameObject);
                _target = null;
                killVFX.SetActive(true);
                
                other.GetComponent<PlayerRunner>()?.PlayerRunnerCaughtByPlayer(this);

                // PhotonView is on the parent of the gameobject that has a collider.
                // int colliderViewId = other.transform.root.GetComponent<PhotonView>().ViewID;
                //
                // RFM.Managers.RFMManager.Instance.photonView.RPC("LocalPlayerCaughtByHunter", 
                //     RpcTarget.All, colliderViewId);
            }
        }
    }
}
