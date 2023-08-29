using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCam : MonoBehaviour
{
    [SerializeField] public float desiredClippingValues;
    float normalClippingValues;
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
        normalClippingValues = cam.nearClipPlane;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            cam.nearClipPlane = normalClippingValues;
            print("On Trigger Enter");
        }
    } 
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            cam.nearClipPlane = desiredClippingValues;
            print("On Trigger Exit");
        }
    }
}
