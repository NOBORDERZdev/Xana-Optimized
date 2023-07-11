using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableReactPopup : MonoBehaviour
{

public Image reactImage ;
private Sprite tempSprite;
public void OnEnable()
{
    tempSprite = reactImage.sprite;
    StartCoroutine(CheckImagePopup());
}

IEnumerator CheckImagePopup()
{
checkAgain:
    yield return new WaitForSeconds(5f);
    if (tempSprite == reactImage.sprite)
        gameObject.SetActive(false);
    else
    {
            tempSprite = reactImage.sprite;
        goto checkAgain;
    }
}
}
