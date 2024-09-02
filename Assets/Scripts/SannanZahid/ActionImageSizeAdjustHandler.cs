using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionImageSizeAdjustHandler : MonoBehaviour
{
    private void OnEnable()
    {
        transform.GetComponent<EmoteReactionItemBtnHandler>().BtnImg.enabled = false;
        StartCoroutine(ResizeElement());
    }
    IEnumerator ResizeElement()
    {
        yield return new WaitForSeconds(0.12f);
        if (GetComponent<EmoteReactionItemBtnHandler>().TypeOfAction == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            transform.GetComponent<EmoteReactionItemBtnHandler>().BtnImg.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
        }
        else
        {
            transform.GetComponent<EmoteReactionItemBtnHandler>().BtnImg.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        }
        transform.GetComponent<EmoteReactionItemBtnHandler>().BtnImg.enabled = true;
    }
}