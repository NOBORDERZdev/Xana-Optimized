using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MainScrollController : MonoBehaviour
{
    public GameObject TopFixedObj;
    public GameObject headerObj;

    public RectTransform containerobj;

    public GameObject headerTitleObject;

    public ScrollRectFasterEx m_ScrollRect;
    public float m_InitialPosition;

    public ScrollRectFasterEx subScrollRectFasterEx;
    //public Vector2 screenSizeVector;

    public bool isProfileScreen;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CheckHeight", 0.01f);
        m_InitialPosition = GetContentAnchoredPosition();
    }

    private void OnEnable()
    {

    }

    void CheckHeight()
    {
        float sizeff = containerobj.rect.height - Screen.height;
    }
    public float EndPose = -700;

    private void LateUpdate()
    {
        if (isProfileScreen)
        {
            //if ((headerObj.transform.position.y + 2) >= TopFixedObj.transform.position.y)
            //{
            //    TopFixedObj.SetActive(true);
            //}
            //else
            //{
            //    TopFixedObj.SetActive(false);
            //}
        }
        else
        {
            if ((headerObj.transform.position.y + 60) >= TopFixedObj.transform.position.y)
            {
                TopFixedObj.SetActive(true);
            }
            else
            {
                TopFixedObj.SetActive(false);
            }
        }

        //Debug.Log(" GetContentAnchoredPosition : " + GetContentAnchoredPosition());
        if (!isProfileScreen)
        {
            var distance = m_InitialPosition - GetContentAnchoredPosition();
            if (GetContentAnchoredPosition() >= 5)
            {
                headerTitleObject.SetActive(true);
            }
            else
            {
                headerTitleObject.SetActive(false);
            }
        }
        return;
    }

    private float GetContentAnchoredPosition()
    {
        return m_ScrollRect.content.anchoredPosition.y;
    }
}