using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileImageOndisable : MonoBehaviour
{
    
    public Sprite DefaultImage;
    public Image ImageRefrence;
   
    private void OnDisable()
    {
        ImageRefrence.sprite = DefaultImage;
        ImageRefrence.sprite = DefaultImage;
        UserLoginSignupManager.instance.SetProfileAvatarTempPath = "";
        UserLoginSignupManager.instance.SetProfileAvatarTempFilename = "";
    }
}
