using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SNSWarningsHandler : MonoBehaviour
{
    public static SNSWarningsHandler Instance;

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
        warningMessageText.text = UITextLocalization.GetLocaliseTextByKey(warningMessage);
        warningMessageScreen.SetActive(true);
    }
}