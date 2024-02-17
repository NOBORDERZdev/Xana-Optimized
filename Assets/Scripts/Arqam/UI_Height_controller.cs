using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Height_controller : MonoBehaviour
{
    public RectTransform targetTransform;
    public float defaultHeight = 1867.45f;
    public float deltaHeight = 1967.1f;

    private void OnEnable()
    {
        targetTransform.sizeDelta = new Vector2(targetTransform.sizeDelta.x, defaultHeight);
    }

    private void OnDisable()
    {
        targetTransform.sizeDelta = new Vector2(targetTransform.sizeDelta.x, deltaHeight);
    }


}
