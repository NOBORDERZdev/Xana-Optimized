using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFloatCam : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float touchSensitivity = 0.1f;


    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera horizontally (around the y-axis)
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Calculate vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera vertically (around the x-axis)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

#endif


    }
}
