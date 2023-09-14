using System;
using UnityEngine;

namespace RFM
{
    public class DisableNonRFMCanvasElements : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToHide;

        private void OnEnable()
        {
            EventsManager.onHideCanvasElements += HideElements;
        }
        
        private void OnDisable()
        {
            EventsManager.onHideCanvasElements -= HideElements;
        }

        private void HideElements()
        {
            foreach (var element in objectsToHide)
            {
                element.SetActive(false);
            }
        }
    }
}
