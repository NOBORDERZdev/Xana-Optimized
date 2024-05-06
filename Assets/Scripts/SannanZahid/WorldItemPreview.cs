using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WorldItemPreview : MonoBehaviour
{
   /* public static bool m_WorldIsClicked = false;
    public static bool m_MuseumIsClicked = false;
    public static bool m_isSignUpPassed = false;
    public GameObject m_WorldPlayPanel;
    public ScrollActivity scrollActivity;



    public void SetPanelToBottom()
    {
        if (scrollActivity.gameObject.activeInHierarchy)
        {
            scrollActivity.BottomToTop();
            m_WorldPlayPanel.transform.SetParent(WorldManager.instance.descriptionParentPanel.transform);
            m_WorldPlayPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            m_WorldPlayPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            m_WorldPlayPanel.GetComponent<RectTransform>().sizeDelta = GameManager.Instance.UiManager.HomePage.GetComponent<RectTransform>().sizeDelta;
            m_WorldPlayPanel.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.UiManager.HomePage.GetComponent<RectTransform>().anchoredPosition;
            m_WorldPlayPanel.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            m_WorldPlayPanel.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
    public void CheckWorld()
    {
        GameManager.Instance.UiManager.HomePage.SetActive(true);
        this.GetComponent<WorldItemView>().m_FadeImage = this.GetComponent<WorldItemView>().worldIcon;
        this.GetComponent<WorldItemView>().UpdateWorldPanel();
            string EnvironmentName = this.GetComponent<WorldItemView>().m_EnvironmentName;
            bool isBuilderScene = this.GetComponent<WorldItemView>().isBuilderScene;

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
            m_WorldPlayPanel.transform.SetParent(GameManager.Instance.UiManager.HomePage.transform);
            m_WorldPlayPanel.GetComponent<OnPanel>().rectInterpolate = true;
            m_MuseumIsClicked = false;
        GameManager.Instance.UiManager.ShowFooter(false);

        GameManager.Instance.WorldBool = true;
        m_WorldIsClicked = true;
        m_isSignUpPassed = true;
    }*/
}
