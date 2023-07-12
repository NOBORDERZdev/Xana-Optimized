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
        if (XanaConstants.xanaConstants.orientationchanged)
        {
            ChangeOrientation_waqas._instance.MyOrientationChangeCode(DeviceOrientation.Portrait);
        }
    }
}
