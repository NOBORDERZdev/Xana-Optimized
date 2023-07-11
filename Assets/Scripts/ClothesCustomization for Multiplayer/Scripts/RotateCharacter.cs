using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    public void CharacterRotate()
    {
        if(CharacterCustomizationManager.Instance.checkInternet.ispopUpClose)
            CharacterCustomizationManager.Instance.RotateAvatar();
    }
}
