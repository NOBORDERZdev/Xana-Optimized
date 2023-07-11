using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexibleRectNewCustomWorld : MonoBehaviour
{
    public RectTransform[] Children;
    public float Offset;

    private RectTransform MyRect;

    private void Start()
    {
        MyRect = GetComponent<RectTransform>();
    }
    

    public void AdjustSize()
    {
        if (Children.Length > 0 && MyRect)
        {
            float TotalHeight = 0;
            foreach (RectTransform rectTransform in Children)
            {
                TotalHeight += rectTransform.rect.height;
            }

            MyRect.sizeDelta = new Vector2(MyRect.sizeDelta.x, TotalHeight + Offset);
        }
        else
        {
            Debug.LogWarning("No Children Found to scale against or Rect Transform not found");
        }
    }

    private void OnGUI()
    {
        AdjustSize();
    }
}
