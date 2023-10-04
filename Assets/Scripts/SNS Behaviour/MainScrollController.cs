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

    [SerializeField] private ScrollRectFasterEx m_ScrollRect;
    public float m_InitialPosition;

    public ScrollRectFasterEx subScrollRectFasterEx;
    //public Vector2 screenSizeVector;

    public bool isProfileScreen;

    // Start is called before the first frame update
    void Start()
    {
        /*screenSizeVector = new Vector2(Screen.width, Screen.height);
        screenSizeVector = Camera.main.ViewportToWorldPoint(screenSizeVector);*/
        Invoke("CheckHeight", 0.01f);

        m_InitialPosition = GetContentAnchoredPosition();
    }

    private void OnEnable()
    {

    }

    void CheckHeight()
    {
        float sizeff = containerobj.rect.height - Screen.height;
        //Debug.Log("targetpos:" + containerobj.rect.height);
        //Debug.Log("targetpos1:" + containerobj.sizeDelta.y);
    }
    public float EndPose = -700;

    private void LateUpdate()
    {
        if (isProfileScreen)
        {
            if ((headerObj.transform.position.y + 2) >= TopFixedObj.transform.position.y)
            {
                TopFixedObj.SetActive(true);
            }
            else
            {
                TopFixedObj.SetActive(false);
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
       Debug.Log("Collide:" + collision.gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       Debug.Log("Triggerd:" + collision);
    }
}