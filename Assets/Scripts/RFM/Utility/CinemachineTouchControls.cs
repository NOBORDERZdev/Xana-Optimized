using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CinemachineTouchControls : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeedX = 2.0f;
    public float rotationSpeedY = 2.0f;

    [Header("Zoom Settings")]
    public float minZoom = 5.0f;
    public float maxZoom = 15.0f;
    public float zoomSpeed = 1.0f;


    private CinemachineFreeLook freeLookCamera;
    private bool isRotating = false; // Flag to check if the image is being touched

    private void Start()
    {
        // Find the Cinemachine FreeLook Camera component attached to this GameObject
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Check if the touch is over the image using a raycast
                if (IsTouchOverImage(touch.position))
                {
                    isRotating = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isRotating = false;
            }
        }

        // Rotate the camera only when the image is being touched
        if (isRotating)
        {
            float touchX = Input.GetTouch(0).deltaPosition.x * rotationSpeedX * Time.deltaTime;
            float touchY = Input.GetTouch(0).deltaPosition.y * rotationSpeedY * Time.deltaTime;

            freeLookCamera.m_XAxis.Value += touchX;
            freeLookCamera.m_YAxis.Value += (-touchY);
        }

        // Handle pinch-to-zoom
        /*
        if (Input.touchCount == 2)
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
        */
    }

    private bool IsTouchOverImage(Vector2 touchPosition)
    {
        // Perform a raycast to check if the touch is over the image
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (!result.gameObject.CompareTag("TouchAreaRFM"))
            {
                return false;
            }
            if (result.gameObject.CompareTag("TouchAreaRFM"))
            {
                return true;
            }
        }

        return false;
    }
}
