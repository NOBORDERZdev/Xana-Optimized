using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TermsConditionsManager : MonoBehaviour
{
    public GameObject TabBG;
    public GameObject mainPanel;
    public Toggle allAgreeToggle;
    public Toggle termsAndPolicyToggle;
    public Toggle privacyPolicyToggle;
    public static TermsConditionsManager instance;
    public Button agreeButton;
    public TextMeshProUGUI termsAndConditionText;

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
            //if (UIHandler.Instance)
            //{
            //    UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().alpha = 1;
            //    UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //    UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().interactable = true;
            //}
            mainPanel.SetActive(false);
        }
        else
        {
            if (UIHandler.Instance)
            {
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().alpha = 0;
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().interactable = false;
            }
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
            //termsAndPolicyToggle.SetIsOnWithoutNotify(false);
            //privacyPolicyToggle.SetIsOnWithoutNotify(false);
            //allAgreeToggle.SetIsOnWithoutNotify(false);
            //agreeButton.interactable = false;
        }
        else
        {
            //termsAndPolicyToggle.SetIsOnWithoutNotify(true);
            //privacyPolicyToggle.SetIsOnWithoutNotify(true);
            //allAgreeToggle.SetIsOnWithoutNotify(true);
            //agreeButton.interactable = true;
        }
        termsAndConditionText.color = Color.black;
        Invoke("OnAgreeButtonClick", 1f);
    }

    public void OnAgreeButtonClick()
    {
        mainPanel.SetActive(false);
         if(UIHandler.Instance){ 
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().alpha=1;
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().blocksRaycasts= true;
                UIHandler.Instance.Canvas.GetComponent<CanvasGroup>().interactable= true;
            }
        UIHandler.Instance.StartCoroutine(UIHandler.Instance.IsSplashEnable(false, 0.1f));
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
