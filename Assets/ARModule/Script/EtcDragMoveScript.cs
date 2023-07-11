using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcDragMoveScript : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private bool isHit;

    private Touch touch;
    public float speedModifire = 0.01f;

    GameObject target;
    bool isMouseDrag;
    Vector3 screenPosition;
    Vector3 offset;

    float tempYPos;
    float tempZPos;
    Vector3 temp = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);
            if (target != null)
            {
                isMouseDrag = true;
                tempYPos = transform.position.y;
                tempZPos = transform.position.z;
                Debug.Log("target position :" + target.transform.position);
                //Convert world position to screen position.
                screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDrag = false;
        }

        if (isMouseDrag)
        {
            Vector3 gestureWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,screenPosition.z)) + offset;

            //track mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, screenPosition.y, screenPosition.z - (tempYPos- gestureWorldPoint.y));
            //Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);

            //convert screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace);
            //Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;

            //It will update target gameobject's current postion.
            target.transform.position = currentPosition;
        }
    }

    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.collider.gameObject;
        }
        return target;
    }
}