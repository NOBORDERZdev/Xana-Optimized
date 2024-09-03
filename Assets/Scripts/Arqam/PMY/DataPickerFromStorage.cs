using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public class DataPickerFromStorage : MonoBehaviour
{
    public RawImage screenImg;
    public VideoPlayer player;


    void Start()
    {

    }

    public void OnPickImageFromGellery(int maxSize)
    {
#if UNITY_IOS
        if (PermissionCheck == "false")
        {
            string url = MyNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
        }
        else
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
        }

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                Debug.Log("OnPickGroupAvatarFromGellery path: " + path);
                screenImg.texture = texture;
            }
        });
        Debug.Log("Permission result: " + permission);

#elif UNITY_ANDROID              
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                Debug.Log("OnPickGroupAvatarFromGellery path: " + path);
                screenImg.texture = texture;
            }
        });
        if (permission != NativeGallery.Permission.Granted)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        Debug.Log("Permission result: " + permission);
#endif

    }

    public void OnPickVideoFromGellery(int maxSize)
    {
#if UNITY_IOS
        if (PermissionCheck == "false")
        {
            string url = MyNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
        }
        else
        {
            iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
        }

        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image
                player.url = path;
                player.Play();
            }
        });
        Debug.Log("Permission result: " + permission);

#elif UNITY_ANDROID              
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image
                player.url = path;
                player.Play();
            }
        });
        if (permission != NativeGallery.Permission.Granted)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }
        Debug.Log("Permission result: " + permission);
#endif

    }


}
