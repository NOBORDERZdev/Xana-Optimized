using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentHeightResetScript : MonoBehaviour
{
    public ContentSizeFitter mainContent;

    public RectTransform content;

    public GameObject defultTargetObj;


    public GameObject[] allScreenTab;


    public RectTransform tempMainScrollHeightObj;

    public float height;

    public bool includeFooter = false;

    public IEnumerator SetFirstTabPos()
    {
        if (defultTargetObj != null)
        {
            yield return new WaitForSeconds(0.01f);
            transform.GetChild(0).GetComponent<ScrollRectGiftScreen>().SetPage(0);
            OnHeightReset(0);
           
        }
    }
    

    public void HeightReset(GameObject targetObjHeight)
    {
        
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.width, targetObjHeight.GetComponent<RectTransform>().sizeDelta.y);
        if (MyProfileDataManager.Instance.gameObject.activeSelf)
        {
            StartCoroutine(MyProfileDataManager.Instance.WaitToRefreshProfileScreen());
        }
       
    }

    IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(0.1f);
        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, _postTabHeight.y);
        //mainContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //mainContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //while (content.anchoredPosition != new Vector2(content.anchoredPosition.x, 0))
        //{
        //    content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        //    yield return new WaitForSeconds(0.001f);
        //}
        //Vector2 size = GetComponent<RectTransform>().sizeDelta;
        //GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, (Mathf.Abs(size.y)));
    }

    public void OnHeightReset(int index)
    {
        HeightReset(allScreenTab[index]);
    }

    public void SetParentheight(Vector2 _postTabHeight)
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, _postTabHeight.y);
        mainContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //Debug.Log("Setting parent height here: " + _postTabHeight.y);
        //mainContent.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, _postTabHeight.y);
        //mainContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //StartCoroutine(waitToReset());
    }


}