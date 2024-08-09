using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SNSAPILoaderController : MonoBehaviour
{
    public GameObject mainLoaderObj;
    public GameObject loaderObj;
    public GameObject AddRemoveFriendLoader;

    public GameObject uploadObj;
    public Image fillImage;
    public TextMeshProUGUI percentageText;

    //this method is used to show api loader.......
    public void ShowApiLoader(bool isShow)
    {
        mainLoaderObj.SetActive(isShow);
    }
    public void ShowFriendApiLoader(bool isOn)
    {
        AddRemoveFriendLoader.SetActive(isOn);
    }

    //this method is used to Reset to default....... 
    public void ResetDefault()
    {
        if (!loaderObj.activeSelf)
        {
            loaderObj.SetActive(true);
        }
        uploadObj.SetActive(false);
        fillImage.fillAmount = 0;
        percentageText.text = "0%";
    }

    //this method is used to setup uploading status ui.......
    public void ShowUploadStatusImage(bool isActive)
    {
        fillImage.fillAmount = 0;
        percentageText.text = "0%";

        uploadObj.SetActive(isActive);

        if (!isActive)
        {
            ResetDefault();
        }
        else 
        {
            loaderObj.SetActive(false);        
        }
    }

    //this method is used to Uploading progress display.......
    public void UploadingStatus(int uploadingValue)
    {
        float fill = ((float)uploadingValue / 100f);
        //Debug.LogError("FillAmount:" + fill + " :uploadValue:" + uploadingValue);
        fillImage.fillAmount = fill;
        percentageText.text = uploadingValue.ToString() + "%";
    }
}