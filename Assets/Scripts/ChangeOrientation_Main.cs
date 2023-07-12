using System;
using System.Collections;
using UnityEngine;

public class ChangeOrientation_Main : MonoBehaviour
{
    public static event Action<DeviceOrientation> OnOrientationChange;
    public static float CheckDelay = 0.5f;        // How long to wait until we check again.

    static DeviceOrientation orientation;        // Current Device Orientation
    public bool isAlive = true;                    // Keep this script running?

    IEnumerator CheckForChange()
    {
        orientation = Input.deviceOrientation;

        while (isAlive)
        {
            print("Waqas : Orientation : Checking");
            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.LandscapeLeft:
                    if (orientation != Input.deviceOrientation)
                    {
                        orientation = Input.deviceOrientation;
                        OnOrientationChange?.Invoke(orientation);
                    }
                    break;
            }

            yield return new WaitForSeconds(CheckDelay);
        }
    }


    private void OnEnable()
    {
        isAlive = true;
        StartCoroutine(CheckForChange());
    }

    private void OnDisable()
    {
        isAlive = false;
        StopCoroutine(CheckForChange());
    }
}
