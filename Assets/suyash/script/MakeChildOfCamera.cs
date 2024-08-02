using UnityEngine;

public class MakeChildOfCamera : MonoBehaviour
{
    void Start()
    {
        // Find the main camera in the scene
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Make the camera a child of this instantiated GameObject
            mainCamera.transform.SetParent(transform, false);

            // Optionally adjust the local position and rotation if needed
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Main camera not found. Please ensure a camera is tagged as 'MainCamera'.");
        }
    }

}
