using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerHunter : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;

        private List<GameObject> _players;
        private Transform _target;

        public Transform CameraTarget => cameraPosition;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Globals.PLAYER_TAG/*Globals.LOCAL_PLAYER_TAG*/))
            {
                // if (Globals.player == null) Globals.player = other.GetComponent<PlayerControllerNew>().gameObject;
                _players.Remove(other.gameObject);

                // PhotonView is on the parent of the gameobject that has a collider.
                int colliderViewId = other.transform.parent.GetComponent<PhotonView>().ViewID;

                RFM.Managers.RFMManager.Instance.photonView.RPC("LocalPlayerCaughtByHunter", RpcTarget.All, colliderViewId);
                killVFX.SetActive(true);
            }
        }
    }
}
