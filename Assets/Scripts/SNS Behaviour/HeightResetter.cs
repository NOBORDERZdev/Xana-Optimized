using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightResetter : MonoBehaviour
{
    RectTransform Panel;

    void Awake()
    {
        Panel = GetComponent<RectTransform>();

        Rect safeArea = Screen.safeArea;
       Debug.Log("ScreenSafeZoneAdjuster:" + safeArea.height + "  :Height:" + Screen.height);
        if(safeArea.height != Screen.height)
        {
            Panel.offsetMax = new Vector2(0, 80);
        }
    }
}
