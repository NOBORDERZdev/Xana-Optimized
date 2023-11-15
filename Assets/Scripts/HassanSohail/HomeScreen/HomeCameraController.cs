using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeCameraController : MonoBehaviour
{
    float PanSpeed = 2.5f;
    float ZoomSpeedTouch = 0.1f;
    float ZoomSpeedMouse = 0.5f;
    
    [SerializeField]
    float[] BoundsX = new float[]{-2f, 2f};
    [SerializeField]
    float[] BoundsZ = new float[]{-5f, -2f};
    float[] ZoomBounds = new float[]{40f, 85f};
    
    private Camera cam;
    
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake() {
        cam = GetComponent<Camera>();
    }
    
    void Update() {
#if UNITY_EDITOR
        HandleMouse();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouch();
#endif
    }
    
     void HandleTouch()
    {
        print("iNPUT " + Input.touchCount);
        if (Input.touchCount>1)
        {
            // wasZoomingLastFrame = true; // ADDED        
            //// Zoom based on the distance between the new positions compared to the 
            //// distance between the previous positions.
            //float newDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            //float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
            //float offset = newDistance - oldDistance;

            //ZoomCamera(offset, ZoomSpeedTouch);

            //lastZoomPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

           // break;


            // get current touch positions
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);
            // get touch position from the previous frame
            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

            // get offset value
            float deltaDistance =currentTouchDistance - oldTouchDistance  ;
            print("DISTANCE IS "+deltaDistance);
            ZoomCamera (deltaDistance, ZoomSpeedTouch);
        }
        else if (Input.touchCount>0)
        {
            wasZoomingLastFrame = false;
            // If the touch began, capture its position and its finger ID.
            // Otherwise, if the finger ID of the touch doesn't match, skip it.
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
            } else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved) {
                PanCamera(touch.position);
            }
        }
        //switch (Input.touchCount)
        //{
        //    case 1: // Panning

        //        wasZoomingLastFrame = false;
            
        //    // If the touch began, capture its position and its finger ID.
        //    // Otherwise, if the finger ID of the touch doesn't match, skip it.
        //    Touch touch = Input.GetTouch(0);
        //    if (touch.phase == TouchPhase.Began) {
        //        lastPanPosition = touch.position;
        //        panFingerId = touch.fingerId;
        //    } else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved) {
        //        PanCamera(touch.position);
        //    }
        //    break;
     
        //    case 2: // Zooming
        //      wasZoomingLastFrame = true; // ADDED        
        //    // Zoom based on the distance between the new positions compared to the 
        //    // distance between the previous positions.
        //    float newDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        //    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
        //    float offset = newDistance - oldDistance;

        //    ZoomCamera(offset, ZoomSpeedTouch);

        //    lastZoomPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

        //   break;
                
        //default:
        //    wasZoomingLastFrame = false;
        //    break;
        //}
    }
    
    void HandleMouse() {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0)) {
            lastPanPosition = Input.mousePosition;
        } else if (Input.GetMouseButton(0)) {
            PanCamera(Input.mousePosition);
        }
    
        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel")*20f;
        ZoomCamera(scroll, ZoomSpeedMouse);
    }
    
    void PanCamera(Vector3 newPanPosition) {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);
        
        // Perform the movement
        transform.Translate(move, Space.World);  
        
        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;
    
        // Cache the position
        lastPanPosition = newPanPosition;
    }
    
    void ZoomCamera(float offset, float speed) {
        if (offset == 0) {
            return;
        }
    
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}
