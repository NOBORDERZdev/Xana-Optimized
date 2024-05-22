using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentHeight : MonoBehaviour
{
    private RectTransform parentRectTransform;
    private RectTransform childRectTransform;

    public float heightPadding = 0f;
    public bool addPading = false;

    void Start()
    {
        // Get the RectTransform components of the parent and the child
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        childRectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        SetChildHeight();
    }

    public void SetChildHeight()
    {
        // Ensure the child follows the parent's height
        if (parentRectTransform != null && childRectTransform != null)
        {
            Vector2 sizeDelta = childRectTransform.sizeDelta;
            sizeDelta.y = parentRectTransform.rect.height;
            sizeDelta.y -= heightPadding;
            childRectTransform.sizeDelta = sizeDelta;
        }
    }

    public void AddToHeightPaddingForSearchUI()
    {
        if (addPading)
        {
            heightPadding += 100f;
        }
        else
        {
            heightPadding -= 100f;
        }
        SetChildHeight();
    }
}
