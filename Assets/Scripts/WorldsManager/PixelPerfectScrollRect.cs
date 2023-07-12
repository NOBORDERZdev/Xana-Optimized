using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PixelPerfectScrollRect : ScrollRect
{
    public static Action<float> OnDragEndVerticalCustom;
    //protected override void LateUpdate()
    //{
    //    base.LateUpdate();
    //    ensurePixelPerfectScroll();
    //}

    void ensurePixelPerfectScroll()
    {
        float diff = content.rect.height - viewport.rect.height;
        float normalizedPixel = 1 / diff;
        verticalNormalizedPosition = Mathf.Ceil(verticalNormalizedPosition / normalizedPixel) * normalizedPixel;
        // can also do the same for horizontalNormalizedPosition, using rect.width instead of rect.height
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        OnDragEndVerticalCustom?.Invoke(verticalNormalizedPosition);
    }
}