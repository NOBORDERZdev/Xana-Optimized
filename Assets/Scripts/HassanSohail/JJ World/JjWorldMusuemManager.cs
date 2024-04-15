using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JjWorldMusuemManager : MonoBehaviour
{
    public bool allowTeleportation= true;

    private void Awake()
    {
        if (ConstantsHolder.xanaConstants.orientationchanged)
        {
            ScreenOrientationManager._instance.MyOrientationChangeCode(DeviceOrientation.Portrait);
        }
    }
}
