using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeletObjectBehaviour : MonoBehaviour
{
    bool isPointerOverButton = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       /* foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                isPointerOverButton = true;
               Debug.Log("pointerUp");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isPointerOverButton)
            {
               Debug.Log("destory");
                isPointerOverButton = false;
                Destroy(ARFaceModuleManager.Instance.mainAvatar);
            }
           
        }
        if (isPointerOverButton)
        {
           Debug.Log("destorrrrry");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
            if (hit.collider != null)
            {
                Destroy(ARFaceModuleManager.Instance.mainAvatar);
               // Destroy(hit.transform.gameObject);
            }
        }*/
    }
}
