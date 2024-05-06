using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionControlPopup : MonoBehaviour
{
    public GameObject VersionPopup;
    public GameObject FooterTabs;
    public GameObject SkipButton;
    public string bundleIdofLunchingApp;
    bool CheckLunchingFail = false;
    bool skipClick = false;
    // Start is called before the first frame update

    public void skip()
    {
        skipClick = true;
        PlayerPrefs.SetInt("SkipCounter", PlayerPrefs.GetInt("SkipCounter") + 1);
       // FooterTabs.SetActive(true);
        VersionPopup.SetActive(false);
    }

    public void Play_AppStore()
    {
#if UNITY_ANDROID
        OpenAppForAndroid();
        //Debug.Log("App update");
        // Application.OpenURL("https://www.apple.com/in/app-store/");
#elif UNITY_IOS
        Application.OpenURL("https://testflight.apple.com/join/TglRnz6A");
        Debug.Log("App update");
#endif


    }

    void OpenAppForAndroid()
    {

        Application.OpenURL("market://details?id=" + "com.nbi.xana");
        //string message = "https://play.google.com/store/apps/details?id=com.nbi.xana";
        //  message += AppID.ToString();
        //print(message);
        //AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        //AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
        //AndroidJavaObject launchIntent = null;
        //try
        //{
        //    launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.nbi.xana");
        //    launchIntent.Call<AndroidJavaObject>("putExtra", "arguments", message);
        //}
        //catch (System.Exception e)
        //{
        //    CheckLunchingFail = true;
        //}
        //print("app not found bool" + CheckLunchingFail);
        //if (CheckLunchingFail)
        //{
        //    print("app not found");
        //    AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        //    AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "https://play.google.com/store/apps/details?id=com.nbi.xana");

        //    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //    AndroidJavaObject intentObject = new AndroidJavaObject(
        //                    "android.content.Intent",
        //                    intentClass.GetStatic<string>("ACTION_VIEW"),
        //                    uriObject
        //    );

        //    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        //    currentActivity.Call("startActivity", intentObject);
        //    // Application.OpenURL(message);

        //}
        //else
        //{
        //    ca.Call("startActivity", launchIntent);
        //}
        //up.Dispose();
        //ca.Dispose();
        //packageManager.Dispose();
        //launchIntent.Dispose();
        //checkPackageAppIsPresent("com.xanaliaApp");

    }
}