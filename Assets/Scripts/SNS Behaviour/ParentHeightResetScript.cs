using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentHeightResetScript : MonoBehaviour
{
    public ContentSizeFitter mainContent;

    public RectTransform content;

    public GameObject defultTargetObj;

    public RectTransform NameHeaderObject, buttonObjectHeader, footerObject;

    public GameObject[] allScreenTab;

    public ScrollRectGiftScreen scrollRectGiftScreen;

    public RectTransform tempMainScrollHeightObj;

    public float height;

    public bool includeFooter = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnEnable()
    {
        //StartCoroutine(SetFirstTabPos());
    }

    public IEnumerator SetFirstTabPos()
    {
        if (defultTargetObj != null)
        {
            yield return new WaitForSeconds(0.01f);
            transform.GetChild(0).GetComponent<ScrollRectGiftScreen>().SetPage(0);
            OnHeightReset(0);
            //yield return new WaitForSeconds(0.5f);
            //if (NameHeaderObject != null)
            //{
            //    if (includeFooter)
            //    {
            //        //height = (NameHeaderObject.rect.height - buttonObjectHeader.rect.height - footerObject.rect.height);
            //        //height = (Screen.height - height) / 2;
            //        //height = (Screen.height - height - NameHeaderObject.rect.height - buttonObjectHeader.rect.height - footerObject.rect.height);
            //        height = tempMainScrollHeightObj.rect.height;
            //    }
            //    else
            //    {
            //        height = (Screen.height - NameHeaderObject.rect.height - buttonObjectHeader.rect.height);
            //    }
            //    // Debug.LogError("height" + height);
            //}
            //HeightReset(defultTargetObj);
            /*for (int i = 0; i < allScreenTab.Length; i++)//kaik uses ke liye tha lekin yad nahi hai so cmnt ki hai
            {
                yield return new WaitForSeconds(0.1f);
                if (allScreenTab[i].GetComponent<RectTransform>().rect.height <= height && NameHeaderObject != null)
                {
                    // Debug.LogError("height" + height);
                    allScreenTab[i].GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    allScreenTab[i].GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.width, height);
                }
                else
                {
                    allScreenTab[i].GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
            }*/
        }
    }

    //public List<float> AllTabHeightList = new List<float>();
    //public float maxHeightval;
    //public int maxIndexOfList;
    //public void GetAndCheckMaxHeightInAllTab()
    //{
    //    AllTabHeightList.Clear();
    //    for (int i = 0; i < allScreenTab.Length; i++)
    //    {
    //        AllTabHeightList.Add(allScreenTab[i].GetComponent<RectTransform>().sizeDelta.y); 
    //    }

    //    maxHeightval = Mathf.Max(AllTabHeightList.ToArray());
    //    maxIndexOfList = AllTabHeightList.IndexOf(maxHeightval);

    //    //HeightReset(allScreenTab[maxIndexOfList]);
    //    HeightReset(allScreenTab[scrollRectGiftScreen._currentPage]);
    //}

    public void HeightReset(GameObject targetObjHeight)
    {
        //Debug.LogError("Height:" + height + "   :target:" + targetObjHeight.GetComponent<RectTransform>().rect.height);

        // AR Changes
        /*if (targetObjHeight.GetComponent<RectTransform>().rect.height <= height && NameHeaderObject != null)
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(targetObjHeight.GetComponent<RectTransform>().rect.width, height);
        }
        else*/
        //{
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.width, targetObjHeight.GetComponent<RectTransform>().sizeDelta.y);
        if (MyProfileDataManager.Instance.gameObject.activeSelf)
        {
            //print("==========My Profile Data Manager is active");
            StartCoroutine(MyProfileDataManager.Instance.WaitToRefreshProfileScreen());
        }
        //}

        //Commented below lines for now to make profile scroller work properly as below 3 lines were making issues in scroller
        //mainContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //if (gameObject.activeInHierarchy)
        //    StartCoroutine(waitToReset());
    }

    IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(0.001f);
        mainContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        while (content.anchoredPosition != new Vector2(content.anchoredPosition.x, 0))
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
            yield return new WaitForSeconds(0.001f);
        }
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, (Mathf.Abs(size.y)));
    }

    public void OnHeightReset(int index)
    {
        HeightReset(allScreenTab[index]);
    }

    public void SetParentheight(Vector2 _postTabHeight)
    {
        //Debug.Log("Setting parent height here: " + _postTabHeight.y);
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, _postTabHeight.y);
    }


}