using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroRectSetter : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}