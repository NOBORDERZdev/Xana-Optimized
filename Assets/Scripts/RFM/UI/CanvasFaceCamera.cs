using UnityEngine;

namespace RFM.UI
{
    public class CanvasFaceCamera : MonoBehaviour
    {
        private Transform _cameraTransform;
        private void Start()
        {
            _cameraTransform = RFM.Managers.RFMManager.Instance._mainCam?.transform;

            if (_cameraTransform == null)
            {
                Debug.LogError("RFM RFM Camera not found!");
                //if (Camera.main != null)
                //{
                //    _cameraTransform = Camera.main.transform;
                //}
            }
        }

        private void Update()
        {
            if (!_cameraTransform)
            {
                Debug.LogError("RFM RFM Camera not found!");
                //if (Camera.main)
                //{
                //    _cameraTransform = Camera.main.transform;
                //}
            }
        
            transform.LookAt(2 * transform.position - _cameraTransform.position);
        }
    }
}
