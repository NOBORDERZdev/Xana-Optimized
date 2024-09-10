using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInputFieldPlugin;
using System.Text.RegularExpressions;
using AIFLogger;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class EmailEntryUIController : MonoBehaviour
{
    public AdvancedInputField UserEmailInputField;
    public GameObject SummitCXOEmailUI;
    public TextMeshProUGUI EmailText;
    public TextMeshProUGUI ErrorMsgText;
    public Button VerifyButton;
    public Action<string> AuthEmailAfterVerification;
    private Tween fadeTween;
    string verifiedEmail;
    bool isEmail;

    // Start is called before the first frame update
    void Start()
    {
        UserEmailInputField.OnValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(string newText)
    {
        isEmail = Regex.IsMatch(newText, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
        // Check if the entered string matches your desired value

        if (isEmail)
        {
            EmailText.color = Color.black;
            VerifyButton.interactable = isEmail;
            verifiedEmail = UserEmailInputField.GetText();
        }
        else
        {
            EmailText.color = Color.red;
            VerifyButton.interactable = isEmail;
        }
    }

    public void SetAuthEmailUIState(bool _state)
    {
        UserEmailInputField.Clear();
        SummitCXOEmailUI.SetActive(_state);
    }

    public void OnClickVerifyEmailBtn()
    {
        AuthEmailAfterVerification?.Invoke(verifiedEmail);
    }

    public void PlayErrorMsgAnim()
    {
        if (fadeTween != null)
        {
            fadeTween.Restart();
        }
        else
        {
            fadeTween = ErrorMsgText.DOFade(1, 1).OnComplete(() =>
            {
                fadeTween = ErrorMsgText.DOFade(0, 4).OnComplete(() =>
                {
                    fadeTween = null;
                });
            });
        }
    }
}
