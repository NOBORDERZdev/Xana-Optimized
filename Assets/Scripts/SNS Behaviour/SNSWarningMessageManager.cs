using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SNSWarningMessageManager : MonoBehaviour
{
    public static SNSWarningMessageManager Instance;

    public GameObject warningMessageScreen;
    public Text warningMessageText;

    private void OnEnable()
    {
        Instance = this;
    }

    //this method is used to show warning message with localize text.......
    public void ShowWarningMessage(string warningMessage)
    {
        warningMessageText.text = "";
        warningMessageText.text = TextLocalization.GetLocaliseTextByKey(warningMessage);
        warningMessageScreen.SetActive(true);
    }
}