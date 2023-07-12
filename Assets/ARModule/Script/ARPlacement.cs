using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    public static ARPlacement Instance;
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    public GameObject aRPlanDetectionInfoScreen;
    public Vector3 defaultScale;

    public GameObject spawnedObject;
    //private Pose PlacementPose;
    //private ARRaycastManager aRRaycastManager;
    //private bool placementPoseIsValid = false;
    //private float initialDistance;
    //private Vector3 initialScale;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //}

    //void Start()
    //{
    //    arObjectToSpawn = ARFaceModuleManager.Instance.mainAvatar;
    //    aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    //    aRPlanDetectionInfoScreen.SetActive(true);
    //}

    //// need to update placement indicator, placement pose and spawn 
    //void Update()
    //{
    //    /*if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        ARPlaceObject(); // at the moment this just spawns the gameobject
    //    }*/

    //    // scale using pinch involves two touches
    //    // we need to count both the touches, store it somewhere, measure the distance between pinch 
    //    // and scale gameobject depending on the pinch distance
    //    // we also need to ignore if the pinch distance is small (cases where two touches are registered accidently)

    //    /*if (Input.touchCount == 2)
    //    {
    //        var touchZero = Input.GetTouch(0);
    //        var touchOne = Input.GetTouch(1);

    //        // if any one of touchzero or touchOne is cancelled or maybe ended then do nothing
    //        if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
    //            touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
    //        {
    //            return; // basically do nothing
    //        }

    //        if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
    //        {
    //            initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
    //            initialScale = spawnedObject.transform.localScale;
    //            Debug.Log("Initial Disatance: " + initialDistance + "GameObject Name: "
    //                + arObjectToSpawn.name); // Just to check in console
    //        }
    //        else // if touch is moved
    //        {
    //            var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

    //            //if accidentally touched or pinch movement is very very small
    //            if (Mathf.Approximately(initialDistance, 0))
    //            {
    //                return; // do nothing if it can be ignored where inital distance is very close to zero
    //            }

    //            var factor = currentDistance / initialDistance;
    //            spawnedObject.transform.localScale = initialScale * factor; // scale multiplied by the factor we calculated
    //        }
    //    }*/
    //    UpdatePlacementPose();
    //    UpdatePlacementIndicator();
    //}

    //void UpdatePlacementIndicator()
    //{
    //    if (spawnedObject == null && placementPoseIsValid)
    //    {
    //        Debug.LogError("placementIndicator true.......");
    //        placementIndicator.SetActive(true);
    //        placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);

    //        if (spawnedObject == null && placementPoseIsValid)
    //        {
    //            ARPlaceObject(); // at the moment this just spawns the gameobject
    //        }
    //    }
    //    else
    //    {
    //        //Debug.LogError("placementIndicator false");
    //        placementIndicator.SetActive(false);
    //    }
    //}

    //void UpdatePlacementPose()
    //{
    //    var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
    //    var hits = new List<ARRaycastHit>();
    //    aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

    //    placementPoseIsValid = hits.Count > 0;
    //    if (placementPoseIsValid)
    //    {
    //        PlacementPose = hits[0].pose;
    //    }
    //}

    //void ARPlaceObject()
    //{
    //    aRPlanDetectionInfoScreen.SetActive(false);
    //    Debug.LogError("ARPlaceObject.......");
    //    Vector3 finalPos = new Vector3(PlacementPose.position.x, (PlacementPose.position.y - 0.3f), (PlacementPose.position.z + 0.4f));
    //    //Vector3 finalPos = new Vector3(PlacementPose.position.x, PlacementPose.position.y, (PlacementPose.position.z + 0.2f));
    //    spawnedObject = Instantiate(arObjectToSpawn, finalPos, PlacementPose.rotation);
    //    spawnedObject.transform.localScale = defaultScale;
    //    spawnedObject.SetActive(true);
    //    Debug.LogError("Scale:" + spawnedObject.transform.localScale);
    //    //new changes.......
    //    spawnedObject.GetComponent<AvatarBorderSelectionManager>().isMainAvatarOnActionScreen = true;
    //    ARFaceModuleManager.Instance.addAvtarItem.Add(spawnedObject);

    //    /*if (spawnedObject.GetComponent<AvatarScript>().avatarShadowPlanObj != null)
    //    {
    //        spawnedObject.GetComponent<AvatarScript>().avatarShadowPlanObj.SetActive(true);
    //    }*/
    //}
}