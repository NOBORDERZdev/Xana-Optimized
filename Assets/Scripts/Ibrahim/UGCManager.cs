using AIFLogger;
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
    public static bool isSelfieTaken = false;

    public void OnClickSaveSelfieButton()
    {
        StoreManager.instance.loaderPanel.SetActive(true);
        //isSelfieTaken = false;
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
        //texture = null;
        OnClickSelfieButton();
    }
    public void OnClickBackSelfieButton()
    {
        isSelfieTaken = false;
        StoreManager.instance.selfiePanel.SetActive(false);
        selfieSprite.sprite = null;
        texture = null;
        StoreManager.instance.StartPanel_PresetParentPanel.SetActive(true);
    }
    public void OnClickSelfieButton()
    {
#if UNITY_IOS
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
                    Texture2D texture = NativeCamera.LoadImageAtPath(path, -1, false);
                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }

                    string fileName = Path.GetFileName(path);
                    Debug.Log("filename : " + fileName);
                    Debug.Log("width:" + texture.width + " :height:" + texture.height);


                    Sprite capturedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    selfieSprite.sprite = capturedSprite;
                    selfiePanel.SetActive(true);
                    StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);

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
                Debug.Log("Camera filename : " + fileName);
                Debug.Log("width:" + texture.width + " :height:" + texture.height);
                // Calculate scaling factors to fit the image within the panel
                // Calculate the scaling factor to fit the texture into the panel
                //float panelWidth = StoreManager.instance.selfiePanel.GetComponent<RectTransform>().rect.width;
                //float panelHeight = StoreManager.instance.selfiePanel.GetComponent<RectTransform>().rect.height;

                //float scaleX = panelWidth / texture.width;
                //float scaleY = panelHeight / texture.height;

                // Use the minimum scaling factor to ensure the entire texture fits
                //float scaleFactor = Mathf.Min(scaleX, scaleY);

                // Apply the scaling factor to the sprite's RectTransform
                //selfieSprite.rectTransform.sizeDelta = new Vector2(texture.width * scaleFactor, texture.height * scaleFactor);
                Sprite capturedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                selfieSprite.sprite = capturedSprite;
                selfieSprite.preserveAspect = true;
                StoreManager.instance.selfiePanel.SetActive(true);
                StoreManager.instance.StartPanel_PresetParentPanel.SetActive(false);

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
        // Create a form with 'multipart/form-data' encoding
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.jpg", "image/*");

        using (UnityWebRequest www = UnityWebRequest.Post(ConstantsGod.UGCAiApi, form))
        {
            www.SetRequestHeader("Accept", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Failed to send image to the server : " + www.error);
                warningPanel.SetActive(true);
                warningText.text = www.error;
                StoreManager.instance.loaderPanel.SetActive(false);

            }
            else
            {
                UGCItemsClass response = JsonUtility.FromJson<UGCItemsClass>(www.downloadHandler.text);
                if (response.status == "reject")
                {
                    Debug.Log("Server Response: " + www.downloadHandler.text);
                    Debug.Log(response.description);
                    warningPanel.SetActive(true);
                    warningText.text = response.description;
                    StoreManager.instance.loaderPanel.SetActive(false);
                    //SNSNotificationManager.Instance.ShowNotificationMsg(response.description);
                    
                }
                else
                {
                    Debug.Log("Response Data: " + response.ToString());
                    // selfieSprite.gameObject.SetActive(false);
                    StoreManager.instance.loaderPanel.SetActive(false);
                   // SNSNotificationManager.Instance.ShowNotificationMsg(response.ToString());
                    ugcItems = response;
                    isSelfieTaken = true;
                    SetFaceData(StoreManager.instance.ugcItemsData.GetFaceData(response.face_type), StoreManager.instance.ugcItemsData.GetNoseData(response.nose_shape),
                        StoreManager.instance.ugcItemsData.GetlipData(response.lip_shape), StoreManager.instance.ugcItemsData.GetHairData(response.hair_style),
                        StoreManager.instance.ugcItemsData.GetEyeData(response.eyes_color));
                    //StoreManager.instance.ApplyUGCValueOnCharacter();
                    Swipe_menu.instance.OnClickNext();
                    GameManager.Instance.m_RenderTextureCamera.gameObject.SetActive(true);
                    GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
                    
                    CharacterCustomizationManager.Instance.ResetCharacterRotation(180f);
                }
            }
        }
    }
    public void SetFaceData(UGCItemsData.ItemData _itemFace, UGCItemsData.ItemData _itemNose, UGCItemsData.ItemData _itemLips, UGCItemsData.HairsEyeData _itemHair, UGCItemsData.HairsEyeData _itemEye)
    {

        StoreManager.instance.itemData.gender = ugcItems.gender;
        StoreManager.instance.itemData.hair_color = ugcItems.hair_color;
        StoreManager.instance.itemData.skin_color = ugcItems.skin_color;
        StoreManager.instance.itemData.lips_color = ugcItems.lips_color;

        if (_itemFace != null)
        {
            StoreManager.instance.itemData.faceItemData = _itemFace.index;
        }
        else 
        {
            StoreManager.instance.itemData.faceItemData = 52;

        }
        if (_itemNose != null)
        {
            StoreManager.instance.itemData.noseItemData = _itemNose.index;
        }
        else
        {
            StoreManager.instance.itemData.noseItemData = 67;

        }
        if (_itemLips != null)
        {
            StoreManager.instance.itemData.lipItemData = _itemLips.index;
        }
        else
        {
            StoreManager.instance.itemData.lipItemData = 58;
        }
        if (_itemHair != null)
        {
            StoreManager.instance.itemData._hairItemData = _itemHair.keyValue;
        }
        else
        {
            StoreManager.instance.itemData._hairItemData = "ai_boy_h_115";
        }
        if (_itemEye != null)
        {
            StoreManager.instance.itemData._eyeItemData = _itemEye.keyValue;
        }
        else
        {
            StoreManager.instance.itemData._eyeItemData = "ai_dusky_blue";
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
    public string description;
    public string face_type;
    public string lip_shape;
    public string nose_shape;
    public string eyes_color;
    public string hair_color;
    public string skin_color;
    public string lips_color;
    public string hair_style;
    public string gender;

    public override string ToString()
    {
        return face_type + lip_shape + nose_shape + eyes_color + hair_color + skin_color + lips_color + hair_style + gender;
    }
}

