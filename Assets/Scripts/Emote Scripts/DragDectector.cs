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
        isDragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Invoke(nameof(enableClick), 1f);
    }

    void enableClick() 
    {
        isDragging = false;
    }
}