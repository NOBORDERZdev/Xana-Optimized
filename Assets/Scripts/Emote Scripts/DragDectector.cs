using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDectector : EventTrigger
{
    public bool isDragging = false;
    PutObjectsOnPath putObjectsOnPath;
    

    public override void OnDrag(PointerEventData eventData)
    {
        //transform.parent.parent.GetComponent<PutObjectsOnPath>().OnDrag(eventData);
        //print("IN ON DRAG");
        isDragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //print("IN END DRAG");
        Invoke(nameof(enableClick), 1f);
    }

    void enableClick() {
        isDragging = false;
    }

}
