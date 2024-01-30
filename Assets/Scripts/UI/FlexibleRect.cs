using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexibleRect : MonoBehaviour
{
    public RectTransform[] Children;
    public float Offset;

    private RectTransform MyRect;

    public static Action<bool> OnAdjustSize;
    private float defaultHeight;
    private void Start()
    {
        MyRect = GetComponent<RectTransform>();
        defaultHeight = MyRect.sizeDelta.y;
        OnAdjustSize += AdjustSize;
    }
    private void OnDestroy()
    {
        OnAdjustSize -= AdjustSize;
    }

    public void AdjustSize(bool isSearchPanel)
    {
        if (Children.Length > 0 && !isSearchPanel)
        {
            float TotalHeight = 0;
            foreach (RectTransform rectTransform in Children)
            {
                if (rectTransform.gameObject.activeInHierarchy)
                {
                    TotalHeight += rectTransform.rect.height;
                }
            }
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, (TotalHeight) + Offset);
            MyRect.GetComponentInParent<HomeScreenScrollHandler>().verticalNormalizedPosition = 1;
        }
        else
        {
            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, defaultHeight);
            MyRect.GetComponentInParent<HomeScreenScrollHandler>().verticalNormalizedPosition = 1;
            Debug.LogWarning("No Children Found to scale against or Rect Transform not found");
        }
    }
}
