using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler , IPointerDownHandler
{
    Vector2 currPos;
    Vector3 newPos;
    RectTransform rt;
    public UICustomizer UI;

    public Vector2 DefaultSize;
    public Vector2 DefaultPos;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        SetSizeandTransparency(PlayerPrefs.GetFloat(transform.name+"size"),PlayerPrefs.GetFloat(transform.name+"transp"));

        rt.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(transform.name + "x"),
        PlayerPrefs.GetFloat(transform.name + "y"));


        //Vector2 anchoredPosition = rectTransform.anchoredPosition;
        //Vector2 sizeDelta = rectTransform.sizeDelta;

        //DefaultPos = rt.anchoredPosition;
        //DefaultSize = rt.sizeDelta;
    }

    private void Update()
    {
        currPos = RectTransformUtility.WorldToScreenPoint(new Camera(),transform.position);
        currPos.x = Mathf.Clamp(currPos.x, rt.sizeDelta.x/2,Screen.width- rt.sizeDelta.x / 2);
        currPos.y = Mathf.Clamp(currPos.y, rt.sizeDelta.y / 2, Screen.height- rt.sizeDelta.y / 2);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt,currPos,new Camera(), out newPos);
        transform.position = newPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void SetSizeandTransparency(float size, float transperncy) {
        rt.sizeDelta = DefaultSize * size;

        GetComponent<Image>().color = new Color(1,1,1,transperncy);

    //    foreach (var g in GetComponentsInChildren<Image>()) {
    //        g.color = new Color(1, 1, 1, transperncy);
    //    }
    //}
}

    public void OnPointerDown(PointerEventData eventData)
    {
        UI.SelectButton = this;
        UI.SetButtonData(rt.sizeDelta.x/DefaultSize.x, GetComponent<Image>().color.a);
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat(transform.name + "size",rt.sizeDelta.x/DefaultSize.x);
        PlayerPrefs.SetFloat(transform.name + "transp", GetComponent<Image>().color.a);
        PlayerPrefs.SetFloat(transform.name + "x", rt.anchoredPosition.x);
        PlayerPrefs.SetFloat(transform.name + "y", rt.anchoredPosition.y);
    }

    public void RestUI()
    {
        SetSizeandTransparency(1f, 1f);
        rt.anchoredPosition = DefaultPos;
    }

    public void LoadUI()
    {
        SetSizeandTransparency(PlayerPrefs.GetFloat(transform.name + "size"), PlayerPrefs.GetFloat(transform.name + "transp"));

        rt.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(transform.name + "x"),
        PlayerPrefs.GetFloat(transform.name + "y"));
    }
}
