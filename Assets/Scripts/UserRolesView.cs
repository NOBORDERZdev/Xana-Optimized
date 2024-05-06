using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UserRolesView : MonoBehaviour
{
    [SerializeField]
    private string userPriorityRole;
    [SerializeField]
    private List<string> UserRoles = new List<string>();

    public GameObject allBadgesParent;
    public GameObject userRolesPanel;
    public Animator userRolesAnim;

    [Header("User Role Button Reference")]
    public GameObject userRoleButtonMainObj;//riken
    private Canvas userRoleButtonCanvas;//riken

    [Header("change grid count from script")]
    public GridLayoutGroup m_GridLayoutGrp;

    [Header("priority Based Badge on profile icon")]
    public GameObject[] alphaPassProfile;
    public GameObject[] premiumUserProfile;
    public GameObject[] djEventUIProfile;
    public GameObject[] vipPassEventProfile;

    [Header("All badges data")]
    public GameObject alphaPass;
    public GameObject PremiumPass;
    public GameObject djEventPass;
    public GameObject vipPassPass;

    private Canvas canvas;//riken
    private GraphicRaycaster graphicRaycaster;//riken

    //this method is used to set user roles.......
    public void SetUpUserRole(string _userPriorityRole, List<string> _UserRoles)
    {
        ResetBadges();
       Debug.Log("SetUpUserRole...:" + _userPriorityRole);
        userPriorityRole = _userPriorityRole;
        UserRoles = _UserRoles;

        switch (userPriorityRole)
        {
            case "alpha-pass":
                {
                    foreach (var item in alphaPassProfile)
                    {
                        item.SetActive(true);
                    }
                    break;
                }
            case "premium":
                {
                    foreach (var item in premiumUserProfile)
                    {
                        item.SetActive(true);
                    }
                    break;
                }
            case "dj-event":
                {
                    foreach (var item in djEventUIProfile)
                    {
                        item.SetActive(true);
                    }
                    break;
                }
            case "free":
                {
                    break;
                }
            case "vip-pass":
                {
                    foreach (var item in vipPassEventProfile)
                    {
                        item.SetActive(true);
                    }
                    break;
                }
        }


        for (int i = 0; i < UserRoles.Count; i++)
        {
            if (UserRoles[i].Equals("alpha-pass"))
            {
                alphaPass.SetActive(true);
            }
            if (UserRoles[i].Equals("premium"))
            {
                PremiumPass.SetActive(true);
            }
            if (UserRoles[i].Equals("dj-event"))
            {
                djEventPass.SetActive(true);
            }
            if (UserRoles[i].Equals("vip-pass"))
            {
                vipPassPass.SetActive(true);
            }
        }

        //if ((UserRoles.Count - 1) > 0)
        //{
        //    m_GridLayoutGrp.constraintCount = UserRoles.Count - 1;
        //    //userRolesPanel.SetActive(true);
        //}
    }

    public void ResetBadges()
    {
        foreach (var item in alphaPassProfile)
        {
            item.SetActive(false);
        }
        foreach (var item in premiumUserProfile)
        {
            item.SetActive(false);
        }
        foreach (var item in djEventUIProfile)
        {
            item.SetActive(false);
        }
        foreach (var item in vipPassEventProfile)
        {
            item.SetActive(false);
        }

        alphaPass.SetActive(false);
        PremiumPass.SetActive(false);
        djEventPass.SetActive(false);
        vipPassPass.SetActive(false);
    }

    public void OnUserRolesPanelOpen()
    {
        if (ConstantsGod.UserPriorityRole.Equals("Guest") || ConstantsGod.UserPriorityRole.Equals("free")) return;

       // userRolesAnim.transform.DOMoveY(userRoleButtonMainObj.transform.position.y - 105, 0);
        allBadgesParent.SetActive(true);
        canvas = userRolesPanel.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5;
        graphicRaycaster = userRolesPanel.AddComponent<GraphicRaycaster>();

        userRoleButtonCanvas = userRoleButtonMainObj.AddComponent<Canvas>();
        userRoleButtonCanvas.overrideSorting = true;
        userRoleButtonCanvas.sortingOrder = 6;

        FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().canvasGroup.interactable = false;
    }

    public void OnUserRolesPanelClose()
    {
        StartCoroutine(ClosePanel());
    }

    IEnumerator ClosePanel()
    {
        userRolesAnim.SetTrigger("Close");
        yield return new WaitForSeconds(.25f);
        allBadgesParent.SetActive(false);
        Destroy(graphicRaycaster);
        Destroy(canvas);
        Destroy(userRoleButtonCanvas);
        FeedUIController.Instance.footerCan.GetComponent<HomeFooterHandler>().canvasGroup.interactable = true;
    }

    public ScrollRectFasterEx profileMainScrollRect;
    public void OnValueChangedProfileMainScroll()
    {
        //Debug.Log(profileMainScrollRect.verticalNormalizedPosition);
        if (profileMainScrollRect.verticalNormalizedPosition <= 0.98)
        {
            userRoleButtonMainObj.SetActive(false);
        }
        else
        {
            userRoleButtonMainObj.SetActive(true);
        }
    }
}