using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TermsAndConditions : MonoBehaviour
{
    public GameObject TabBG;
    public GameObject mainPanel;
    public Toggle allAgreeToggle;
    public Toggle termsAndPolicyToggle;
    public Toggle privacyPolicyToggle;

    public Button agreeButton;

    private string privacyPolicyLink = "https://cdn.xana.net/xanaprod/privacy-policy/PRIVACYPOLICY-2.pdf";
    private string termsAndConditionLink = "https://cdn.xana.net/xanaprod/privacy-policy/termsofuse.pdf";

    private void OnEnable()
    {
        CheckForTermsAndCondition();
    }
    private void Start()
    {
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
            TabBG.SetActive(true);
    }
    public void CheckForTermsAndCondition()
    {
        if (PlayerPrefs.HasKey("TermsConditionAgreement"))
        {
            mainPanel.SetActive(false);
        }
        else
        {
            mainPanel.SetActive(true);
        }
    }

    public void EnableToggle(Toggle toggle)
    {
        if (privacyPolicyToggle.isOn && termsAndPolicyToggle.isOn)
        {
            termsAndPolicyToggle.SetIsOnWithoutNotify(true);
            privacyPolicyToggle.SetIsOnWithoutNotify(true);
            allAgreeToggle.SetIsOnWithoutNotify(true);
            agreeButton.interactable = true;
        }
        else
        {
            allAgreeToggle.SetIsOnWithoutNotify(false);
            agreeButton.interactable = false;
        }
    }

    public void AgreeAllCondition()
    {
        if(privacyPolicyToggle.isOn && termsAndPolicyToggle.isOn)
        {
            termsAndPolicyToggle.SetIsOnWithoutNotify(false);
            privacyPolicyToggle.SetIsOnWithoutNotify(false);
            allAgreeToggle.SetIsOnWithoutNotify(false);
            agreeButton.interactable = false;
        }
        else
        {
            termsAndPolicyToggle.SetIsOnWithoutNotify(true);
            privacyPolicyToggle.SetIsOnWithoutNotify(true);
            allAgreeToggle.SetIsOnWithoutNotify(true);
            agreeButton.interactable = true;
        }
    }

    public void OnAgreeButtonClick()
    {
        mainPanel.SetActive(false);
        PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
    }


    public void OpenPrivacyPolicyHyperLink()
    {
        Application.OpenURL(privacyPolicyLink);
    }

    public void OpenTermsAndConditionHyperLink()
    {
        Application.OpenURL(termsAndConditionLink);
    }

}
