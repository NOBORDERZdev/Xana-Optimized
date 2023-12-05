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
                if (Camera.main != null)
                {
                    _cameraTransform = Camera.main.transform;
                }
            }
        }

        private void Update()
        {
            if (!_cameraTransform)
            {
                if (Camera.main)
                {
                    _cameraTransform = Camera.main.transform;
                }
            }
        
            transform.LookAt(2 * transform.position - _cameraTransform.position);
        }
    }
}
