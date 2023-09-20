using UnityEngine;
using Cinemachine;

public class CinemachineTouchControls : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeedX = 2.0f;
    public float rotationSpeedY = .05f;

    [Header("Zoom Settings")]
    public float minZoom = 5.0f;
    public float maxZoom = 15.0f;
    public float zoomSpeed = 1.0f;

    private CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        // Find the Cinemachine FreeLook Camera component attached to this GameObject
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        // Handle camera rotation with touch controls
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && freeLookCamera)
        {
            float touchX = Input.GetTouch(0).deltaPosition.x * rotationSpeedX * Time.deltaTime;
            float touchY = Input.GetTouch(0).deltaPosition.y * rotationSpeedY * Time.deltaTime;

            freeLookCamera.m_XAxis.Value += touchX;
            freeLookCamera.m_YAxis.Value += touchY;
        }

        // Handle pinch-to-zoom
        if (Input.touchCount == 2 && freeLookCamera)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float zoomDifference = currentMagnitude - prevMagnitude;
            freeLookCamera.m_Lens.FieldOfView -= zoomDifference * zoomSpeed * Time.deltaTime;
            freeLookCamera.m_Lens.FieldOfView = Mathf.Clamp(freeLookCamera.m_Lens.FieldOfView, minZoom, maxZoom);
        }
    }
}
