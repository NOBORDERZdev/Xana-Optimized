using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentHeight : MonoBehaviour
{

    public float HeightPadding = 0f;
    public bool AddPading = false;
    private RectTransform ParentRectTransform;
    private RectTransform ChildRectTransform;

    void Start()
    {
        // Get the RectTransform components of the parent and the child
        ParentRectTransform = transform.parent.GetComponent<RectTransform>();
        ChildRectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        SetChildHeight();
    }

    public void SetChildHeight()
    {
        // Ensure the child follows the parent's height
        if (ParentRectTransform != null && ChildRectTransform != null)
        {
            Vector2 sizeDelta = ChildRectTransform.sizeDelta;
            sizeDelta.y = ParentRectTransform.rect.height;
            sizeDelta.y -= HeightPadding;
            ChildRectTransform.sizeDelta = sizeDelta;
        }
    }

    public void AddToHeightPaddingForSearchUI()
    {
        if (AddPading)
        {
            HeightPadding += 100f;
        }
        else
        {
            HeightPadding -= 100f;
        }
        SetChildHeight();
    }
}
