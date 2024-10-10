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
    GameManager gameManager;
    private void OnEnable()
    {
        
    }
    private void Start()
    {
        instance = this;
        Application.targetFrameRate = 30;
        gameManager = GameManager.Instance;
        if (ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
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
            if(gameManager==null){ 
                gameManager = GameManager.Instance;  
            }
            if (gameManager.UiManager)
            {
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().alpha = 0;
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().interactable = false;
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
        if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            termsAndConditionText.color = Color.black;
        }
        
        Invoke("OnAgreeButtonClick", 1f);
    }

    public void OnAgreeButtonClick()
    {
        mainPanel.SetActive(false);
         if(gameManager){ 
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().alpha=1;
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().blocksRaycasts= true;
                gameManager.UiManager.Canvas.GetComponent<CanvasGroup>().interactable= true;
            }
        gameManager.UiManager.StartCoroutine(gameManager.UiManager.IsSplashEnable(false, 0.1f));
        PlayerPrefs.SetString("TermsConditionAgreement", "Agree");
        UserLoginSignupManager.instance.CheckForAutoLogin();
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
