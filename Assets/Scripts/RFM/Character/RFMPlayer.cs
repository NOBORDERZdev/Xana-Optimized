using System;
using UnityEngine;

namespace RFM
{
    public class RFMPlayer : MonoBehaviour
    {
        public bool isHunter;
        
        // For Invisibility Card
        public SkinnedMeshRenderer[] _meshRenderers;
        public Material[][] _defaultMaterials;
        //
        

        private void Start()
        {
            _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();


            // A single meshRenderer may have more than one materials.
            _defaultMaterials = new Material[_meshRenderers.Length][];
            for (var i = 0; i < _meshRenderers.Length; i++)
            {
                _defaultMaterials[i] = _meshRenderers[i].sharedMaterials;
                
                // for (var j = 0; j < _meshRenderers[i].sharedMaterials.Length; j++)
                // {
                //     var material = _meshRenderers[i].sharedMaterials[j];
                //     _defaultMaterials[i][j] = material;
                // }
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
                        ChangeMaterials();

                        Debug.LogError("RFM Invisibility Card Picked");
                        break;
                    }
                    case RFMCard.CardType.SpeedBoost:
                    {
                        Debug.LogError("RFM SpeedBoost Card Picked");
                        break;
                    }
                }
                
                Destroy(other.gameObject);
            }
        }

        private void ChangeMaterials()
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                var mats = new Material[meshRenderer.sharedMaterials.Length];
                for (var index = 0; index < mats.Length; index++)
                {
                    mats[index] = RFMManager.Instance.invisibilityMaterial;
                }

                meshRenderer.sharedMaterials = mats;
            }

            Invoke(nameof(ResetMaterial), 5);
        }

        
        private void ResetMaterial()
        {
            for (var i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].sharedMaterials = _defaultMaterials[i];
            }
        }
    }
}
