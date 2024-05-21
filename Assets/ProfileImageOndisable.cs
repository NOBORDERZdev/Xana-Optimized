using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileImageOndisable : MonoBehaviour
{
    
    public Sprite defaultImage;
    public Image imageRefrence;
   
    private void OnDisable()
    {
        imageRefrence.sprite = defaultImage;
        UserLoginSignupManager.instance.profilePicText.enabled = true;
        UserLoginSignupManager.instance.setProfileAvatarTempPath = "";
        UserLoginSignupManager.instance.setProfileAvatarTempFilename = "";
    }
}
