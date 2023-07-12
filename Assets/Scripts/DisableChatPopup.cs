using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableChatPopup : MonoBehaviour
{

    public TMPro.TMP_Text chatText;
    private string tempString;
    public void OnEnable()
    {
        tempString = chatText.text;
        StartCoroutine(CheckChatPopup());
    }

    IEnumerator CheckChatPopup()
    {
        checkAgain:
        yield return new WaitForSeconds(5f);
        if (tempString == chatText.text)
            gameObject.SetActive(false);
        else
        {
            tempString = chatText.text;
            goto checkAgain;
        }
    }
}
