using UnityEngine;

public class PillarVisibility : MonoBehaviour
{
    private Camera mainCamera;
    private Transform cameraTransform;
    public float distanceThreshold = 10f;
    public float frontDistanceThreshold = 7f;
    public float backDistanceThreshold = 3f;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Vector3 toObject = transform.position - cameraTransform.position;
        float dotProduct = Vector3.Dot(cameraTransform.forward, toObject);

        if (dotProduct > 0)
        {
            Debug.Log("Object is in front of the camera.");
            distanceThreshold = frontDistanceThreshold;
        }
        else if (dotProduct < 0)
        {
            Debug.Log("Object is behind the camera.");
            distanceThreshold = backDistanceThreshold;
        }

        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);

        if (distanceToCamera <= distanceThreshold)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }

    ////////////////////

    //public float distanceThreshold = 10f; // Set your desired distance threshold here.

    //private Camera mainCamera;
    //private MeshRenderer meshRenderer;

    //private void Start()
    //{
    //    // Get a reference to the main camera
    //    mainCamera = Camera.main;

    //    // Get a reference to the Mesh Renderer component
    //    meshRenderer = GetComponent<MeshRenderer>();
    //}

    //private void Update()
    //{
    //    // Calculate the distance between the object and the camera
    //    float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);

    //    // Check if the distance is less than or equal to the threshold
    //    if (distanceToCamera <= distanceThreshold)
    //    {
    //        // If the distance is within the threshold, disable the Mesh Renderer
    //        meshRenderer.enabled = false;
    //    }
    //    else
    //    {
    //        // If the distance is greater than the threshold, enable the Mesh Renderer
    //        meshRenderer.enabled = true;
    //    }
    //}









    //=============================================================================================================

    //private Camera mainCamera;
    //private MeshRenderer meshRenderer;

    //private void Start()
    //{
    //    // Get a reference to the main camera
    //    mainCamera = Camera.main;

    //    // Get a reference to the Mesh Renderer component
    //    meshRenderer = GetComponent<MeshRenderer>();
    //}

    //private void Update()
    //{
    //    // Check if the object's bounds intersect with the camera's frustum
    //    if (IsObjectInView())
    //    {
    //        // If the object is in view, disable its Mesh Renderer
    //        meshRenderer.enabled = false;
    //    }
    //    else
    //    {
    //        // If the object is out of view, enable its Mesh Renderer
    //        meshRenderer.enabled = true;
    //    }
    //}

    //private bool IsObjectInView()
    //{
    //    // Calculate the object's bounds
    //    Bounds objectBounds = meshRenderer.bounds;

    //    // Check if the bounds intersect with the camera's frustum
    //    return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(mainCamera), objectBounds);
    //}

    //=====================================================================================================================================


    //private Transform cameraTransform;
    //private MeshRenderer pillarRenderer;

    //private void Start()
    //{
    //    // Get a reference to the main camera's transform
    //    cameraTransform = Camera.main.transform;

    //    // Get a reference to the Mesh Renderer of the pillar
    //    pillarRenderer = GetComponent<MeshRenderer>();
    //}

    //private void Update()
    //{
    //    // Check if the pillar obstructs the camera's view
    //    if (IsPillarObstructingView())
    //    {
    //        // Disable the Mesh Renderer to hide the pillar
    //        pillarRenderer.enabled = false;
    //    }
    //    else
    //    {
    //        // Enable the Mesh Renderer to show the pillar
    //        pillarRenderer.enabled = true;
    //    }
    //}
    //public float thresholdAngle = 45f;
    //private bool IsPillarObstructingView()
    //{
    //    // Calculate the direction from the camera to the pillar
    //    Vector3 toPillar = transform.position - cameraTransform.position;

    //    // Calculate the angle between the camera's forward direction and the direction to the pillar
    //    float angle = Vector3.Angle(-cameraTransform.forward, toPillar);

    //    // You can adjust this threshold angle based on your needs
    //    // If the angle is below the threshold, consider the pillar obstructing the view

    ////    thresholdAngle = 60f;
    //    return angle < thresholdAngle;
    //}
}