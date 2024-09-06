using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentHeight : MonoBehaviour
{
    public float HeightPadding = 0f;
    public bool AddPading = false;
    RectTransform _parentRectTransform;
    RectTransform _childRectTransform;

    void Start()
    {
        // Get the RectTransform components of the parent and the child
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        _childRectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        SetChildHeight();
    }

    public void SetChildHeight()
    {
        // Ensure the child follows the parent's height
        if (_parentRectTransform != null && _childRectTransform != null)
        {
            Vector2 sizeDelta = _childRectTransform.sizeDelta;
            sizeDelta.y = _parentRectTransform.rect.height;
            sizeDelta.y -= HeightPadding;
            _childRectTransform.sizeDelta = sizeDelta;
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
