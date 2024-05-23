using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePictureManager : MonoBehaviour
{
    public TMP_InputField userNameInput;
    //public string userName;
    public TextMeshProUGUI userFirstCharText;
    public Image profileImage;
    public GameObject createProfileObject;
    public Color[] profileBGColors;
    public Image profileBGImg;
    public RenderTexture profileRT;
    public string savePath;
    public static ProfilePictureManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {

    }
    public void MakeProfilePicture(string userName)
    {
        Debug.Log("MakeProfilePicture: " + userName);
        StartCoroutine(IEMakeProfilePicture(userName));
    }

    public IEnumerator IEMakeProfilePicture(string userName)
    {
        //Debug.LogError("IEMakeProfilePicture: " + userName);
        createProfileObject.SetActive(true);
        profileBGImg.color = profileBGColors[UnityEngine.Random.Range(0, profileBGColors.Length)];
        userName = userName.Replace(" ", "");

        if (!string.IsNullOrEmpty(userName))
        {
            // Regular expression pattern to match only alphabetic characters
            string pattern = @"^[a-zA-Z]+$";
            string firstChar = "";
            // Check if input matches the pattern
            for (int i = 0; i < userName.Length; i++)
            {
                if (Regex.IsMatch(userName[i].ToString(), pattern))
                {
                    firstChar = userName[i].ToString();
                    Debug.Log("Input is valid");
                    break;
                }
                else
                {
                    Debug.Log("Input is invalid");
                }
            }
            if (string.IsNullOrEmpty(firstChar))
            {
                userFirstCharText.text = "X";
            }
            else
            {
                userFirstCharText.text = firstChar.ToString();
            }

            yield return new WaitForSeconds(.1f);
            Texture2D tempTexture = new Texture2D(profileRT.width, profileRT.height, TextureFormat.RGB24, false);
            // Read pixels from RenderTexture to the temporary texture
            RenderTexture.active = profileRT;
            tempTexture.ReadPixels(new Rect(0, 0, profileRT.width, profileRT.height), 0, 0);
            RenderTexture.active = null;
            tempTexture.Apply();
            byte[] bytes = tempTexture.EncodeToPNG();
            if (!Directory.Exists((Application.persistentDataPath + "/Profile")))
            {
                Directory.CreateDirectory((Application.persistentDataPath + "/Profile"));
            }
            savePath = Application.persistentDataPath + "/Profile/userProfile.png";
            Debug.Log(savePath);
            System.IO.File.WriteAllBytes(savePath, bytes);
            yield return new WaitForSeconds(.1f);
            savePath = Application.persistentDataPath + "/Profile/userProfile.png";

            byte[] fileData = File.ReadAllBytes(savePath);
            while (string.IsNullOrEmpty(ConstantsHolder.xanaToken) && string.IsNullOrEmpty(ConstantsGod.AUTH_TOKEN))
            {
                //Debug.LogError("Waiting for token");
                yield return new WaitForSeconds(1f);
            }
            AWSHandler.Instance.PostObjectMethodAvatar(fileData,"-"+ firstChar + "-DefaultUserProfile", UploadProfile);
            
            Debug.Log("Changing  Imageing Now");
            profileImage.sprite = CreateSpriteFromTexture(NativeGallery.LoadImageAtPath(savePath));
            if (MyProfileDataManager.Instance)
                MyProfileDataManager.Instance.profileImage.sprite = profileImage.sprite;
        }
        createProfileObject.SetActive(false);
    }
    public UploadFileRoot uploadFileRoot=new UploadFileRoot();
    public void UploadProfile(UploadFileRoot uploadFile)
    {
        uploadFileRoot=uploadFile;
        StartCoroutine(UpdateUserAvatar());
    }

    public IEnumerator UpdateUserAvatar()
    {
        yield return new WaitForSeconds(2f);
        //SNS_APIManager.Instance.RequestUpdateUserAvatar(uploadFileRoot.cdn_link, "EditProfileAvatar");
        WWWForm form = new WWWForm();

        form.AddField("avatar", uploadFileRoot.cdn_link);

        using (UnityWebRequest www = UnityWebRequest.Post((ConstantsGod.API_BASEURL + ConstantsGod.r_url_UpdateUserAvatar), form))
        {
            string tempToken = ConstantsHolder.xanaToken;

            if (string.IsNullOrEmpty(tempToken))
                tempToken = ConstantsGod.AUTH_TOKEN;

            www.SetRequestHeader("Authorization", tempToken);

            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Uploading complete!");
                string data = www.downloadHandler.text;
                ChangeProfileAfterUploading();
                //Debug.Log("UpdateUserAvatar data:" + data);
            }
        }

    }


    void ChangeProfileAfterUploading()
    {
        //MyProfileDataManager.Instance.profileImage.sprite = profileImage.sprite;    
        MyProfileDataManager.Instance.UpdateProfilePic();
    }
    Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        // Create a new sprite using the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}
