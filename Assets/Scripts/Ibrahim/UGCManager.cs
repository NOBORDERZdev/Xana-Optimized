using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UGCManager : MonoBehaviour
{
    private Texture2D texture;
    public Image selfieSprite;
    private UGCItemsClass ugcItems;
    public TMP_Text warningText;
    public GameObject warningPanel;
    public GameObject permissionPopup;
    public static bool isSelfieTaken = false;

    public void OnClickSaveSelfieButton()
    {
        InventoryManager.instance.loaderPanel.SetActive(true);
        isSelfieTaken = false;
        if (texture != null)
        {
            // Encode texture to PNG format
            byte[] imageBytes = texture.EncodeToPNG();
            StartCoroutine(IERequest(imageBytes));
        }
    }
    public void OnClickRetakeSelfieButton()
    {
        //selfieSprite.sprite = null;
        isSelfieTaken = false;
        InventoryManager.instance.itemData.CharactertypeAi = false;
        //texture = null;
        OnClickSelfieButton();
    }
    public void OnClickBackSelfieButton()
    {
        isSelfieTaken = false;
        InventoryManager.instance.selfiePanel.SetActive(false);
        selfieSprite.sprite = null;
        texture = null;
        InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        InventoryManager.instance.itemData.CharactertypeAi = false;
        GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
    }

    public void CheckPermissionStatus()
    {
        if (Application.isEditor)
        {
            permissionPopup.SetActive(true);
        }
        else
        {
            NativeCamera.Permission permission = NativeCamera.CheckPermission(true);
#if !UNITY_EDITOR && UNITY_ANDROID
             if (permission == NativeCamera.Permission.ShouldAsk) //||permission == NativeCamera.Permission.Denied
            {
                permissionPopup.SetActive(true);
            }
            else
            {
                OnClickSelfieButton();
            }
#elif !UNITY_EDITOR && UNITY_IOS
                if(PlayerPrefs.GetInt("PicPermission", 0) == 0){
                     permissionPopup.SetActive(true);
                }
                else
                {
                    OnClickSelfieButton();
                }
#endif
        }
    }

    public void OnClickSelfieButton()
    {
#if UNITY_IOS
            PlayerPrefs.SetInt("PicPermission", 1);

            if (permissionCheck == "false")
            {
                string url = MyNativeBindings.GetSettingsURL();
                Debug.Log("the settings url is:" + url);
                Application.OpenURL(url);
            }
            else
            {
                iOSCameraPermission.VerifyPermission(gameObject.name, "SampleCallback");
            }
            texture = null;
            selfieSprite.sprite = null;
            NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
            {
                if (path != null)
                {
                    texture = NativeCamera.LoadImageAtPath(path, -1, false);
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }

                    string fileName = Path.GetFileName(path);

                    Sprite capturedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    selfieSprite.sprite = capturedSprite;
                    selfieSprite.preserveAspect = true;
                    InventoryManager.instance.selfiePanel.SetActive(true);
                    InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(false);

                }
            }, -1);

#elif UNITY_ANDROID

        texture = null;
        selfieSprite.sprite = null;
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                texture = NativeCamera.LoadImageAtPath(path, -1, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load Image from " + path);
                    return;
                }
                string fileName = Path.GetFileName(path);

                // Calculate scaling factors to fit the image within the panel
                // Calculate the scaling factor to fit the texture into the panel
                //float panelWidth = InventoryManager.instance.selfiePanel.GetComponent<RectTransform>().rect.width;
                //float panelHeight = InventoryManager.instance.selfiePanel.GetComponent<RectTransform>().rect.height;

                //float scaleX = panelWidth / texture.width;
                //float scaleY = panelHeight / texture.height;

                // Use the minimum scaling factor to ensure the entire texture fits
                //float scaleFactor = Mathf.Min(scaleX, scaleY);

                // Apply the scaling factor to the sprite's RectTransform
                //selfieSprite.rectTransform.sizeDelta = new Vector2(texture.width * scaleFactor, texture.height * scaleFactor);
                Sprite capturedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                selfieSprite.sprite = capturedSprite;
                selfieSprite.preserveAspect = true;
                InventoryManager.instance.selfiePanel.SetActive(true);
                InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(false);

            }
        }, -1);

        if (permission != NativeCamera.Permission.Granted)
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

    public IEnumerator IERequest(byte[] imageBytes)
    {
        float requestTimeout = 180f; // Timeout value in seconds (3 minutes)
        float timer = 0f;
        // Create a form with 'multipart/form-data' encoding
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.jpg", "image/*");
        UnityWebRequest www;
        if (APIBasepointManager.instance.IsXanaLive)
        {
            www = UnityWebRequest.Post(ConstantsGod.API_BASEURL_UGC + ConstantsGod.UGCAiApi, form); // for main server
        }
        else
        {
            www = UnityWebRequest.Post("http://182.70.242.10:8040/analyze-image/", form); // for testing server
        }
        www.SetRequestHeader("Accept", "application/json");
        // Start the request
        AsyncOperation operation = www.SendWebRequest();
        // Wait until the request is done or timeout occurs
        while (!operation.isDone && timer < requestTimeout)
        {
            yield return null; // Wait for the next frame
            timer += Time.deltaTime;
        }
        // Check if the request has timed out
        if (timer >= requestTimeout)
        {
            Debug.Log("Request timed out.");
            // Handle timeout (e.g., show a message, stop further processing)
            www.Abort(); // Stop the request
            warningText.text = "The process has timed out. Please try again.";
            warningPanel.SetActive(true);
            InventoryManager.instance.loaderPanel.SetActive(false);
            yield break; // Exit the coroutine
        }
        //using (UnityWebRequest www = UnityWebRequest.Post(ConstantsGod.UGCAiApi, form))
        {
            // www.SetRequestHeader("Accept", "application/json");
            //yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Failed to send image to the server : " + www.error);
                if (www.isHttpError)
                {
                    warningText.text = "An error occurred during processing. Please try again.";
                    warningPanel.SetActive(true);
                }
                //else
                //{
                //    warningPanel.SetActive(true);
                //    warningText.text = www.error;
                //}
                InventoryManager.instance.loaderPanel.SetActive(false);
                GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
            }
            else
            {
                UGCItemsClass response = JsonUtility.FromJson<UGCItemsClass>(www.downloadHandler.text);
                if (response.status == "reject")
                {
                    Debug.Log("Server Response: " + www.downloadHandler.text);
                    Debug.Log(response.description_Eng);
                    if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese) { warningText.text = response.description_Eng; }
                    else { warningText.text = response.description_Jap; }
                    warningPanel.SetActive(true);
                    InventoryManager.instance.loaderPanel.SetActive(false);
                    GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
                    //SNSNotificationHandler.Instance.ShowNotificationMsg(response.description);
                }
                else
                {
                    Debug.Log("Response Data: " + response.ToString());
                    // selfieSprite.gameObject.SetActive(false);
                    InventoryManager.instance.loaderPanel.SetActive(false);
                    // SNSNotificationHandler.Instance.ShowNotificationMsg(response.ToString());
                    ugcItems = response;
                    isSelfieTaken = true;
                    SetFaceData(InventoryManager.instance.ugcItemsData.GetFaceData(response.face_type), InventoryManager.instance.ugcItemsData.GetNoseData(response.nose_shape),
                        InventoryManager.instance.ugcItemsData.GetlipData(response.lip_shape), InventoryManager.instance.ugcItemsData.GetHairData(response.hair_style),
                        InventoryManager.instance.ugcItemsData.GetEyeData(response.eyes_color), InventoryManager.instance.ugcItemsData.GetEyeShapeData(response.eye_shape));
                    //InventoryManager.instance.ApplyUGCValueOnCharacter();
                    GameManager.Instance.m_RenderTextureCamera.gameObject.SetActive(true);
                    CharacSelectScroll.instance.OnClickNext();
                    //if (GameManager.Instance.UiManager.isAvatarSelectionBtnClicked)
                    //{
                    //    GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
                    //    AvatarCustomizationManager.Instance.ResetCharacterRotation(180f);
                    //}
                    //Swipe_menu.instance.OnClickNext();
                    GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();

                    // release memory after result successfull
                    //Destroy(selfieSprite);
                    Destroy(texture);
                }
            }
        }
    }

    public void SetFaceData(UGCItemsData.ItemData _itemFace, UGCItemsData.ItemData _itemNose, UGCItemsData.ItemData _itemLips, UGCItemsData.HairsEyeData _itemHair,
        UGCItemsData.HairsEyeData _itemEye, UGCItemsData.ItemData _itemEyeShape)
    {
        InventoryManager.instance.itemData.gender = ugcItems.gender.ToLower();
        InventoryManager.instance.itemData.hair_color = HexToColor(ugcItems.hair_color);
        char[] charsToTrim = { '#' };
        string cleanString = ugcItems.skin_color.TrimStart(charsToTrim);
        InventoryManager.instance.itemData.skin_color = cleanString;
        InventoryManager.instance.itemData.lips_color = HexToColor(ugcItems.lips_color);
        InventoryManager.instance.itemData.CharactertypeAi = true;
        if (_itemFace != null)
        {
            InventoryManager.instance.itemData.faceItemData = _itemFace.index;
        }
        if (_itemNose != null)
        {
            InventoryManager.instance.itemData.noseItemData = _itemNose.index;
        }
        if (_itemLips != null)
        {
            InventoryManager.instance.itemData.lipItemData = _itemLips.index;
        }
        if (_itemHair != null)
        {
            InventoryManager.instance.itemData._hairItemData = _itemHair.keyValue;
        }
        else
        {
            InventoryManager.instance.itemData._hairItemData = "No hair";
        }
        if (_itemEye != null)
        {
            InventoryManager.instance.itemData._eyeItemData = _itemEye.keyValue;
        }
        if (_itemEyeShape != null)
        {
            InventoryManager.instance.itemData.eyeShapeItemData = _itemEyeShape.index;
        }
    }
    Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Failed to parse hexadecimal color string: " + hex);
            return Color.white; // Return a default color or handle the error as needed
        }
    }
    #region Permission Methods
    private string permissionCheck = "";
    public void RequestPermission()
    {
        if (UniAndroidPermission.IsPermitted(AndroidPermission.CAMERA))
        {
            Debug.Log("CAMERA is already permitted!!");
            return;
        }
        UniAndroidPermission.RequestPermission(AndroidPermission.CAMERA, OnAllow, OnDeny, OnDenyAndNeverAskAgain);
    }

    private void OnAllow()
    {
        Debug.Log("CAMERA is permitted NOW!!");
    }

    private void OnDeny()
    {
        Debug.Log("CAMERA is NOT permitted...");
    }

    private void OnDenyAndNeverAskAgain()
    {
        Debug.Log("CAMERA is NOT permitted and checked never ask again option");

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
    private void SampleCallback(string permissionWasGranted)
    {
        Debug.Log("Callback.permissionWasGranted = " + permissionWasGranted);

        if (permissionWasGranted == "true")
        {
            // You can now use the device camera.
        }
        else
        {
            permissionCheck = permissionWasGranted;

            // permission denied, no access should be visible, when activated when requested permission
            return;

            // You cannot use the device camera.  You may want to display a message to the user
            // about changing the camera permission in the Settings app.
            // You may want to re-enable the button to display the Settings message again.
        }
    }
    #endregion
}

// UGC Items Data
[Serializable]
public class UGCItemsClass
{
    public string status;
    public string description_Eng;
    public string description_Jap;
    public string face_type;
    public string lip_shape;
    public string nose_shape;
    public string eyes_color;
    public string hair_color;
    public string skin_color;
    public string lips_color;
    public string hair_style;
    public string eye_shape;
    public string gender;

    public override string ToString()
    {
        return face_type + lip_shape + nose_shape + eyes_color + hair_color + skin_color + lips_color + hair_style + eye_shape + gender;
    }
}

