using System;
using Photon.Pun;
using UnityEngine;

namespace RFM
{
    public class RFMPlayer : MonoBehaviour, IPunObservable
    {
        public bool isHunter;
        
        [Header("Invisibility Card")]
        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        private Material[][] _defaultMaterials;
        [SerializeField] private float invisibilityDuration = 30;
        private bool _isInvisible;
        //
        
        
        [Header("SpeedBoost Card")]
        [SerializeField] private float boostDuration = 20;
        //

        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStart;
            EventsManager.onRestarting += OnGameRestarting;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStart;
            EventsManager.onRestarting -= OnGameRestarting;
        }

        private void OnGameStart()
        {
            meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            // A single meshRenderer may have more than one materials.
            _defaultMaterials = new Material[meshRenderers.Length][];
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                _defaultMaterials[i] = meshRenderers[i].sharedMaterials;
            }
        }
        
        private void OnGameRestarting()
        {
            ResetMaterial();
            ResetSpeed();
        }

        private void Update()
        {
            if (_isInvisible)
            {
                ChangeMaterials();
            }
            else
            {
                ResetMaterial();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<RFMCard>(out var card))
            {
                switch (card.cardType)
                {
                    case RFMCard.CardType.Invisibility:
                    {
                        _isInvisible = true;

                        Debug.LogError("RFM Invisibility Card Picked");
                        break;
                    }
                    case RFMCard.CardType.SpeedBoost:
                    {
                        RFM.Globals.player.characterMovement.RunSpeed *= 2;
                        Invoke(nameof(ResetSpeed), boostDuration);
                        Debug.LogError("RFM SpeedBoost Card Picked");
                        break;
                    }
                }
                
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

        private void ChangeMaterials()
        {
            foreach (var meshRenderer in meshRenderers)
            {
                var mats = new Material[meshRenderer.sharedMaterials.Length];
                for (var index = 0; index < mats.Length; index++)
                {
                    mats[index] = RFMManager.Instance.invisibilityMaterial;
                }

                meshRenderer.sharedMaterials = mats;
            }

            _isInvisible = true;
            Invoke(nameof(ResetMaterial), invisibilityDuration);
        }

        
        private void ResetMaterial()
        {
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].sharedMaterials = _defaultMaterials[i];
            }
            
            _isInvisible = false;
        }

        private void ResetSpeed()
        {
            RFM.Globals.player.characterMovement.RunSpeed /= 2;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(_isInvisible);
            }
            else
            {
                // Network player, receive data
                this._isInvisible = (bool)stream.ReceiveNext();
            }
        }
    }
}
