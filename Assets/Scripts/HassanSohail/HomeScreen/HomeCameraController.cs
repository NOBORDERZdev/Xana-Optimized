using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCameraController : MonoBehaviour
{
    [SerializeField] GameObject LookObj;
    /*   [SerializeField] Camera camera;
     Vector3 touchStart;
     public float zoomOutMin = 1;
     public float zoomOutMax = 8;

     // Update is called once per frame
     void Update()
     {
         if (Input.GetMouseButtonDown(0))
         {
             touchStart = camera.ScreenToWorldPoint(Input.mousePosition);
             print("Mouse down : "+ touchStart);
         }
         if (Input.touchCount == 2)
         {
             Touch touchZero = Input.GetTouch(0);
             Touch touchOne = Input.GetTouch(1);

             Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
             Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

             float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
             float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

             float difference = currentMagnitude - prevMagnitude;

             zoom(difference * 0.01f);
         }
         else if (Input.GetMouseButton(0))
         {
             Vector3 direction = touchStart - camera.ScreenToWorldPoint(Input.mousePosition);
             print("Applying pos: "+direction);
             camera.transform.position += direction;
             //LookObj.transform.position += direction;
         }
         zoom(Input.GetAxis("Mouse ScrollWheel"));
     }

     void zoom(float increment)
     {
         print("Zoom call");
         camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - increment, zoomOutMin, zoomOutMax);
     }
     */
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.touchCount==1)
        {
            print("~~~~~");
            RaycastHit hit;
            // Cast a ray from the mouse position to the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //When the ray hits anything, print the world position of what the ray hits using hit.point
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            print(hit.point);
            LookObj.transform.position = hit.point;
        }
        
    }
}