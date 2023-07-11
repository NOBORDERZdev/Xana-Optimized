using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userNameOnOff : MonoBehaviour
{
    public GameObject otherButton;

    //private void OnEnable()
    //{
        //    if (XanaConstants.xanaConstants.userName == 1)
        //    {
        //        if (this.gameObject.name == "OffButtonName")
        //        {
        //            otherButton.SetActive(true);
        //            this.gameObject.SetActive(false);
        //        }
        //        else if (this.gameObject.name == "OnButtonName")
        //        {
        //            this.gameObject.SetActive(true);
        //            otherButton.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        if (this.gameObject.name == "OffButtonName")
        //        {
        //            this.gameObject.SetActive(true);
        //            otherButton.SetActive(false);
        //        }
        //        else if (this.gameObject.name == "OnButtonName")
        //        {
        //            otherButton.SetActive(true);
        //            this.gameObject.SetActive(false);
        //        }
        //    }
        //XanaVoiceChat.instance.UpdateMicButton();
    //}

    public void ClickUserName()
    {
        Debug.Log("XanaValu:"+ XanaConstants.xanaConstants.userName);
        if (XanaConstants.xanaConstants.userName == 1)
        {

            ArrowManager.OnInvokeUsername(0);
           XanaConstants.xanaConstants.userName = 0;
           ReferrencesForDynamicMuseum.instance.onBtnUsername.SetActive(false);
        }
        else
        {
            ArrowManager.OnInvokeUsername(1);
           
            XanaConstants.xanaConstants.userName = 1;
            ReferrencesForDynamicMuseum.instance.onBtnUsername.SetActive(true);
        }
        //OnEnable();
    }
}
