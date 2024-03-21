using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoginPageManager : MonoBehaviour
{

    public static bool m_WorldIsClicked = false;
    public static bool m_MuseumIsClicked = false;
    public static bool m_isSignUpPassed = false;
    public GameObject m_WorldPlayPanel;
    public GameObject m_TapAndHoldPanel;
    public Text world_name, museum_description, CreaterName, tapholdworld, Tapcreatorname;
    public string[] _world_name, _description, _CreaterName;

    public string _SceneName;
    public Image _bannerimage, taphold_banner;
    public Sprite[] tapholdimages, tapholdProfileicon;

    public Sprite creatorPic;
    public GameObject profileIcon, taphold_profile, t_Scroll_discription;
    public ScrollActivity scrollActivity;
   

    public void SetPanelToBottom()
    {
        if (scrollActivity.gameObject.activeInHierarchy)
        {
            scrollActivity.BottomToTop();
           // m_WorldPlayPanel.transform.SetParent(WorldsHandler.instance.DescriptionParentPanel.transform);
            m_WorldPlayPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            m_WorldPlayPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            m_WorldPlayPanel.GetComponent<RectTransform>().sizeDelta = UIHandler.Instance.HomePage.GetComponent<RectTransform>().sizeDelta;
            m_WorldPlayPanel.GetComponent<RectTransform>().anchoredPosition = UIHandler.Instance.HomePage.GetComponent<RectTransform>().anchoredPosition;
            m_WorldPlayPanel.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            m_WorldPlayPanel.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
    public void CheckWorld()
    {
        UIHandler.Instance.HomePage.SetActive(true);
        this.GetComponent<FeedEventPrefab>().m_FadeImage = this.GetComponent<FeedEventPrefab>().worldIcon;
        this.GetComponent<FeedEventPrefab>().UpdateWorldPanel();
        if (!gameObject.GetComponent<PointerDown>().tapAndHolded)
        {
            string EnvironmentName = this.GetComponent<FeedEventPrefab>().m_EnvironmentName;
            bool isBuilderScene = this.GetComponent<FeedEventPrefab>().isBuilderScene;
            
            if (EnvironmentName == "TACHIBANA SHINNNOSUKE METAVERSE MEETUP" || EnvironmentName == "DJ Event")
            {
                print("Clicked on DJ event");
                EnvironmentName = "DJ Event";
                if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName, false))
                {
                    if (EnvironmentName != "DJ Event")
                    {
                        //UserPassManager.Instance.PremiumUserUI.SetActive(true);
                    }
                    else
                    {
                        UserPassManager.Instance.PremiumUserUIDJEvent.SetActive(true);
                    }
                    return;
                }
            }
            else if (EnvironmentName == " Astroboy x Tottori Metaverse Museum")
            {
                if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName, true))
                {
                    //UserPassManager.Instance.PremiumUserUIDJEvent.SetActive(true);
                    return;
                }
            }
           
            else if (!isBuilderScene)
            {
                if (!UserPassManager.Instance.CheckSpecificItem(EnvironmentName))
                {
                    //if (EnvironmentName != "DJ Event")
                    //{
                    //    //UserPassManager.Instance.PremiumUserUI.SetActive(true);
                    //}
                    //else
                    //{
                    //    UserPassManager.Instance.PremiumUserUIDJEvent.SetActive(true);
                    //}
                    return;
                }
            }
            m_WorldPlayPanel.SetActive(true);
            m_WorldPlayPanel.transform.SetParent(UIHandler.Instance.HomePage.transform);
            m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
            GetComponent<PointerDown>().tapAndHolded = false;
            m_MuseumIsClicked = false;
        }
        else
        {
            m_TapAndHoldPanel.SetActive(true);
            m_TapAndHoldPanel.GetComponent<OnPanel>().rectInterpolate = true;
            m_TapAndHoldPanel.transform.SetParent(UIHandler.Instance.HomePage.transform);
            m_TapAndHoldPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            m_TapAndHoldPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            m_TapAndHoldPanel.GetComponent<RectTransform>().sizeDelta = UIHandler.Instance.HomePage.GetComponent<RectTransform>().sizeDelta;
            m_TapAndHoldPanel.GetComponent<RectTransform>().anchoredPosition = UIHandler.Instance.HomePage.GetComponent<RectTransform>().anchoredPosition;
            m_TapAndHoldPanel.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            tapholdloadre();
            this.GetComponent<PointerDown>().tapAndHolded = false;
            GetComponent<PointerDown>().timer = 0;
        }
        UIHandler.Instance.ShowFooter(false);//rik

        GameManager.Instance.WorldBool = true;
        m_WorldIsClicked = true;
        m_isSignUpPassed = true;
        //------------------------
    }

    void ScrollTopSetter()
    {
        museum_description.GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    public void IsMuseumClicked()
    {
        m_MuseumIsClicked = true;
        _bannerimage.sprite = gameObject.GetComponent<Image>().sprite;
        ScrollTopSetter();
        PlayerPrefs.SetString("ScenetoLoad", _SceneName);
        tapholdloadre();
        switch (_SceneName)
        {

            case ("Aurora"):
                CreaterName.text = _CreaterName[0];
                museum_description.text = _description[0];
                world_name.text = _world_name[0];
                profileIcon.transform.GetComponent<Image>().sprite = creatorPic;

                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
            case ("GekkoSan"):
                CreaterName.text = _CreaterName[1];
                museum_description.text = _description[1];
                world_name.text = _world_name[1];
                profileIcon.gameObject.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();

                break;
            case ("Hokusai"):
                CreaterName.text = _CreaterName[2];
                museum_description.text = _description[2];
                world_name.text = _world_name[2];
                profileIcon.gameObject.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
            case ("Yukinori"):
                CreaterName.text = _CreaterName[3];
                museum_description.text = _description[3];
                world_name.text = _world_name[3];
                profileIcon.gameObject.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
            case ("Museum"):
                CreaterName.text = _CreaterName[4];
                museum_description.text = _description[4];
                world_name.text = _world_name[4];
                profileIcon.transform.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
            case ("GOZMuseum"):
                CreaterName.text = _CreaterName[5];
                museum_description.text = _description[5];
                world_name.text = _world_name[5];
                profileIcon.transform.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
            case ("NFTMuseum"):
                CreaterName.text = _CreaterName[6];
                museum_description.text = _description[6];
                world_name.text = _world_name[6];
                profileIcon.transform.GetComponent<Image>().sprite = creatorPic;
                world_name.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                museum_description.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                CreaterName.gameObject.GetComponent<UITextLocalization>().LocalizeTextText();
                break;
        }

        if (_SceneName == "GekkoSan")
        {

            t_Scroll_discription.GetComponent<ScrollRect>().enabled = false;
        }

    }



    public void CheckWorldOnClick()
    {
        if (m_WorldIsClicked || m_isSignUpPassed || XanaConstants.loggedIn)
        {
            UIHandler.Instance.LoginRegisterScreen.SetActive(false);
            UIHandler.Instance.HomePage.SetActive(true);
            m_WorldPlayPanel.SetActive(true);
            m_WorldPlayPanel.transform.SetParent(UIHandler.Instance.HomePage.transform);
            m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
            m_WorldIsClicked = false;
            UIHandler.Instance.ShowFooter(false);//rik
        }
    }

    private void tapholdloadre()
    {
        switch (_SceneName)
        {
            case ("Aurora"):
                taphold_banner.sprite = tapholdimages[0];
                tapholdworld.text = _world_name[0];
                Tapcreatorname.text = _CreaterName[0];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[0];
                break;
            case ("GekkoSan"):
                taphold_banner.sprite = tapholdimages[1];
                tapholdworld.text = _world_name[1];
                Tapcreatorname.text = _CreaterName[1];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[1];
                break;
            case ("Hokusai"):
                taphold_banner.sprite = tapholdimages[2];
                tapholdworld.text = _world_name[2];
                Tapcreatorname.text = _CreaterName[2];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[2];
                break;
            case ("Yukinori"):
                taphold_banner.sprite = tapholdimages[3];
                tapholdworld.text = _world_name[3];
                Tapcreatorname.text = _CreaterName[3];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[3];
                break;

            case ("Museum"):
                taphold_banner.sprite = tapholdimages[4];
                tapholdworld.text = _world_name[4];
                Tapcreatorname.text = _CreaterName[4];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[4];
                break;
            case ("GOZMuseum"):
                taphold_banner.sprite = tapholdimages[5];
                tapholdworld.text = _world_name[5];
                Tapcreatorname.text = _CreaterName[5];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[5];
                break;
            case ("NFTMuseum"):
                taphold_banner.sprite = tapholdimages[6];
                tapholdworld.text = _world_name[6];
                Tapcreatorname.text = _CreaterName[6];
                taphold_profile.transform.GetComponent<Image>().sprite = tapholdProfileicon[6];
                break;
        }
    }

}
