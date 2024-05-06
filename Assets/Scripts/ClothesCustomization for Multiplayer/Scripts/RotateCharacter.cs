using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    public void CharacterRotate()
    {
        if(AvatarCustomizationManager.Instance.checkInternet.ispopUpClose)
            AvatarCustomizationManager.Instance.RotateAvatar();
    }
}
