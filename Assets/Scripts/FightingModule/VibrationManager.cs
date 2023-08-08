using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static void Vibrate()
    {
#if UNITY_EDITOR
        Debug.Log("Vibrating on PC");
#elif UNITY_STANDALONE_WIN
        Debug.Log("Vibrating on PC");
#else
        Debug.LogWarning("Vibration is not supported on this platform.");
        VibrationExample._instance.TapVibrateCustom();
#endif
    }
}
