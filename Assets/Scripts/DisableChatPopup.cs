using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisableChatPopup : MonoBehaviour
{

    public TMPro.TMP_Text chatText;
    private string tempString;
    private int maxCharacterInLine = 20;


    public void OnEnable()
    {
        tempString = chatText.text;
        StartCoroutine(CheckChatPopup());
        CheckCharacterLength();
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

    private void CheckCharacterLength()
    {
        if (chatText.text.Length < maxCharacterInLine)
            chatText.GetComponent<LayoutElement>().enabled = false;
        else
        {
            chatText.GetComponent<LayoutElement>().enabled = true;
            chatText.GetComponent<LayoutElement>().preferredWidth = 18;
        }
    }

}
