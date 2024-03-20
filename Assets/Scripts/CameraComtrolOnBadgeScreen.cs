using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComtrolOnBadgeScreen : MonoBehaviour
{

    public void pressedenablecamera() {

        CameraLook.instance.AllowControl();
    }
}
