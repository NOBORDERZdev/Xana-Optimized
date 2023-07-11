using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class toastSizeFitter : MonoBehaviour
{
    public RectTransform elementImage;
    private RectTransform rectTransform;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, elementImage.sizeDelta.y);
    }

}
