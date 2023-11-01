using System.Collections;
using UnityEditor;
using UnityEngine;

public class PerformenceSetting : MonoBehaviour
{
    public bool CapFPS;

    void Awake()
    {
        if (CapFPS)
            StartCoroutine(Init());
        SizeofDeviceScreenSet();
    }
    IEnumerator Init()
    {
        yield return new WaitForSeconds(0.5f);
#if !UNITY_EDITOR
		//Application.targetFrameRate = 60;
        Application.targetFrameRate = 120;//Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount= 0;
        //Screen.SetResolution(1280, 720, true);
        // PlayerSettings.gcIncremental = true;
#endif
    }
    void SizeofDeviceScreenSet()
    {
        /// AS Checking for Portrate version
        float width = Screen.height;
        float height = Screen.width;
        int stateofDevice = 0;
        if (width.Equals(800) && height.Equals(480))
            stateofDevice = 0;
        else if (width.Equals(1280) && height.Equals(720))
            stateofDevice = 1;
        else if (width.Equals(1920) && height.Equals(1080))
            stateofDevice = 2;
        else if (width.Equals(2160) && height.Equals(1080))
            stateofDevice = 3;
        else if (width.Equals(2560) && height.Equals(1440))
            stateofDevice = 4;
        else if (width.Equals(2960) && height.Equals(1440))
            stateofDevice = 5;
        PlayerPrefs.SetInt("DeviceSizeStateXana", stateofDevice);
    }
}
