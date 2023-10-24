using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class WorldCategoryScroller : ScrollRect
{
    private bool routeToParent = false;
    public HomeScreenScrollHandler ParentSlider;

    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            ParentSlider.OnDrag(eventData);
        else
            base.OnDrag(eventData);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            routeToParent = true;
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
            routeToParent = true;
        else
            routeToParent = false;

        if (routeToParent)
            ParentSlider.OnBeginDrag(eventData); 
        else
            base.OnBeginDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            ParentSlider.OnEndDrag(eventData);
        else
            base.OnEndDrag(eventData);
        routeToParent = false;
    }
}