using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UserPostFeature;
using UnityEngine.Networking;

public class HomeCameraController : MonoBehaviour
{
    float PanSpeed = 2.5f;
    float ZoomSpeedTouch = 0.1f;
    float ZoomSpeedMouse = 0.5f;
    
    //[SerializeField]
    float[] BoundsX = new float[]{1.5f, 8f};
    //[SerializeField]
    float[] BoundsZ = new float[]{-5f, 1f};
    float[] ZoomBounds = new float[]{35f, 45f};
    
    private Camera cam;
    
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake() {
      centerAlignCam();
    }
    
    /// <summary>
    /// To Center align the camera to the gird of avatar movements.
    /// </summary>
    void centerAlignCam()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView =ZoomBounds[1]; // to set zoom on start 
        cam.transform.position = new Vector3(2.16f, 3.68f, -0.1077f);
        //StartCoroutine (AllignWithCharacter());
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
    public void ViewPlayer(){ 
        StartCoroutine(AllignWithCharacter());
    }

    IEnumerator AllignWithCharacter()
    {
        while (PlayerPrefs.GetString("UserNameAndPassword") == "")
            yield return new WaitForSeconds(2f);
        //Vector3 pos = transform.position;
        yield return new WaitForSeconds(3f);
       // pos.x = Mathf.Clamp(GameManager.Instance.mainCharacter.transform.position.x, BoundsX[0], -0.43f);
       Vector3 pos = new Vector3(GameManager.Instance.mainCharacter.transform.position.x, transform.position.y,Mathf.Clamp(GameManager.Instance.mainCharacter.transform.position.x, BoundsZ[0], -0.7f));
        print("~~~~~~~ player cam pos "+pos);
        transform.position = pos;
    }
}
