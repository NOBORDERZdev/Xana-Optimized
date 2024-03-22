using System.Collections;
using UnityEditor;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public bool CapFPS;

    void Awake()
    {
        if (CapFPS)
            StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        yield return new WaitForSeconds(0.5f);
#if !UNITY_EDITOR
		//Application.targetFrameRate = 60;
        Application.targetFrameRate = 30;//Screen.currentResolution.refreshRate;
        //QualitySettings.vSyncCount= 0;
        //Screen.SetResolution(1280, 720, true);
        // PlayerSettings.gcIncremental = true;
#endif
        //Debug.LogError("Target Frame Rate " + Application.targetFrameRate);
    }
}
