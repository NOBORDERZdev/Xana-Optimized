using System;
using Photon.Pun;
using UnityEngine;

namespace RFM
{
    public class RFMPlayer : MonoBehaviour, IPunObservable
    {
        public bool isHunter;
        
        // For Invisibility Card
        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        private Material[][] _defaultMaterials;

        public bool _isInvisible;
        //

        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStart;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStart;
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
            Debug.LogError("RFMPlayer OnTriggerEnter() with " + other.gameObject.name + ", " + other.tag);
            
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
            Invoke(nameof(ResetMaterial), 100);
        }

        
        private void ResetMaterial()
        {
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].sharedMaterials = _defaultMaterials[i];
            }
            
            _isInvisible = false;
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
