using System.Collections;
using UnityEditor;
using UnityEngine;

public class PerformenceSetting : MonoBehaviour
{
    public static PerformenceSetting instance;

    public bool CapFPS;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            if (CapFPS)
            StartCoroutine(Init());
        }
        else
        {
            DestroyImmediate(this);
        }
       
    }

    IEnumerator Init()
    {

        yield return new WaitForSeconds(0.5f);
        #if !UNITY_EDITOR
		Application.targetFrameRate = 30;
        QualitySettings.vSyncCount= 0;
        //Screen.SetResolution(1280, 720, true);
        // PlayerSettings.gcIncremental = true;
        #endif

    }

}
