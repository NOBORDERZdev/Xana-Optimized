using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userNameOnOff : MonoBehaviour
{
    public GameObject otherButton;

    private void OnEnable()
    {
        if (ConstantsHolder.xanaConstants.userNameVisibilty == 1)
        {
            if (gameObject.name == "OffButtonName")
            {
                otherButton.SetActive(true);
                gameObject.SetActive(false);
            }
            else if (gameObject.name == "OnButtonName")
            {
                gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
        }
        else
        {
            if (gameObject.name == "OffButtonName")
            {
                gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
            else if (gameObject.name == "OnButtonName")
            {
                otherButton.SetActive(true);
                gameObject.SetActive(false);
            }
        }
        //XanaVoiceChat.instance.UpdateMicButton();
    }

    public void ClickUserName()
    {
        //Debug.Log("XanaValu:"+ ConstantsHolder.xanaConstants.userName);
        if (ConstantsHolder.xanaConstants.userNameVisibilty == 1)
        {

            ArrowManager.OnInvokeUsername(0);
           ConstantsHolder.xanaConstants.userNameVisibilty = 0;
            ReferencesForGamePlay.instance.onBtnUsername.SetActive(true);
            ReferencesForGamePlay.instance.offBtnUsername.SetActive(false);
        }
        else
        {
            ArrowManager.OnInvokeUsername(1);
           
            ConstantsHolder.xanaConstants.userNameVisibilty = 1;
            ReferencesForGamePlay.instance.offBtnUsername.SetActive(true);
            ReferencesForGamePlay.instance.onBtnUsername.SetActive(false);
        }
        //OnEnable();
    }
}
