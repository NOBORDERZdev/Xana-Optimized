using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentHeightAdjuster : MonoBehaviour
{
    public RectTransform parentRect;
    public RectTransform childRect;

    public void SetParentHeight()
    {
        if (parentRect != null && childRect != null)
        {
            // Get the height of the child element
            float childHeight = childRect.rect.height;

            // Adjust the parent's height based on the child's height
            Vector2 parentSize = parentRect.sizeDelta;
            parentSize.y = childHeight;
            parentRect.sizeDelta = parentSize;
        }
    }
}
