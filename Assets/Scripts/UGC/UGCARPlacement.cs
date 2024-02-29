using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.GraphicsBuffer;

public class UGCARPlacement : MonoBehaviour
{
    public GameObject currentGameObject;
    public ARRaycastManager arRaycastManager;
    public Transform arCamera;
    public Vector3 touchStartPos;
    public float zoomMultiply = .1f;
    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;

    Vector2 firstTouchPrevPos, secondTouchPrevPos;

    [SerializeField]
    float zoomModifierSpeed = 0.1f;

    public float rotateSpeed;
    public List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public LayerMask layer;
    void Start()
    {

    }
    public bool ispointer=false;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            #region SpawnAndPosioning
            Touch touch = Input.GetTouch(0);

            // Perform a raycast from the screen point
            ispointer = IsPointerOverUIObject(touch.position);
            var touchPosition = touch.position;
            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                // Get the pose of the hit point
                var hitPose = hits[0].pose;

                // Instantiate the object at the hit pose
                if (IsPointerOverUIObject(touch.position))
                {
                    return;
                }
                if (!currentGameObject.activeInHierarchy)
                {
                    currentGameObject.SetActive(true);
                    currentGameObject.transform.parent.position = hitPose.position;
                    Vector3 playerPosition = currentGameObject.transform.position;
                    Vector3 restrictedTargetPosition = new Vector3(arCamera.position.x, playerPosition.y, arCamera.position.z);
                    currentGameObject.transform.LookAt(restrictedTargetPosition);
                }
                else
                {
                    currentGameObject.transform.parent.position = hitPose.position;
                }
                #endregion
            }
            else
            {
                #region Rotation

                // Check touch phase
                if (touch.phase == TouchPhase.Began)
                {
                    // Store the initial touch position
                    touchStartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    // Calculate the rotation amount based on touch delta position
                    float rotateAmount = (touch.position.x - touchStartPos.x) * rotateSpeed * Time.deltaTime;

                    // Rotate the GameObject around its up axis
                    if (currentGameObject.activeInHierarchy)
                    {
                        Vector3 rot = currentGameObject.transform.parent.rotation.eulerAngles;
                        rot += (Vector3.up * (-rotateAmount));
                        currentGameObject.transform.parent.rotation = Quaternion.Euler(rot);
                    }

                    // Update the touch start position for the next frame
                    touchStartPos = touch.position;
                }
                #endregion
            }

            #region Scaling

            if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
                secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

                touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
                touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

                zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;
                if (currentGameObject.activeInHierarchy)
                {
                    if (touchesPrevPosDifference < touchesCurPosDifference && currentGameObject.transform.parent.localScale.y < 2)
                        currentGameObject.transform.parent.localScale += Vector3.one * (zoomModifier) * zoomMultiply;
                    if (touchesPrevPosDifference > touchesCurPosDifference && currentGameObject.transform.parent.localScale.y > .1f)
                        currentGameObject.transform.parent.localScale -= Vector3.one * (zoomModifier) * zoomMultiply;
                }
            }
            #endregion
        }
    }

    bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        // Create a pointer event data with the current event system
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        // Set the position of the event data to the touch position
        eventDataCurrentPosition.position = touchPosition;

        // Create a list to store the raycast results
        var results = new List<RaycastResult>();

        // Raycast using the event data and store the results in the list
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // If there are any results, it means the touch is on a UI object
        return results.Count > 0;
    }
}
